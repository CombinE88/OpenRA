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
        // Widget Properties
        readonly string background = "textfield";

        public readonly NodeEditorBackgroundWidget BackgroundWidget;
        public readonly NodeScriptContainerWidget NodeScriptContainerWidget;
        public readonly List<VariableInfo> VariableInfos = new List<VariableInfo>();
        public readonly World World;
        public readonly WorldRenderer WorldRenderer;
        Tuple<Rectangle, OutConnection> brushItem;

        // Coordinate System
        public int2 CenterCoordinates = new int2(0, 0);
        int copyCounter;

        // Copy Paste nodes storage
        List<NodeWidget> copyNodes = new List<NodeWidget>();
        bool first;
        public int2 MouseOffsetCoordinates = new int2(0, 0);
        BasicNodeWidget nodeBrush;

        // Saved Nodes and Variables
        public List<NodeWidget> Nodes = new List<NodeWidget>();

        // Position of Mouse Cursor
        int2 oldCursorPosition;

        public int RunningNodeId;

        // SelectioNFrame
        List<NodeWidget> selectedNodes = new List<NodeWidget>();
        Rectangle selectionRectangle;
        int2 selectionStart;

        // Coordinates of the Center
        string text;

        // Mouse counterTimer
        int timer;
        public int2 WidgetScreenCenterCoordinates = new int2(0, 0);

        [ObjectCreator.UseCtorAttribute]
        public NodeEditorNodeScreenWidget(NodeScriptContainerWidget nodeScriptContainerWidget,
            NodeEditorBackgroundWidget backgroundWidget,
            WorldRenderer worldRenderer, World world)
        {
            NodeScriptContainerWidget = nodeScriptContainerWidget;
            BackgroundWidget = backgroundWidget;
            World = world;
            WorldRenderer = worldRenderer;
            CurrentBrush = NodeBrush.Free;
        }

        // Working Brush
        public NodeBrush CurrentBrush { get; private set; }

        public override void Tick()
        {
            if (first)
                return;

            LoadInVariables();
            LoadInNodes();
            first = true;
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

        public void AddNode(NodeType nodeType, string nodeId = null, string nodeName = null)
        {
            var node = NodeLibrary.AddNode(nodeType, this, nodeId, nodeName);

            if (node == null)
                return;

            AddChild(node);
            Nodes.Add(node);
        }

        string FetchNewAvailableId()
        {
            RunningNodeId++;
            return "ND" + (RunningNodeId < 10 ? "0" + RunningNodeId : RunningNodeId.ToString());
        }

        void LoadInVariables()
        {
            foreach (var variableInfo in World.WorldActor.Trait<EditorNodeLayer>().VariableInfos)
                BackgroundWidget.AddNewVariable(variableInfo.VarType, variableInfo.VariableName);
        }

        void LoadInNodes()
        {
            Nodes = NodeLibrary.LoadInNodes(this, World.WorldActor.Trait<EditorNodeLayer>().NodeInfos);
            foreach (var node in Nodes)
                AddChild(node);

            foreach (var node in Nodes)
                node.AddOutConnectionReferences();

            foreach (var node in Nodes)
                node.AddInConnectionReferences();

            var count = 0;
            foreach (var node in Nodes)
            {
                int c;
                int.TryParse(node.NodeInfo.NodeId.Replace("ND", ""), out c);
                count = Math.Max(c, count);
            }

            RunningNodeId = count;
        }

        void RemoveNode(NodeWidget widget)
        {
            Nodes.Remove(widget);
            RemoveChild(widget);
        }

        public override bool HandleKeyPress(KeyInput e)
        {
            if (!Visible)
                return false;

            // Copy selected nodes
            if (e.Event == KeyInputEvent.Down && e.Key == Keycode.C && e.Modifiers == Modifiers.Ctrl &&
                selectedNodes.Any())
            {
                copyNodes = new List<NodeWidget>();
                foreach (var node in selectedNodes) copyNodes.Add(node);

                copyCounter = 1;
                return true;
            }

            // Paste stored nodes
            if (e.Event == KeyInputEvent.Down && e.Key == Keycode.V && e.Modifiers == Modifiers.Ctrl)
            {
                var newCopyNodes = PasteStoredNodes();

                foreach (var node in selectedNodes) node.Selected = false;

                selectedNodes = newCopyNodes;

                foreach (var node in selectedNodes) node.Selected = true;

                copyCounter++;
                return true;
            }

            // Delete selected nodes
            if (!selectedNodes.Any() || e.Event != KeyInputEvent.Down || e.Key != Keycode.DELETE)
                return false;

            foreach (var node in selectedNodes)
                RemoveNode(node);

            selectedNodes = new List<NodeWidget>();

            return false;
        }

        List<NodeWidget> PasteStoredNodes()
        {
            var infos = copyNodes.Select(node => node.BuildNodeInfo()).ToList();

            foreach (var info in infos)
            {
                var oldId = info.NodeId;
                var newId = FetchNewAvailableId();

                info.NodeId = newId;
                info.NodeName = null;

                foreach (var connection in infos.SelectMany(subInfo =>
                    subInfo.InConnectionsReference.Where(connection => connection.WidgetReferenceId == oldId)))
                    connection.WidgetReferenceId = newId;
            }

            // Create new nodes with nodeInfos of stored nodes
            var newNodes = NodeLibrary.LoadInNodes(this, infos);

            foreach (var node in newNodes)
            {
                Nodes.Add(node);
                AddChild(node);

                if (node == null)
                    continue;

                node.OffsetPosX -= 20 * copyCounter;
                node.OffsetPosY -= 20 * copyCounter;
            }

            foreach (var node in newNodes)
                node.AddOutConnectionReferences();

            foreach (var node in newNodes)
                node.AddInConnectionReferences();

            return newNodes;
        }

        public override bool HandleMouseInput(MouseInput mi)
        {
            if (EventBounds.Contains(mi.Location) && mi.Event == MouseInputEvent.Down)
                TakeKeyboardFocus();

            // Clear Brush
            if (!RenderBounds.Contains(mi.Location) && CurrentBrush == NodeBrush.Free)
            {
                CurrentBrush = NodeBrush.Free;
                brushItem = null;
                nodeBrush = null;
                oldCursorPosition = mi.Location;
                BackgroundWidget.DropDownMenuWidget.Visible = false;
                YieldKeyboardFocus();
                return false;
            }

            // Close open dropDownMenus
            if ((mi.Button == MouseButton.Left || mi.Button == MouseButton.Right) &&
                BackgroundWidget.DropDownMenuWidget.Visible &&
                CurrentBrush == NodeBrush.Free)
            {
                DropDownMenuWidget.Collapse(BackgroundWidget.DropDownMenuWidget);
                BackgroundWidget.DropDownMenuWidget.Visible = false;
            }

            // Open dropDownMenu for new node Creation
            if (mi.Button == MouseButton.Middle && CurrentBrush == NodeBrush.Free)
            {
                var newMouseGridCoordinates = new int2(mi.Location.X - BackgroundWidget.Bounds.X - Bounds.X,
                    mi.Location.Y - BackgroundWidget.Bounds.Y - Bounds.Y);

                MouseOffsetCoordinates =
                    new int2(CenterCoordinates.X - (RenderBounds.Width / 2 - newMouseGridCoordinates.X),
                        CenterCoordinates.Y - (RenderBounds.Height / 2 - newMouseGridCoordinates.Y));

                // Make the Widget visible
                BackgroundWidget.DropDownMenuWidget.Visible = true;
                // Orient the menu to mouse cursor
                BackgroundWidget.DropDownMenuWidget.Bounds = new Rectangle(newMouseGridCoordinates.X,
                    newMouseGridCoordinates.Y,
                    BackgroundWidget.DropDownMenuWidget.Bounds.Width,
                    BackgroundWidget.DropDownMenuWidget.Bounds.Height);
            }

            // Connecting lines
            if (mi.Button != MouseButton.Left && mi.Button != MouseButton.Right)
            {
                foreach (var connection in Nodes.SelectMany(node => node.InConnections.Where(connection =>
                    new Rectangle(connection.InWidgetPosition.X - 20,
                        connection.InWidgetPosition.Y - 20,
                        connection.InWidgetPosition.Width + 40,
                        connection.InWidgetPosition.Height + 40).Contains(mi.Location)
                    && CurrentBrush == NodeBrush.Connecting
                    && brushItem != null
                    && (brushItem.Item2.ConnectionTyp == connection.ConnectionTyp ||
                        connection.ConnectionTyp == ConnectionType.Universal ||
                        brushItem.Item2.ConnectionTyp == ConnectionType.Universal))))
                {
                    connection.In = brushItem.Item2;
                    
                    // May only have one execution line per execution OutConnection
                    if (brushItem.Item2.ConnectionTyp == ConnectionType.Exec)
                        foreach (var con in Nodes.SelectMany(node =>
                            node.InConnections.Where(con => con.In == brushItem.Item2 && connection != con)))
                            con.In = null;
                }

                // Frame nodes for selecting
                if (CurrentBrush == NodeBrush.Frame)
                    foreach (var node in Nodes.Where(node =>
                        selectionRectangle.Contains(node.WidgetBackground) && !selectedNodes.Contains(node)))
                    {
                        selectedNodes.Add(node);
                        node.Selected = true;
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
                if (selectedNodes.Any() && mi.Event == MouseInputEvent.Down) timer = 0;

                if (selectedNodes.Any() && mi.Event == MouseInputEvent.Up && timer < 3)
                {
                    foreach (var node in selectedNodes) node.Selected = false;

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

            switch (CurrentBrush)
            {
                case NodeBrush.Drag:
                {
                    if (mi.Location != oldCursorPosition) CenterCoordinates += oldCursorPosition - mi.Location;

                    break;
                }
                case NodeBrush.Frame:
                {
                    var sizeX = Math.Max(selectionStart.X, mi.Location.X) - Math.Min(selectionStart.X, mi.Location.X);
                    var sizeY = Math.Max(selectionStart.Y, mi.Location.Y) - Math.Min(selectionStart.Y, mi.Location.Y);
                    selectionRectangle = new Rectangle(Math.Min(selectionStart.X, mi.Location.X),
                        Math.Min(selectionStart.Y, mi.Location.Y), sizeX, sizeY);
                    break;
                }
            }

            oldCursorPosition = mi.Location;
            return true;
        }

        bool HandleNodes(MouseInput mi)
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

                if (!node.WidgetBackground.Contains(mi.Location) || mi.Button != MouseButton.Left ||
                    mi.Event != MouseInputEvent.Down || CurrentBrush != NodeBrush.Free) continue;
                {
                    if (node.DeleteButton.Contains(mi.Location))
                    {
                        RemoveNode(node);
                        return true;
                    }

                    if (!node.DragBar.Contains(mi.Location) || CurrentBrush != NodeBrush.Free) continue;
                    if (!selectedNodes.Any())
                    {
                        node.CursorLocation = mi.Location;
                        CurrentBrush = NodeBrush.Node;
                        nodeBrush = node;
                    }
                    else
                    {
                        foreach (var subnode in selectedNodes) subnode.CursorLocation = mi.Location;

                        CurrentBrush = NodeBrush.MoveFrame;
                    }
                }
            }

            return false;
        }

        bool ConnectNodes(MouseInput mi)
        {
            foreach (var node in Nodes)
            {
                foreach (var outCon in node.OutConnections.Where(outCon =>
                    outCon.InWidgetPosition.Contains(mi.Location) && mi.Button == MouseButton.Left &&
                    mi.Event == MouseInputEvent.Down && CurrentBrush == NodeBrush.Free))
                {
                    CurrentBrush = NodeBrush.Connecting;
                    brushItem = new Tuple<Rectangle, OutConnection>(outCon.InWidgetPosition,
                        outCon);
                    return true;
                }

                foreach (var connection in node.InConnections.Where(connection =>
                    connection.InWidgetPosition.Contains(mi.Location)
                    && mi.Button == MouseButton.Right
                    && mi.Event == MouseInputEvent.Down
                    && CurrentBrush == NodeBrush.Free
                    && connection.In != null))
                {
                    connection.In.Out = null;
                    connection.In = null;
                    return true;
                }
            }

            return false;
        }

        public override void Draw()
        {
            text = "X: " + CenterCoordinates.X + " Y: " + CenterCoordinates.Y;
            WidgetUtils.DrawPanel(background,
                new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6, RenderBounds.Height + 6));
            NodeScriptContainerWidget.FontRegular.DrawTextWithShadow(text,
                new float2(RenderBounds.X + 2, RenderBounds.Y + 2),
                Color.White, Color.Black, 1);
            NodeScriptContainerWidget.FontRegular.DrawTextWithShadow(CurrentBrush.ToString(),
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
                        && (connection.ConnectionTyp == brushItem.Item2.ConnectionTyp
                            || connection.ConnectionTyp == ConnectionType.Undefined
                            || connection.ConnectionTyp == ConnectionType.Universal
                            || brushItem.Item2.ConnectionTyp == ConnectionType.Undefined
                            || brushItem.Item2.ConnectionTyp == ConnectionType.Universal))
                    {
                        conTarget = new int2(connection.InWidgetPosition.X + 10, connection.InWidgetPosition.Y + 10);
                        break;
                    }

                    if (conTarget != oldCursorPosition)
                        break;
                }

                DrawLine(new int2(brushItem.Item1.X + 10, brushItem.Item1.Y + 10), conTarget,
                    brushItem.Item2.Color);
            }

            if (CurrentBrush == NodeBrush.Frame)
                WidgetUtils.FillRectWithColor(selectionRectangle, Color.FromArgb(100, 255, 255, 255));
        }

        public static void DrawLine(int2 from, int2 to, Color color)
        {
            if (from.X > to.X)
            {
                var tmp = from;
                from = to;
                to = tmp;
            }

            const int stepSize = 10;

            var yDiff = to.Y - from.Y;
            for (var x = from.X;
                x < to.X;
                x += stepSize)
            {
                var currentSegmentStartX = x;
                var currentSegmentEndX = Math.Min(x + stepSize, to.X);

                var currentSegmentStartY =
                    (int) (from.Y + yDiff *
                           Easing.InOutCubic((currentSegmentStartX - from.X) / (double) (to.X - from.X)));
                var currentSegmentEndY =
                    (int) (from.Y + yDiff *
                           Easing.InOutCubic((currentSegmentEndX - from.X) / (double) (to.X - from.X)));

                Game.Renderer.RgbaColorRenderer.DrawLine(
                    new int2(currentSegmentStartX, currentSegmentStartY),
                    new int2(currentSegmentEndX, currentSegmentEndY),
                    3, color);
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
        MoveFrame
    }
}