using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Scripting;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public enum NodeType
    {
        // MapInfo
        MapInfoNode,
        MapInfoActorInfoNode,
        MapInfoActorReference,

        // Actor
        ActorCreateActor,
        ActorGetInformations,
        ActorQueueMove,
        ActorQueueAttack,
        ActorQueueHunt,
        ActorQueueAttackMoveActivity,
        ActorQueueSell,
        ActorQueueFindResources,
        ActorKill,
        ActorRemove,

        // Trigger,
        TriggerWorldLoaded,
        TriggerCreateTimer,
        TimerStart,
        TimerReset,
        TimerStop,
        TriggerTick,
        TriggerOnEnteredFootprint,
        TriggerOnEnteredRange,
        TriggerOnIdle,
        TriggerOnKilled,

        // Actor Groups
        GroupPlayerGroup,
        GroupActorGroup,
        GroupActorInfoGroup,
        TriggerOnAllKilled,

        // Arithmetic
        ArithmeticsAnd,
        ArithmeticsOr,

        // Complex Functions
        Reinforcements,
        ReinforcementsWithTransport,
        CreateEffect,

        // UI Nodes
        UiPlayNotification,
        UiPlaySound,
        UiRadarPing,
        UiTextMessage,
        UiAddMissionText
    }

    public class NodeEditorNodeScreenWidget : Widget
    {
        public readonly string Background = "textfield";

        public int NodeID;
        public WorldRenderer WorldRenderer;
        public World World;

        public NodeEditorBackgroundWidget Bgw;

        public ScriptNodeWidget Snw;

        // Coordinate System
        public int2 CenterCoordinates = new int2(0, 0);
        public int2 CorrectCenterCoordinates = new int2(0, 0);

        public List<NodeWidget> Nodes = new List<NodeWidget>();

        // Position of Mouse Cursor
        int2 oldCursorPosition;

        // Coordinates of the Center
        string text;
        int2 textsize;

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

        NodeLibrary nodeLibrary;

        [ObjectCreator.UseCtor]
        public NodeEditorNodeScreenWidget(ScriptNodeWidget snw, NodeEditorBackgroundWidget bgw, WorldRenderer worldRenderer, World world)
        {
            Snw = snw;
            Bgw = bgw;
            WorldRenderer = worldRenderer;
            World = world;

            nodeLibrary = new NodeLibrary();

            CurrentBrush = NodeBrush.Free;
        }

        public override void Tick()
        {
            Bounds = new Rectangle(Bgw.RenderBounds.X + 200, Bgw.RenderBounds.Y + 5, Bgw.RenderBounds.Width - 205, Bgw.RenderBounds.Height - 10);
            CorrectCenterCoordinates = new int2(RenderBounds.X + (RenderBounds.Width / 2), RenderBounds.Y + (RenderBounds.Height / 2));

            if (tick++ < 1)
            {
                LoadInNodes();

                tick = 2;
            }
        }

        void LoadInNodes()
        {
            Nodes = nodeLibrary.LoadInNodes(this);

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

            NodeID = count;
        }

        public NodeWidget AddNode(NodeType nodeType, string nodeId = null, string nodeName = null)
        {
            var node = nodeLibrary.AddNode(nodeType, this, nodeId, nodeName);

            AddChild(node);
            Nodes.Add(node);

            return node;
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

            if (e.Event == KeyInputEvent.Down && e.Key == Keycode.C && e.Modifiers == Modifiers.Ctrl && selectedNodes.Any())
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
                var newCopyNodes = new List<NodeWidget>();

                foreach (var nodeinfo in copyNodes)
                {
                    var node = AddNode(nodeinfo.NodeType);
                    newCopyNodes.Add(node);

                    if (node != null)
                    {
                        node.OffsetPosX = nodeinfo.OffsetPosX + 20 * copyCounter;
                        node.OffsetPosY = nodeinfo.OffsetPosY + 20 * copyCounter;
                    }
                }

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

            return false;
        }

        public override bool HandleMouseInput(MouseInput mi)
        {
            if (RenderBounds.Contains(mi.Location) && mi.Event == MouseInputEvent.Down)
                TakeKeyboardFocus();

            if (!RenderBounds.Contains(mi.Location) && CurrentBrush == NodeBrush.Free)
            {
                CurrentBrush = NodeBrush.Free;
                brushItem = null;
                nodeBrush = null;
                oldCursorPosition = mi.Location;
                return false;
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
                            && (brushItem.Item2.ConTyp == connection.ConTyp || connection.ConTyp == ConnectionType.Universal))
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

            if (mi.Button == MouseButton.Right && mi.Event == MouseInputEvent.Down && CurrentBrush == NodeBrush.Free)
            {
                if (selectedNodes.Any())
                {
                    foreach (var node in selectedNodes)
                    {
                        node.Selected = false;
                    }

                    selectedNodes = new List<NodeWidget>();
                }
                else
                    CurrentBrush = NodeBrush.Drag;
            }
            else if (mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down && CurrentBrush == NodeBrush.Free)
            {
                CurrentBrush = NodeBrush.Frame;
                selectionStart = mi.Location;
            }

            if (CurrentBrush == NodeBrush.Drag)
            {
                if (mi.Location != oldCursorPosition)
                {
                    CenterCoordinates += oldCursorPosition - mi.Location;
                    CorrectCenterCoordinates = new int2(RenderBounds.Width / 2 + CenterCoordinates.X, RenderBounds.Height / 2 + CenterCoordinates.Y);
                }
            }

            if (CurrentBrush == NodeBrush.Frame)
            {
                var sizeX = Math.Max(selectionStart.X, mi.Location.X) - Math.Min(selectionStart.X, mi.Location.X);
                var sizeY = Math.Max(selectionStart.Y, mi.Location.Y) - Math.Min(selectionStart.Y, mi.Location.Y);

                selectionRectangle = new Rectangle(Math.Min(selectionStart.X, mi.Location.X), Math.Min(selectionStart.Y, mi.Location.Y), sizeX, sizeY);
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

                if (node.WidgetBackground.Contains(mi.Location) && mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down && CurrentBrush == NodeBrush.Free)
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
                    if (node.OutConnections[i].InWidgetPosition.Contains(mi.Location) && mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down &&
                        CurrentBrush == NodeBrush.Free)
                    {
                        CurrentBrush = NodeBrush.Connecting;
                        brushItem = new Tuple<Rectangle, OutConnection>(node.OutConnections[i].InWidgetPosition, node.OutConnections[i]);

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
            textsize = Snw.FontRegular.Measure(text);

            WidgetUtils.DrawPanel(Background, new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6, RenderBounds.Height + 6));

            Snw.FontRegular.DrawTextWithShadow(text, new float2(RenderBounds.X + 2, RenderBounds.Y + 2),
                Color.White, Color.Black, 1);

            Snw.FontRegular.DrawTextWithShadow(CurrentBrush.ToString(), new float2(RenderBounds.X + 2, RenderBounds.Y + 50),
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

                Game.Renderer.RgbaColorRenderer.DrawLine(new int2(brushItem.Item1.X + 10, brushItem.Item1.Y + 10), conTarget,
                    2, brushItem.Item2.Color);
            }

            if (CurrentBrush == NodeBrush.Frame)
            {
                WidgetUtils.FillRectWithColor(selectionRectangle, Color.FromArgb(100, 255, 255, 255));
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