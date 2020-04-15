using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class NodeEditorNodeScreenWidget : Widget
    {
        public readonly string Background = "textfield";

        public int NodeId;
        public readonly World World;
        public readonly WorldRenderer WorldRenderer;

        public readonly NodeEditorBackgroundWidget BackgroundWidget;

        public readonly ScriptNodeWidget ScriptNodeWidget;

        // Coordinate System
        public int2 CenterCoordinates = new int2(0, 0);
        public int2 CorrectCenterCoordinates = new int2(0, 0);

        public List<NodeWidget> Nodes = new List<NodeWidget>();
        public readonly List<VariableInfo> VariableInfos = new List<VariableInfo>();

        // Position of Mouse Cursor
        int2 oldCursorPosition;

        // Coordinates of the Center
        string text;

        public NodeBrush CurrentBrush { get; private set; }
        Tuple<Rectangle, OutConnection> brushItem = null;
        BasicNodeWidget nodeBrush = null;

        // SelectioNFrame
        List<NodeWidget> selectedNodes = new List<NodeWidget>();
        int2 selectionStart;
        Rectangle selectionRectangle;
        int tick;

        List<NodeWidget> copyNodes = new List<NodeWidget>();
        int copyCounter = 0;

        readonly NodeLibrary nodeLibrary;
        int timer;

        DropDownMenuWidget dropDownMenuWidget;
        readonly List<Widget> DropDownMenu = new List<Widget>();

        [ObjectCreator.UseCtor]
        public NodeEditorNodeScreenWidget(ScriptNodeWidget scriptNodeWidget,
            NodeEditorBackgroundWidget backgroundWidget,
            WorldRenderer worldRenderer, World world)
        {
            ScriptNodeWidget = scriptNodeWidget;
            BackgroundWidget = backgroundWidget;
            World = world;
            WorldRenderer = worldRenderer;

            nodeLibrary = new NodeLibrary();

            CurrentBrush = NodeBrush.Free;

            CreateLeftClickDropDownMenu();
        }

        public override void Tick()
        {
            Bounds = new Rectangle(BackgroundWidget.Bounds.X + 200, BackgroundWidget.Bounds.Y + 5,
                BackgroundWidget.Bounds.Width - 355,
                BackgroundWidget.Bounds.Height - 10);
            CorrectCenterCoordinates = new int2((Bounds.Width / 2), (Bounds.Height / 2));

            if (tick++ < 1)
            {
                LoadInVariables();
                LoadInNodes();

                tick = 2;
            }
        }

        void CreateLeftClickDropDownMenu()
        {
            dropDownMenuWidget = new DropDownMenuWidget()
            {
                Bounds = new Rectangle(0, 0, 180, 75),
                Visible = false
            };

            var element1 = new ButtonWidget(ScriptNodeWidget.ModData)
            {
                Bounds = new Rectangle(0, 0, 180, 75),
                Text = "height = 75",
            };
            var element2 = new ButtonWidget(ScriptNodeWidget.ModData)
            {
                Bounds = new Rectangle(0, 0, 180, 35),
                Text = "height = 35",
            };
            var element3 = new ButtonWidget(ScriptNodeWidget.ModData)
            {
                Bounds = new Rectangle(0, 0, 180, 55),
                Text = "height = 55"
            };

            element1.OnClick = () => DropDownMenuWidget.Collapse(dropDownMenuWidget);
            element2.OnClick = () => DropDownMenuWidget.Collapse(dropDownMenuWidget);


            var element4 = new DropDownMenuWidget()
            {
                Bounds = new Rectangle(180, 0, 180, 55),
                Visible = false
            };

            element3.OnClick = () =>
            {
                DropDownMenuWidget.Collapse(dropDownMenuWidget);
                element4.Visible = true;
            };

            element3.AddChild(element4);

            dropDownMenuWidget.AddDropDownMenu(element1);
            dropDownMenuWidget.AddDropDownMenu(element2);
            dropDownMenuWidget.AddDropDownMenu(element3);


            AddChild(dropDownMenuWidget);
        }

        public void AddVariableInfo(VariableInfo info)
        {
            VariableInfos.Add(info);
        }

        public void RemoveVariableInfo(VariableInfo info)
        {
            if (VariableInfos.Contains(info))
                VariableInfos.Remove(info);
        }

        public NodeWidget AddNode(NodeType nodeType, string nodeId = null, string nodeName = null)
        {
            var node = nodeLibrary.AddNode(nodeType, this, nodeId, nodeName);

            if (node != null)
            {
                AddChild(node);
                Nodes.Add(node);

                return node;
            }

            return null;
        }

        string NewId()
        {
            NodeId++;
            return "ND" + (NodeId < 10 ? "0" + NodeId : NodeId.ToString());
        }

        void LoadInVariables()
        {
            foreach (var variableInfo in World.WorldActor.Trait<EditorNodeLayer>().VariableInfos)
            {
                BackgroundWidget.AddNewVariable(variableInfo.VarType, variableInfo.VariableName);
            }
        }

        void LoadInNodes()
        {
            Nodes = nodeLibrary.LoadInNodes(this, World.WorldActor.Trait<EditorNodeLayer>().NodeInfos);

            foreach (var node in Nodes)
            {
                AddChild(node);
            }

            foreach (var node in Nodes)
            {
                node.AddOutConnectionReferences();
            }

            foreach (var node in Nodes)
            {
                node.AddInConnectionReferences();
            }

            int count = 0;
            foreach (var node in Nodes)
            {
                int c;
                int.TryParse(node.NodeInfo.NodeID.Replace("ND", ""), out c);
                count = Math.Max(c, count);
            }

            NodeId = count;
        }

        public void DeleteNode(NodeWidget widget)
        {
            Nodes.Remove(widget);
            RemoveChild(widget);
        }

        public override bool HandleKeyPress(KeyInput e)
        {
            if (!Visible)
                return false;

            if (e.Event == KeyInputEvent.Down && e.Key == Keycode.C && e.Modifiers == Modifiers.Ctrl &&
                selectedNodes.Any())
            {
                copyNodes = new List<NodeWidget>();
                foreach (var node in selectedNodes)
                {
                    copyNodes.Add(node);
                }

                copyCounter = 1;
                return true;
            }

            if (e.Event == KeyInputEvent.Down && e.Key == Keycode.V && e.Modifiers == Modifiers.Ctrl)
            {
                var newCopyNodes = Paste();

                foreach (var node in selectedNodes)
                {
                    node.Selected = false;
                }

                selectedNodes = newCopyNodes;

                foreach (var node in selectedNodes)
                {
                    node.Selected = true;
                }

                copyCounter++;
                return true;
            }

            if (selectedNodes.Any() && e.Event == KeyInputEvent.Down && e.Key == Keycode.DELETE)
            {
                foreach (var node in selectedNodes)
                {
                    DeleteNode(node);
                }

                selectedNodes = new List<NodeWidget>();
            }

            return false;
        }

        List<NodeWidget> Paste()
        {
            List<NodeInfo> infos = new List<NodeInfo>();
            List<NodeWidget> newNodes;

            foreach (var node in copyNodes)
            {
                var newNodeInfo = node.BuildNodeInfo();
                infos.Add(newNodeInfo);
            }

            foreach (var info in infos)
            {
                var oldId = info.NodeID;
                var newId = NewId();

                info.NodeID = newId;
                info.NodeName = null;

                foreach (var subInfo in infos)
                {
                    foreach (var connection in subInfo.InConnectionsReference)
                    {
                        if (connection.WidgetReferenceId == oldId)
                            connection.WidgetReferenceId = newId;
                    }
                }
            }

            newNodes = nodeLibrary.LoadInNodes(this, infos);

            foreach (var node in newNodes)
            {
                Nodes.Add(node);
                AddChild(node);
                if (node != null)
                {
                    node.OffsetPosX -= 20 * copyCounter;
                    node.OffsetPosY -= 20 * copyCounter;
                }
            }

            foreach (var node in newNodes)
            {
                node.AddOutConnectionReferences();
            }

            foreach (var node in newNodes)
            {
                node.AddInConnectionReferences();
            }

            return newNodes;
        }

        public override bool HandleMouseInput(MouseInput mi)
        {
            if (EventBounds.Contains(mi.Location) && mi.Event == MouseInputEvent.Down)
                TakeKeyboardFocus();

            if (!RenderBounds.Contains(mi.Location) && CurrentBrush == NodeBrush.Free)
            {
                CurrentBrush = NodeBrush.Free;
                brushItem = null;
                nodeBrush = null;
                oldCursorPosition = mi.Location;
                dropDownMenuWidget.Visible = false;
                return false;
            }

            if ((mi.Button == MouseButton.Left || mi.Button == MouseButton.Right) && dropDownMenuWidget.Visible &&
                CurrentBrush == NodeBrush.Free)
            {
                DropDownMenuWidget.Collapse(dropDownMenuWidget);
                dropDownMenuWidget.Visible = false;
            }

            if (mi.Button == MouseButton.Middle && CurrentBrush == NodeBrush.Free)
            {
                CurrentBrush = NodeBrush.CreateNode;
                dropDownMenuWidget.Visible = true;
                dropDownMenuWidget.Bounds = new Rectangle(mi.Location.X - Bounds.X, mi.Location.Y - Bounds.Y,
                    dropDownMenuWidget.Bounds.Width,
                    dropDownMenuWidget.Bounds.Height);
            }

            if (mi.Button != MouseButton.Left && mi.Button != MouseButton.Right)
            {
                foreach (var node in Nodes)
                {
                    foreach (var connection in node.InConnections)
                    {
                        if (new Rectangle(connection.InWidgetPosition.X - 20,
                                connection.InWidgetPosition.Y - 20,
                                connection.InWidgetPosition.Width + 40,
                                connection.InWidgetPosition.Height + 40).Contains(mi.Location)
                            && CurrentBrush == NodeBrush.Connecting
                            && brushItem != null
                            && (brushItem.Item2.ConTyp == connection.ConTyp ||
                                connection.ConTyp == ConnectionType.Universal ||
                                brushItem.Item2.ConTyp == ConnectionType.Universal))
                        {
                            connection.In = brushItem.Item2;
                        }
                    }
                }

                if (CurrentBrush == NodeBrush.Frame)
                {
                    foreach (var node in Nodes)
                    {
                        if (selectionRectangle.Contains(node.WidgetBackground) && !selectedNodes.Contains(node))
                        {
                            selectedNodes.Add(node);
                            node.Selected = true;
                        }
                    }
                }

                CurrentBrush = NodeBrush.Free;
                brushItem = null;
                nodeBrush = null;
                oldCursorPosition = mi.Location;
                return false;
            }

            if (CurrentBrush != NodeBrush.Frame)
            {
                if (HandleNodes(mi))
                    return true;
                if (ConnectNodes(mi))
                    return true;
            }

            if (mi.Button == MouseButton.Right)
            {
                timer++;
                if (selectedNodes.Any() && mi.Event == MouseInputEvent.Down)
                {
                    timer = 0;
                }

                if (selectedNodes.Any() && mi.Event == MouseInputEvent.Up && timer < 3)
                {
                    foreach (var node in selectedNodes)
                    {
                        node.Selected = false;
                    }

                    selectedNodes = new List<NodeWidget>();
                }
            }

            if (mi.Button == MouseButton.Right && mi.Event == MouseInputEvent.Down && CurrentBrush == NodeBrush.Free)
            {
                CurrentBrush = NodeBrush.Drag;
            }
            else if (mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down &&
                     CurrentBrush == NodeBrush.Free)
            {
                CurrentBrush = NodeBrush.Frame;
                selectionStart = mi.Location;
            }

            if (CurrentBrush == NodeBrush.Drag)
            {
                if (mi.Location != oldCursorPosition)
                {
                    CenterCoordinates += oldCursorPosition - mi.Location;
                    CorrectCenterCoordinates = new int2(RenderBounds.Width / 2 + CenterCoordinates.X,
                        RenderBounds.Height / 2 + CenterCoordinates.Y);
                }
            }

            if (CurrentBrush == NodeBrush.Frame)
            {
                var sizeX = Math.Max(selectionStart.X, mi.Location.X) - Math.Min(selectionStart.X, mi.Location.X);
                var sizeY = Math.Max(selectionStart.Y, mi.Location.Y) - Math.Min(selectionStart.Y, mi.Location.Y);
                selectionRectangle = new Rectangle(Math.Min(selectionStart.X, mi.Location.X),
                    Math.Min(selectionStart.Y, mi.Location.Y), sizeX, sizeY);
            }

            oldCursorPosition = mi.Location;
            return true;
        }

        public bool HandleNodes(MouseInput mi)
        {
            foreach (var node in Nodes)
            {
                if (CurrentBrush == NodeBrush.Node && nodeBrush == node)
                {
                    node.NewOffset = node.CursorLocation - mi.Location;
                    node.OffsetPosX -= node.NewOffset.X;
                    node.OffsetPosY -= node.NewOffset.Y;
                    node.CursorLocation = mi.Location;
                    return false;
                }

                if (CurrentBrush == NodeBrush.MoveFrame)
                {
                    foreach (var subnode in selectedNodes)
                    {
                        subnode.NewOffset = subnode.CursorLocation - mi.Location;
                        subnode.OffsetPosX -= subnode.NewOffset.X;
                        subnode.OffsetPosY -= subnode.NewOffset.Y;
                        subnode.CursorLocation = mi.Location;
                    }

                    return false;
                }

                if (node.WidgetBackground.Contains(mi.Location) && mi.Button == MouseButton.Left &&
                    mi.Event == MouseInputEvent.Down && CurrentBrush == NodeBrush.Free)
                {
                    if (node.DeleteButton.Contains(mi.Location))
                    {
                        DeleteNode(node);
                        return true;
                    }

                    if (node.DragBar.Contains(mi.Location) && CurrentBrush == NodeBrush.Free)
                    {
                        if (!selectedNodes.Any())
                        {
                            node.CursorLocation = mi.Location;
                            CurrentBrush = NodeBrush.Node;
                            nodeBrush = node;
                        }
                        else
                        {
                            foreach (var subnode in selectedNodes)
                            {
                                subnode.CursorLocation = mi.Location;
                            }

                            CurrentBrush = NodeBrush.MoveFrame;
                        }
                    }
                }
            }

            return false;
        }

        public bool ConnectNodes(MouseInput mi)
        {
            foreach (var node in Nodes)
            {
                for (int i = 0; i < node.OutConnections.Count; i++)
                {
                    if (node.OutConnections[i].InWidgetPosition.Contains(mi.Location) &&
                        mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down &&
                        CurrentBrush == NodeBrush.Free)
                    {
                        CurrentBrush = NodeBrush.Connecting;
                        brushItem = new Tuple<Rectangle, OutConnection>(node.OutConnections[i].InWidgetPosition,
                            node.OutConnections[i]);
                        return true;
                    }
                }

                foreach (var connection in node.InConnections)
                {
                    if (connection.InWidgetPosition.Contains(mi.Location)
                        && mi.Button == MouseButton.Right
                        && mi.Event == MouseInputEvent.Down
                        && CurrentBrush == NodeBrush.Free
                        && connection.In != null)
                    {
                        connection.In.Out = null;
                        connection.In = null;
                        return true;
                    }
                }
            }

            return false;
        }

        public override void Draw()
        {
            text = "X: " + CenterCoordinates.X + " Y: " + CenterCoordinates.Y;
            WidgetUtils.DrawPanel(Background,
                new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6, RenderBounds.Height + 6));
            ScriptNodeWidget.FontRegular.DrawTextWithShadow(text, new float2(RenderBounds.X + 2, RenderBounds.Y + 2),
                Color.White, Color.Black, 1);
            ScriptNodeWidget.FontRegular.DrawTextWithShadow(CurrentBrush.ToString(),
                new float2(RenderBounds.X + 2, RenderBounds.Y + 50),
                Color.White, Color.Black, 1);
            if (brushItem != null && CurrentBrush == NodeBrush.Connecting)
            {
                var conTarget = oldCursorPosition;
                foreach (var nodes in Nodes)
                foreach (var connection in nodes.InConnections)
                {
                    if (new Rectangle(connection.InWidgetPosition.X - 10,
                            connection.InWidgetPosition.Y - 10,
                            connection.InWidgetPosition.Width + 20,
                            connection.InWidgetPosition.Height + 20).Contains(oldCursorPosition)
                        && (connection.ConTyp == brushItem.Item2.ConTyp
                            || connection.ConTyp == ConnectionType.Undefined
                            || connection.ConTyp == ConnectionType.Universal
                            || brushItem.Item2.ConTyp == ConnectionType.Undefined
                            || brushItem.Item2.ConTyp == ConnectionType.Universal))
                    {
                        conTarget = new int2(connection.InWidgetPosition.X + 10, connection.InWidgetPosition.Y + 10);
                        break;
                    }

                    if (conTarget != oldCursorPosition)
                        break;
                }

                NodeEditorNodeScreenWidget.DrawLine(new int2(brushItem.Item1.X + 10, brushItem.Item1.Y + 10), conTarget,
                    brushItem.Item2.Color);
            }

            if (CurrentBrush == NodeBrush.Frame)
            {
                WidgetUtils.FillRectWithColor(selectionRectangle, Color.FromArgb(100, 255, 255, 255));
            }
        }

        public static void DrawLine(int2 from, int2 to, Color color)
        {
            if (from.X > to.X)
            {
                var tmp = from;
                from = to;
                to = tmp;
            }

            var stepSize = 10;
            var yDiff = to.Y - from.Y;

            for (var x = from.X; x < to.X; x += stepSize)
            {
                var currentSegmentStartX = x;
                var currentSegmentEndX = Math.Min(x + stepSize, to.X);

                var currentSegmentStartY =
                    (int) (from.Y + yDiff *
                           Easing.InOutSine((currentSegmentStartX - from.X) / (double) (to.X - from.X)));
                var currentSegmentEndY =
                    (int) (from.Y + yDiff * Easing.InOutSine((currentSegmentEndX - from.X) / (double) (to.X - from.X)));

                Game.Renderer.RgbaColorRenderer.DrawLine(
                    new int2(currentSegmentStartX, currentSegmentStartY),
                    new int2(currentSegmentEndX, currentSegmentEndY),
                    2, color);
            }
        }
    }

    public enum NodeBrush
    {
        Free,
        Connecting,
        Drag,
        Node,
        Frame,
        MoveFrame,
        CreateNode
    }
}