using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes
{
    public class BasicNodeWidget : Widget
    {
        public readonly string Background = "dialog";

        public readonly string BackgroundCross = "button";

        // BAckground
        public readonly string BackgroundDrag = "button-highlighted";
        public readonly string BackgroundEntries = "button-pressed";
        public readonly EditorViewportControllerWidget Editor;

        public Rectangle AddInput;
        public Rectangle AddOutput;
        public int2 CursorLocation;
        public ButtonWidget DeleteButton;

        // Node Inhalte
        public Rectangle DragBar;
        public Rectangle FreeWidgetEntries;

        // Node Coordiantions in the System
        public int GridPosX;
        public int GridPosY;

        // Node Connections
        public List<InConnection> InConnections;
        public List<OutConnection> OutConnections;

        // Node Local Position
        public int2 NewOffset;
        public string NodeID;

        public TextFieldWidget NodeIDTextfield;
        public NodeInfo NodeInfo;

        public string NodeName = "General Widget";
        public NodeType NodeType;
        public int OffsetPosX;
        public int OffsetPosY;
        public Rectangle RemoveInput;
        public Rectangle RemoveOutput;

        public NodeEditorNodeScreenWidget Screen;

        // Node Informations

        // Selection
        public bool Selected;
        public int SizeX;
        public int SizeY;
        public AdvancedTextFieldType Textfield;
        public Rectangle WidgetBackground;
        public Rectangle WidgetEntries;

        [ObjectCreator.UseCtorAttribute]
        public BasicNodeWidget(NodeEditorNodeScreenWidget screen)
        {
            Editor = screen.NodeScriptContainerWidget.Parent.Get<EditorViewportControllerWidget>("MAP_EDITOR");
            Screen = screen;

            InConnections = new List<InConnection>();
            OutConnections = new List<OutConnection>();

            OffsetPosX = screen.CenterCoordinates.X;
            OffsetPosY = screen.CenterCoordinates.Y;

            SizeY = Math.Max(InConnections.Count, OutConnections.Count) * 35;
            Bounds = new Rectangle(GridPosX, GridPosY, 200 + SizeX, 150 + SizeY);

            WidgetBackground = new Rectangle(Bounds.X - 3, Bounds.Y - 3, Bounds.Width + 6, Bounds.Height + 6);
            DragBar = new Rectangle(Bounds.X + 1, Bounds.Y + 1, Bounds.Width - 27, 25);
            WidgetEntries = new Rectangle(Bounds.X + 1, Bounds.Y + 27, Bounds.Width - 2, Bounds.Height - 28);
            FreeWidgetEntries = new Rectangle(5, 30, WidgetEntries.Width - 10, WidgetEntries.Height - 28 - 26);

            AddChild(DeleteButton = new ButtonWidget(Screen.NodeScriptContainerWidget.ModData)
            {
                Text = "X",
                Bounds = new Rectangle(Bounds.X + Bounds.Width - 26, Bounds.Y + 1, 25, 25)
            });

            AddChild(NodeIDTextfield = new TextFieldWidget());
            NodeIDTextfield.OnTextEdited = () => { NodeName = NodeIDTextfield.Text; };
            NodeIDTextfield.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y, WidgetEntries.Width - 10, 20);
        }

        public
            void
            SetOuts
            (
                List
                    <
                        OutConnection
                    >
                    o
            )
        {
            OutConnections
                =
                o
                ;
        }

        public
            void
            SetIns
            (
                List
                    <
                        InConnection
                    >
                    i
            )
        {
            InConnections
                =
                i
                ;
        }

        public override void Tick()
        {
            GridPosX = Screen.WidgetScreenCenterCoordinates.X + OffsetPosX - Screen.CenterCoordinates.X -
                       Bounds.Width / 2;
            GridPosY = Screen.WidgetScreenCenterCoordinates.Y + OffsetPosY - Screen.CenterCoordinates.Y -
                       Bounds.Height / 2;

            SizeY = Math.Max(InConnections.Count, OutConnections.Count) * 35;
            Bounds = new Rectangle(GridPosX, GridPosY, 200 + SizeX, 150 + SizeY);

            WidgetBackground = new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6,
                RenderBounds.Height + 6);
            DragBar = new Rectangle(RenderBounds.X + 1, RenderBounds.Y + 1, RenderBounds.Width - 27, 25);
            WidgetEntries = new Rectangle(RenderBounds.X + 1, RenderBounds.Y + 27, RenderBounds.Width - 2,
                RenderBounds.Height - 28);
            FreeWidgetEntries = new Rectangle(5, 27, WidgetEntries.Width - 10, WidgetEntries.Height - 28 - 26);

            var splitHeight = RenderBounds.Height / (InConnections.Count + 1);
            for (var i = 0; i < InConnections.Count; i++)
            {
                var rect = new Rectangle(RenderBounds.X - 15, RenderBounds.Y + splitHeight * (i + 1), 20, 20);
                InConnections[i].InWidgetPosition = rect;
            }

            splitHeight = (RenderBounds.Height + 20) / (OutConnections.Count + 1);
            for (var i = 0; i < OutConnections.Count; i++)
            {
                var rect = new Rectangle(RenderBounds.X + RenderBounds.Width - 5,
                    RenderBounds.Y + splitHeight * (i + 1), 20, 20);
                OutConnections[i].InWidgetPosition = rect;
            }

            foreach (var connection in OutConnections)
                if (connection.Out != null && connection.Out.In == connection)
                    connection.Out.In = connection;
        }

        public override void DrawOuter()
        {
            Game.Renderer.EnableScissor(Screen.RenderBounds);

            base.DrawOuter();

            Game.Renderer.DisableScissor();
        }

        public override bool TakeMouseFocus(MouseInput mi)
        {
            return false;
        }

        public override bool HandleMouseInput(MouseInput mi)
        {
            return false;
        }

        public override bool YieldMouseFocus(MouseInput mi)
        {
            return false;
        }

        public override void MouseEntered()
        {
        }

        public override void MouseExited()
        {
        }

        public override void Draw()
        {
            for (var i = 0; i < InConnections.Count; i++)
                if (InConnections[i].In != null)
                {
                    var found = false;
                    var conin = InConnections[i].InWidgetPosition.Location;
                    var conout = conin;
                    foreach (var node in Screen.Nodes)
                    {
                        for (var j = 0; j < node.OutConnections.Count; j++)
                            if (node.OutConnections[j] == InConnections[i].In)
                            {
                                conout = node.OutConnections[j].InWidgetPosition.Location;
                                found = true;
                                break;
                            }

                        if (found)
                            break;
                    }

                    if (found)
                        NodeEditorNodeScreenWidget.DrawLine(new int2(conout.X + 10, conout.Y + 10),
                            new int2(conin.X + 10, conin.Y + 10), InConnections[i].Color);
                }

            if (!Screen.Bounds.Contains(WidgetBackground.X, WidgetBackground.Y)
                && !Screen.Bounds.Contains(WidgetBackground.X + WidgetBackground.Width, WidgetBackground.Y)
                && !Screen.Bounds.Contains(WidgetBackground.X, WidgetBackground.Y + WidgetBackground.Height)
                && !Screen.Bounds.Contains(WidgetBackground.X + WidgetBackground.Width,
                    WidgetBackground.Y + WidgetBackground.Height))
            {
                foreach (var child in Children.Where(child => child.Visible))
                    child.Visible = false;

                return;
            }

            foreach (var child in Children.Where(child => !child.Visible))
                child.Visible = true;

            //// Debug

            WidgetUtils.FillRectWithColor(
                new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height), Color.Brown);
            //// Outer

            if (Selected)
                WidgetUtils.FillRectWithColor(
                    new Rectangle(WidgetBackground.X - 1, WidgetBackground.Y - 1, WidgetBackground.Width + 2,
                        WidgetBackground.Height + 2), Color.Blue);

            WidgetUtils.DrawPanel(Background, WidgetBackground);

            WidgetUtils.DrawPanel(BackgroundDrag, DragBar);
            WidgetUtils.DrawPanel(BackgroundEntries, WidgetEntries);

            //// InConnection Buttons

            WidgetUtils.FillRectWithColor(AddOutput, Color.DarkGray);
            Screen.NodeScriptContainerWidget.FontRegular.DrawTextWithShadow("+",
                new float2(AddOutput.X + 2, AddOutput.Y + 2),
                Color.White, Color.Black, 2);

            WidgetUtils.FillRectWithColor(RemoveOutput, Color.DarkGray);
            Screen.NodeScriptContainerWidget.FontRegular.DrawTextWithShadow("-",
                new float2(RemoveOutput.X + 2, RemoveOutput.Y + 2),
                Color.White, Color.Black, 2);

            WidgetUtils.FillRectWithColor(AddInput, Color.DarkGray);
            Screen.NodeScriptContainerWidget.FontRegular.DrawTextWithShadow("+",
                new float2(AddInput.X + 2, AddInput.Y + 2),
                Color.White, Color.Black, 2);

            WidgetUtils.FillRectWithColor(RemoveInput, Color.DarkGray);
            Screen.NodeScriptContainerWidget.FontRegular.DrawTextWithShadow("-",
                new float2(RemoveInput.X + 2, RemoveInput.Y + 2),
                Color.White, Color.Black, 2);

            var text = "X: " + OffsetPosX + " Y: " + OffsetPosY;
            Screen.NodeScriptContainerWidget.FontRegular.DrawTextWithShadow(text,
                new float2(
                    WidgetBackground.X + WidgetBackground.Width -
                    Screen.NodeScriptContainerWidget.FontRegular.Measure(text).X - 10,
                    WidgetBackground.Y + WidgetBackground.Height - 25),
                Color.White, Color.Black, 1);

            Screen.NodeScriptContainerWidget.FontRegular.DrawTextWithShadow(NodeName + " " + NodeID,
                new float2(DragBar.X, DragBar.Y - 2),
                Color.White, Color.Black, 1);
            Screen.NodeScriptContainerWidget.FontSmall.DrawTextWithShadow(NodeType.ToString(),
                new float2(DragBar.X + 2, DragBar.Y + 12),
                Color.White, Color.Black, 1);

            for (var i = 0; i < InConnections.Count; i++)
            {
                if (InConnections[i].ConnectionTyp == ConnectionType.Exec)
                {
                    WidgetUtils.FillRectWithColor(
                        new Rectangle(InConnections[i].InWidgetPosition.X - 1, InConnections[i].InWidgetPosition.Y - 1,
                            InConnections[i].InWidgetPosition.Width + 2,
                            InConnections[i].InWidgetPosition.Width + 2), Color.Black);
                    WidgetUtils.FillRectWithColor(InConnections[i].InWidgetPosition, InConnections[i].Color);
                    WidgetUtils.FillRectWithColor(
                        new Rectangle(InConnections[i].InWidgetPosition.X + 2, InConnections[i].InWidgetPosition.Y + 2,
                            InConnections[i].InWidgetPosition.Width - 4,
                            InConnections[i].InWidgetPosition.Width - 4), Color.Black);
                }
                else
                {
                    WidgetUtils.FillEllipseWithColor(
                        new Rectangle(InConnections[i].InWidgetPosition.X - 1, InConnections[i].InWidgetPosition.Y - 1,
                            InConnections[i].InWidgetPosition.Width + 2,
                            InConnections[i].InWidgetPosition.Width + 2), Color.Black);
                    WidgetUtils.FillEllipseWithColor(InConnections[i].InWidgetPosition, InConnections[i].Color);
                    WidgetUtils.FillEllipseWithColor(
                        new Rectangle(InConnections[i].InWidgetPosition.X + 2, InConnections[i].InWidgetPosition.Y + 2,
                            InConnections[i].InWidgetPosition.Width - 4,
                            InConnections[i].InWidgetPosition.Width - 4), Color.Black);
                }

                Screen.NodeScriptContainerWidget.FontSmall.DrawTextWithShadow(InConnections[i].ConnectionTyp.ToString(),
                    new int2(InConnections[i].InWidgetPosition.X + 22, InConnections[i].InWidgetPosition.Y + 4),
                    Color.White, Color.Black, 1);
            }

            for (var i = 0; i < OutConnections.Count; i++)
            {
                if (OutConnections[i].ConnectionTyp == ConnectionType.Exec)
                {
                    WidgetUtils.FillRectWithColor(
                        new Rectangle(OutConnections[i].InWidgetPosition.X - 1,
                            OutConnections[i].InWidgetPosition.Y - 1, OutConnections[i].InWidgetPosition.Width + 2,
                            OutConnections[i].InWidgetPosition.Width + 2), Color.Black);
                    WidgetUtils.FillRectWithColor(OutConnections[i].InWidgetPosition, OutConnections[i].Color);
                    WidgetUtils.FillRectWithColor(
                        new Rectangle(OutConnections[i].InWidgetPosition.X + 2,
                            OutConnections[i].InWidgetPosition.Y + 2, OutConnections[i].InWidgetPosition.Width - 4,
                            OutConnections[i].InWidgetPosition.Width - 4), Color.Black);
                }
                else
                {
                    WidgetUtils.FillEllipseWithColor(
                        new Rectangle(OutConnections[i].InWidgetPosition.X - 1,
                            OutConnections[i].InWidgetPosition.Y - 1, OutConnections[i].InWidgetPosition.Width + 2,
                            OutConnections[i].InWidgetPosition.Width + 2), Color.Black);
                    WidgetUtils.FillEllipseWithColor(OutConnections[i].InWidgetPosition, OutConnections[i].Color);
                    WidgetUtils.FillEllipseWithColor(
                        new Rectangle(OutConnections[i].InWidgetPosition.X + 2,
                            OutConnections[i].InWidgetPosition.Y + 2, OutConnections[i].InWidgetPosition.Width - 4,
                            OutConnections[i].InWidgetPosition.Width - 4), Color.Black);
                }

                if (Screen.CurrentBrush == NodeBrush.Connecting)
                    Screen.NodeScriptContainerWidget.FontSmall.DrawTextWithShadow(
                        OutConnections[i].ConnectionTyp.ToString(),
                        new int2(OutConnections[i].InWidgetPosition.X + 22, OutConnections[i].InWidgetPosition.Y + 4),
                        Color.White, Color.Black, 1);
            }

            /* WidgetUtils.FillRectWithColor(new Rectangle(WidgetBackground.X, WidgetBackground.Y, 2, 2), Color.Blue);
            WidgetUtils.FillRectWithColor(new Rectangle(WidgetBackground.X + WidgetBackground.Width, WidgetBackground.Y, 2, 2), Color.Blue);
            WidgetUtils.FillRectWithColor(new Rectangle(WidgetBackground.X, WidgetBackground.Y + WidgetBackground.Height, 2, 2), Color.Blue);
            WidgetUtils.FillRectWithColor(new Rectangle(WidgetBackground.X + WidgetBackground.Width, WidgetBackground.Y + WidgetBackground.Height, 2, 2), Color.Blue);
            */
        }

        public override Widget Clone()
        {
            return new BasicNodeWidget(Screen);
        }
    }
}