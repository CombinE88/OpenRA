using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using OpenRA.Mods.Common.Scripting;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public enum NodeType
    {
        // MapInfo
        MapInfoNode,

        // Actor
        ActorCreateActor,
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
        TriggerTick,
        TriggerOnEnteredFootprint,
        TriggerOnEnteredRange,

        // Actor Groups
        GroupPlayerGroup,
        GroupActorGroup,
        GroupActorInfoGroup,

        // Arithmetic
        Arithmetics,

        // Complex Functions
        Reinforcements,
        ReinforcementsWithTransport
    }

    public class NodeEditorNodeScreenWidget : Widget
    {
        // Naming

        public int NodeID;

        // Background
        public readonly string Background = "textfield";

        public NodeEditorBackgroundWidget Bgw;

        public ScriptNodeWidget Snw;

        // Coordinate System
        public int2 CenterCoordinates = new int2(0, 0);
        public int2 CorrectCenterCoordinates = new int2(0, 0);

        // Position of Mouse Cursor
        int2 oldCursorPosition;

        // Coordinates of the Center
        string text;
        int2 textsize;

        NodeBrush currentBrush = NodeBrush.Free;
        Tuple<Rectangle, OutConnection> BrushItem = null;
        BasicNodeWidget nodeBrush = null;

        public List<NodeWidget> Nodes = new List<NodeWidget>();

        // SelectioNFrame
        List<NodeWidget> selectedNodes = new List<NodeWidget>();
        int2 selectionStart;
        Rectangle selectionRectangle;
        int tick;

        List<NodeWidget> copyNodes = new List<NodeWidget>();
        int copyCounter = 0;


        [ObjectCreator.UseCtor]
        public NodeEditorNodeScreenWidget(ScriptNodeWidget snw, NodeEditorBackgroundWidget bgw)
        {
            Snw = snw;
            this.Bgw = bgw;
        }

        public override void Tick()
        {
            Bounds = new Rectangle(Bgw.RenderBounds.X + 200, Bgw.RenderBounds.Y + 5, Bgw.RenderBounds.Width - 205, Bgw.RenderBounds.Height - 10);
            CorrectCenterCoordinates = new int2(RenderBounds.X + (RenderBounds.Width / 2), RenderBounds.Y + (RenderBounds.Height / 2));

            if (tick++ < 10)
            {
                LoadInNodes();

                tick = 11;
            }
        }

        public void LoadInNodes()
        {
            foreach (var nodeinfo in Snw.World.WorldActor.Trait<EditorNodeLayer>().NodeInfo)
            {
                if (nodeinfo.NodeType == NodeType.MapInfoNode)
                {
                    var newNode = new MapInfoNode(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }

                else if (nodeinfo.NodeType == NodeType.TriggerCreateTimer)
                {
                    var newNode = new TriggerNodeCreateTimer(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }

                else if (nodeinfo.NodeType == NodeType.TriggerWorldLoaded)
                {
                    var newNode = new TriggerNodeWorldLoaded(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }

                else if (nodeinfo.NodeType == NodeType.TriggerTick)
                {
                    var newNode = new TriggerNodeTick(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }

                else if (nodeinfo.NodeType == NodeType.TriggerOnEnteredFootprint)
                {
                    var newNode = new TriggerNodeOnEnteredFootPrint(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }

                else if (nodeinfo.NodeType == NodeType.TriggerOnEnteredRange)
                {
                    var newNode = new TriggerNodeOnEnteredRange(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }

                else if (nodeinfo.NodeType == NodeType.ActorCreateActor)
                {
                    var newNode = new ActorNodeCreateActor(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }

                else if (nodeinfo.NodeType == NodeType.GroupPlayerGroup)
                {
                    var newNode = new GroupPlayerGroup(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }

                else if (nodeinfo.NodeType == NodeType.ActorQueueMove)
                {
                    var newNode = new ActorNodeQueueAbility(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueAttack)
                {
                    var newNode = new ActorNodeQueueAbility(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueHunt)
                {
                    var newNode = new ActorNodeQueueAbility(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueAttackMoveActivity)
                {
                    var newNode = new ActorNodeQueueAbility(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueSell)
                {
                    var newNode = new ActorNodeQueueAbility(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueFindResources)
                {
                    var newNode = new ActorNodeQueueAbility(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorKill)
                {
                    var newNode = new ActorNodeQueueAbility(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorRemove)
                {
                    var newNode = new ActorNodeQueueAbility(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.Reinforcements)
                {
                    var newNode = new FunctionNodeReinforcements(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ReinforcementsWithTransport)
                {
                    var newNode = new FunctionNodeReinforcements(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.GroupActorGroup)
                {
                    var newNode = new GroupActorGroup(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.GroupActorInfoGroup)
                {
                    var newNode = new GroupActorInfoGroup(this, nodeinfo);
                    Nodes.Add(newNode);
                    AddChild(newNode);
                }
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
            NodeWidget newNode = null;

            if (nodeType == NodeType.MapInfoNode)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);
                newNode = new MapInfoNode(this, nodeInfo);
                AddChild(newNode);
                Nodes.Add(newNode);
            }

            else if (nodeType == NodeType.TriggerCreateTimer)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new TriggerNodeCreateTimer(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Boolean, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.TimerConnection, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));

                AddChild(newNode);
                Nodes.Add(newNode);
            }

            else if (nodeType == NodeType.TriggerWorldLoaded)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new TriggerNodeWorldLoaded(this, nodeInfo);

                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));

                AddChild(newNode);
                Nodes.Add(newNode);
            }

            else if (nodeType == NodeType.TriggerTick)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new TriggerNodeTick(this, nodeInfo);

                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));

                AddChild(newNode);
                Nodes.Add(newNode);
            }

            else if (nodeType == NodeType.TriggerOnEnteredFootprint)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new TriggerNodeOnEnteredFootPrint(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.PlayerGroup, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.CellArray, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Boolean, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));

                AddChild(newNode);
                Nodes.Add(newNode);
            }

            else if (nodeType == NodeType.TriggerOnEnteredFootprint)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new TriggerNodeOnEnteredRange(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.PlayerGroup, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Location, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Boolean, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));

                AddChild(newNode);
                Nodes.Add(newNode);
            }

            else if (nodeType == NodeType.ActorCreateActor)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeCreateActor(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.ActorInfo, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Player, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Location, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Actor, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));

                AddChild(newNode);
                Nodes.Add(newNode);
            }

            else if (nodeType == NodeType.GroupPlayerGroup)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Player, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.PlayerGroup, newNode));

                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.GroupActorGroup)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new GroupActorGroup(this, nodeInfo);

                newNode.AddOutConnection(new OutConnection(ConnectionType.ActorList, newNode));

                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.GroupActorInfoGroup)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new GroupActorInfoGroup(this, nodeInfo);

                newNode.AddOutConnection(new OutConnection(ConnectionType.ActorInfos, newNode));

                AddChild(newNode);
                Nodes.Add(newNode);
            }

            // Actor and Activities
            else if (nodeType == NodeType.ActorKill)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.InConTexts.Add("Actor");
                newNode.InConTexts.Add("Trigger");

                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ActorRemove)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.InConTexts.Add("Actor");
                newNode.InConTexts.Add("Trigger");

                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ActorQueueMove)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Location, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.InConTexts.Add("Actor");
                newNode.InConTexts.Add("Location");
                newNode.InConTexts.Add("CloseEnough");
                newNode.InConTexts.Add("Trigger");

                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ActorQueueAttack)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Boolean, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Boolean, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.InConTexts.Add("Actor");
                newNode.InConTexts.Add("Actor target");
                newNode.InConTexts.Add("Allow Movement");
                newNode.InConTexts.Add("Force Attack");
                newNode.InConTexts.Add("Facing Tolerance");
                newNode.InConTexts.Add("Trigger");

                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ActorQueueHunt)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.InConTexts.Add("Actor");
                newNode.InConTexts.Add("Trigger");

                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ActorQueueAttackMoveActivity)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Location, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.InConTexts.Add("Actor");
                newNode.InConTexts.Add("Location");
                newNode.InConTexts.Add("Trigger");

                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ActorQueueSell)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Boolean, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.InConTexts.Add("Actor");
                newNode.InConTexts.Add("Show Ticks");
                newNode.InConTexts.Add("Trigger");

                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ActorQueueFindResources)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.InConTexts.Add("Actor");
                newNode.InConTexts.Add("Trigger");

                AddChild(newNode);
                Nodes.Add(newNode);
            }

            // Complex Nodes
            else if (nodeType == NodeType.Reinforcements)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new FunctionNodeReinforcements(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Player, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorInfos, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.CellPath, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.InConTexts.Add("Player");
                newNode.InConTexts.Add("Actors");
                newNode.InConTexts.Add("Path Spawn - End");
                newNode.InConTexts.Add("Spawn interval");
                newNode.InConTexts.Add("Trigger");

                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ReinforcementsWithTransport)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new FunctionNodeReinforcements(this, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Player, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorInfo, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorInfos, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.CellPath, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.CellPath, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.InConTexts.Add("Player");
                newNode.InConTexts.Add("Transport");
                newNode.InConTexts.Add("Actors");
                newNode.InConTexts.Add("Path Entry");
                newNode.InConTexts.Add("Path Exit");
                newNode.InConTexts.Add("Trigger");

                AddChild(newNode);
                Nodes.Add(newNode);
            }

            return newNode;
        }

        public void DeleteNode(NodeWidget widget)
        {
            Nodes.Remove(widget);
            RemoveChild(widget);
        }

        public override bool HandleKeyPress(KeyInput e)
        {
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
                foreach (var nodeinfo in copyNodes)
                {
                    var node = AddNode(nodeinfo.NodeType);
                    if (node != null)
                    {
                        node.OffsetPosX = nodeinfo.OffsetPosX + 20 * copyCounter;
                        node.OffsetPosY = nodeinfo.OffsetPosY + 20 * copyCounter;
                    }
                }

                copyCounter++;
                return true;
            }

            return false;
        }

        public override bool HandleMouseInput(MouseInput mi)
        {
            if (!RenderBounds.Contains(mi.Location) && currentBrush == NodeBrush.Free)
            {
                currentBrush = NodeBrush.Free;
                BrushItem = null;
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
                        if (connection.InWidgetPosition.Contains(mi.Location)
                            && currentBrush == NodeBrush.Connecting
                            && BrushItem != null
                            && (BrushItem.Item2.ConTyp == connection.ConTyp || connection.ConTyp == ConnectionType.Universal))
                        {
                            connection.In = BrushItem.Item2;
                        }
                    }
                }

                if (currentBrush == NodeBrush.Frame)
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

                currentBrush = NodeBrush.Free;
                BrushItem = null;
                nodeBrush = null;
                oldCursorPosition = mi.Location;
                return false;
            }

            if (currentBrush != NodeBrush.Frame)
            {
                if (HandleNodes(mi))
                    return true;

                if (ConnectNodes(mi))
                    return true;
            }

            if (mi.Button == MouseButton.Right && mi.Event == MouseInputEvent.Down && currentBrush == NodeBrush.Free)
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
                    currentBrush = NodeBrush.Drag;
            }
            else if (mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down && currentBrush == NodeBrush.Free)
            {
                currentBrush = NodeBrush.Frame;
                selectionStart = mi.Location;
            }

            if (currentBrush == NodeBrush.Drag)
            {
                if (mi.Location != oldCursorPosition)
                {
                    CenterCoordinates += oldCursorPosition - mi.Location;
                    CorrectCenterCoordinates = new int2(RenderBounds.Width / 2 + CenterCoordinates.X, RenderBounds.Height / 2 + CenterCoordinates.Y);
                }
            }

            if (currentBrush == NodeBrush.Frame)
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
                if (currentBrush == NodeBrush.Node && nodeBrush == node)
                {
                    node.NewOffset = node.CursorLocation - mi.Location;
                    node.OffsetPosX -= node.NewOffset.X;
                    node.OffsetPosY -= node.NewOffset.Y;
                    node.CursorLocation = mi.Location;
                    return false;
                }

                if (currentBrush == NodeBrush.MoveFrame)
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

                if (node.WidgetBackground.Contains(mi.Location) && mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down && currentBrush == NodeBrush.Free)
                {
                    if (node.DeleteButton.Contains(mi.Location))
                    {
                        DeleteNode(node);
                        return true;
                    }

                    if (node.DragBar.Contains(mi.Location) && currentBrush == NodeBrush.Free)
                    {
                        if (!selectedNodes.Any())
                        {
                            node.CursorLocation = mi.Location;
                            currentBrush = NodeBrush.Node;
                            nodeBrush = node;
                        }
                        else
                        {
                            foreach (var subnode in selectedNodes)
                            {
                                subnode.CursorLocation = mi.Location;
                            }

                            currentBrush = NodeBrush.MoveFrame;
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
                        currentBrush == NodeBrush.Free)
                    {
                        currentBrush = NodeBrush.Connecting;
                        BrushItem = new Tuple<Rectangle, OutConnection>(node.OutConnections[i].InWidgetPosition, node.OutConnections[i]);

                        return true;
                    }
                }

                foreach (var connection in node.InConnections)
                {
                    if (connection.InWidgetPosition.Contains(mi.Location)
                        && mi.Button == MouseButton.Right
                        && mi.Event == MouseInputEvent.Down
                        && currentBrush == NodeBrush.Free
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

            Snw.FontRegular.DrawTextWithShadow(currentBrush.ToString(), new float2(RenderBounds.X + 2, RenderBounds.Y + 50),
                Color.White, Color.Black, 1);

            if (BrushItem != null && currentBrush == NodeBrush.Connecting)
            {
                Game.Renderer.RgbaColorRenderer.DrawLine(new int2(BrushItem.Item1.X + 10, BrushItem.Item1.Y + 10), oldCursorPosition,
                    2, BrushItem.Item2.Color);
            }

            if (currentBrush == NodeBrush.Frame)
            {
                WidgetUtils.FillRectWithColor(selectionRectangle, Color.FromArgb(100, 255, 255, 255));
            }
        }
    }

    enum NodeBrush
    {
        Free,
        Connecting,
        Drag,
        Node,
        Frame,
        MoveFrame
    }
}