using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorArrayNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetic;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ComplexFunctrions;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Trigger;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public enum NodeType
    {
        // Outputs
        ActorOutPut,
        PathNode,
        PlayerOutput,
        LocationOutput,
        CellArrayOutput,
        CellRange,
        InfoStrings,

        // Actor
        CreateActor,
        MoveActor,
        ActorFollowPath,
        KillActor,
        RemoveActor,
        ActorInfo,
        QueueAction,

        // Trigger
        ActorKilledTrigger,
        ActorIdleTrigger,
        MathTimerTrigger,
        WorldLoaded,

        // Acto Groups
        DefineGroup,
        FindActorsInCircle,
        FindActorsOnCells,

        // Arithmetic
        SelectBy,
        Select,
        Compare,
        ForEach,

        // Complex Functions
        Reinforcments,
        ReinforcWithTransPort
    }

    public class NodeEditorNodeScreenWidget : Widget
    {
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
        SimpleNodeWidget nodeBrush = null;

        public List<SimpleNodeWidget> Nodes = new List<SimpleNodeWidget>();

        // SelectioNFrame
        List<SimpleNodeWidget> selectedNodes = new List<SimpleNodeWidget>();
        int2 selectionStart;
        Rectangle selectionRectangle;


        [ObjectCreator.UseCtor]
        public NodeEditorNodeScreenWidget(ScriptNodeWidget snw, NodeEditorBackgroundWidget bgw)
        {
            Snw = snw;
            this.Bgw = bgw;
        }

        public override void Tick()
        {
            Bounds = new Rectangle(Bgw.RenderBounds.X + 200, Bgw.RenderBounds.Y + 5, Bgw.RenderBounds.Width - 205, Bgw.RenderBounds.Height - 10);
            CorrectCenterCoordinates = new int2((RenderBounds.Width / 2) + CenterCoordinates.X, (RenderBounds.Height / 2) + CenterCoordinates.Y);
        }

        public void AddNode(NodeType nodeType)
        {
            // Outputs
            if (nodeType == NodeType.ActorOutPut)
            {
                var newNode = new ActorInfoOutPutWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.PathNode)
            {
                var newNode = new PathNodeWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.PlayerOutput)
            {
                var newNode = new PlayerOutPutWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.LocationOutput)
            {
                var newNode = new LocationOutputWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.CellArrayOutput)
            {
                var newNode = new CellArrayWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.CellRange)
            {
                var newNode = new CelLRangeWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.InfoStrings)
            {
                var newNode = new InfoStringsWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }

            // Actor
            else if (nodeType == NodeType.CreateActor)
            {
                var newNode = new CreateActorNodeWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.KillActor)
            {
                var newNode = new KillActorWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.RemoveActor)
            {
                var newNode = new DisposeActorWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.MoveActor)
            {
                var newNode = new MoveActorWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ActorFollowPath)
            {
                var newNode = new MovePathActorWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ActorInfo)
            {
                var newNode = new ActorInformationsWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }

            // Trigger
            else if (nodeType == NodeType.WorldLoaded)
            {
                var newNode = new TriggerWorldLoadedWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ActorIdleTrigger)
            {
                var newNode = new ActorTriggerOnIldeWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ActorKilledTrigger)
            {
                var newNode = new ActorTriggerKilled(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.MathTimerTrigger)
            {
                var newNode = new MAthTimerTriggerWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }

            //Groups
            else if (nodeType == NodeType.DefineGroup)
            {
                var newNode = new ActorGroupWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.FindActorsInCircle)
            {
                var newNode = new FindActorsInCircle(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.FindActorsOnCells)
            {
                var newNode = new FindActorsOnCells(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }

            // Arithmetics
            else if (nodeType == NodeType.SelectBy)
            {
                var newNode = new SelectByWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.Select)
            {
                var newNode = new SelectWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.Compare)
            {
                var newNode = new ArithmeticsCompare(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ForEach)
            {
                var newNode = new ArithmeticForEachWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }

            // Function
            else if (nodeType == NodeType.Reinforcments)
            {
                var newNode = new ComplexReinforcementsWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
            else if (nodeType == NodeType.ReinforcWithTransPort)
            {
                var newNode = new ComplexReinforcementsWithTransportWidget(this);
                AddChild(newNode);
                Nodes.Add(newNode);
            }
        }

        public void DeleteNode(SimpleNodeWidget widget)
        {
            Nodes.Remove(widget);
            RemoveChild(widget);
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
                            && (BrushItem.Item2.conTyp == connection.conTyp || connection.conTyp == ConnectionType.Universal || BrushItem.Item2.conTyp == ConnectionType.Universal))
                        {
                            connection.In = BrushItem.Item2;
                            BrushItem.Item2.Out = connection;
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

                    selectedNodes = new List<SimpleNodeWidget>();
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

                    /*
                    if (node.WidgetEntries.Contains(mi.Location))
                    {
                        if (node.AddInput.Contains(mi.Location))
                            node.AddInConnection();
                        else if (node.RemoveInput.Contains(mi.Location))
                            node.RemoveInConnection();
                        else if (node.AddOutput.Contains(mi.Location))
                            node.AddOutConnection();
                        else if (node.RemoveOutput.Contains(mi.Location))
                            node.RemoveOutConnection();
                    }
                    */

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
            // WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6, RenderBounds.Height + 6), Color.Black);
            // WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height), Color.DarkGray);

            Snw.FontRegular.DrawTextWithShadow(text, new float2(RenderBounds.X + 2, RenderBounds.Y + 2),
                Color.White, Color.Black, 1);

            Snw.FontRegular.DrawTextWithShadow(currentBrush.ToString(), new float2(RenderBounds.X + 2, RenderBounds.Y + 50),
                Color.White, Color.Black, 1);

            if (BrushItem != null && currentBrush == NodeBrush.Connecting)
            {
                Game.Renderer.RgbaColorRenderer.DrawLine(new int2(BrushItem.Item1.X + 10, BrushItem.Item1.Y + 10), oldCursorPosition,
                    2, BrushItem.Item2.color);
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