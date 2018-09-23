using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Traits.Render;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class NodeEditorNodeScreenWidget : Widget
    {
        NodeEditorBackgroundWidget bgw;

        public ScriptNodeWidget Snw;

        // Coordinate System
        public int2 CenterCoordinates = new int2(0, 0);
        public int2 CorrectCenterCoordinates = new int2(0, 0);

        // Position of Mouse Cursor
        int2 oldCursorPosition;

        // Coordinates of the Center
        string text;
        int2 textsize;

        string test = "";

        NodeBrush currentBrush = NodeBrush.Free;
        Tuple<Rectangle, OutConnection> BrushItem = null;
        SimpleNodeWidget nodeBrush = null;

        List<SimpleNodeWidget> Nodes = new List<SimpleNodeWidget>();

        [ObjectCreator.UseCtor]
        public NodeEditorNodeScreenWidget(ScriptNodeWidget snw, NodeEditorBackgroundWidget bgw)
        {
            Snw = snw;
            this.bgw = bgw;
        }

        public override void Tick()
        {
            Bounds = new Rectangle(bgw.RenderBounds.X + 125, bgw.RenderBounds.Y + 5, bgw.RenderBounds.Width - 130, bgw.RenderBounds.Height - 10);
            CorrectCenterCoordinates = new int2(RenderBounds.Width / 2 + CenterCoordinates.X, RenderBounds.Height / 2 + CenterCoordinates.Y);
        }

        public void AddNode()
        {
            var newNode = new SimpleNodeWidget(this);
            AddChild(newNode);
            Nodes.Add(newNode);
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
                        if (connection.Item1.Contains(mi.Location) && currentBrush == NodeBrush.Connecting && BrushItem != null)
                        {
                            connection.Item2.In = BrushItem;
                        }
                    }
                }

                currentBrush = NodeBrush.Free;
                BrushItem = null;
                nodeBrush = null;
                oldCursorPosition = mi.Location;
                return false;
            }

            if (HandleNodes(mi))
                return false;

            if (ConnectNodes(mi))
                return false;

            if (mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down && currentBrush == NodeBrush.Free)
            {
                currentBrush = NodeBrush.Drag;
            }
            else if (mi.Button != MouseButton.Left)
                currentBrush = NodeBrush.Free;

            if (currentBrush == NodeBrush.Drag)
            {
                if (mi.Location != oldCursorPosition)
                {
                    CenterCoordinates += oldCursorPosition - mi.Location;
                    CorrectCenterCoordinates = new int2(RenderBounds.Width / 2 + CenterCoordinates.X, RenderBounds.Height / 2 + CenterCoordinates.Y);
                }
            }

            oldCursorPosition = mi.Location;

            return true;
        }

        public virtual Rectangle EventBounds
        {
            get { return RenderBounds; }
        }

        public bool HandleNodes(MouseInput mi)
        {
            foreach (var node in Nodes)
            {
                if (node.WidgetBackground.Contains(mi.Location) && mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down && currentBrush == NodeBrush.Free)
                {
                    if (node.DeleteButton.Contains(mi.Location))
                    {
                        DeleteNode(node);
                        return true;
                    }

                    if (node.WidgetEntries.Contains(mi.Location))
                    {
                        return true;
                    }

                    if (node.DragBar.Contains(mi.Location) && currentBrush == NodeBrush.Free)
                    {
                        node.CursorLocation = mi.Location;
                        currentBrush = NodeBrush.Node;
                        nodeBrush = node;
                        test = "test";
                    }
                }

                if (currentBrush == NodeBrush.Node && nodeBrush == node)
                {
                    node.NewOffset = node.CursorLocation - mi.Location;
                    node.OffsetPosX -= node.NewOffset.X;
                    node.OffsetPosY -= node.NewOffset.Y;
                    test = node.NewOffset.ToString();
                    node.CursorLocation = mi.Location;
                    return true;
                }
            }

            return false;
        }

        public bool ConnectNodes(MouseInput mi)
        {
            foreach (var node in Nodes)
            {
                foreach (var connection in node.OutConnections)
                {
                    if (connection.Item1.Contains(mi.Location) && mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down && currentBrush == NodeBrush.Free)
                    {
                        currentBrush = NodeBrush.Connecting;
                        BrushItem = connection;
                        return true;
                    }
                }

                foreach (var connection in node.InConnections)
                {
                    if (connection.Item1.Contains(mi.Location)
                        && mi.Button == MouseButton.Right
                        && mi.Event == MouseInputEvent.Down
                        && currentBrush == NodeBrush.Free
                        && connection.Item2.In != null)
                    {
                        connection.Item2.In = null;
                        return true;
                    }
                }
            }

            return false;
        }

        public override void MouseExited()
        {
            currentBrush = NodeBrush.Free;
        }

        public override void Draw()
        {
            text = "X: " + CenterCoordinates.X + " Y: " + CenterCoordinates.Y;
            textsize = Snw.FontRegular.Measure(text);

            WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6, RenderBounds.Height + 6), Color.Black);
            WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height), Color.DarkGray);

            Snw.FontRegular.DrawTextWithShadow(text, new float2(RenderBounds.X + 2, RenderBounds.Y + 2),
                Color.White, Color.Black, 1);

            Snw.FontRegular.DrawTextWithShadow(test, new float2(RenderBounds.X + 2, RenderBounds.Y + 25),
                Color.White, Color.Black, 1);

            Snw.FontRegular.DrawTextWithShadow(currentBrush.ToString(), new float2(RenderBounds.X + 2, RenderBounds.Y + 50),
                Color.White, Color.Black, 1);

            if (BrushItem != null && currentBrush == NodeBrush.Connecting)
            {
                Game.Renderer.RgbaColorRenderer.DrawLine(new int2(BrushItem.Item1.X + 5, BrushItem.Item1.Y + 5), oldCursorPosition,
                    2, BrushItem.Item2.color);
            }
        }
    }

    enum NodeBrush
    {
        Free,
        Connecting,
        Drag,
        Node
    }
}