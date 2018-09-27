using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes
{
    public class SimpleNodeWidget : Widget
    {
        public NodeEditorNodeScreenWidget Screen;

        // BAckground
        public readonly string BackgroundDrag = "button-highlighted";
        public readonly string BackgroundCross = "button";
        public readonly string BackgroundEntries = "button-pressed";

        public readonly string Background = "dialog";

        // Node Coordiantions in the System
        public int GridPosX;
        public int GridPosY;
        public int PosX;
        public int PosY;
        public int SizeX;
        public int SizeY;
        public int OffsetPosX;
        public int OffsetPosY;
        public int SetOffsetPosX;
        public int SetOffsetPosY;

        // Node Inhalte
        public Rectangle DragBar;
        public Rectangle DeleteButton;
        public Rectangle WidgetEntries;
        public Rectangle WidgetBackground;

        // Node Local Position
        public int2 NewOffset;
        public int2 CursorLocation;

        // Node Connections
        public List<InConnection> InConnections = new List<InConnection>();
        public List<OutConnection> OutConnections = new List<OutConnection>();

        public Rectangle AddInput;
        public Rectangle AddOutput;
        public Rectangle RemoveInput;
        public Rectangle RemoveOutput;
        public Rectangle FreeWidgetEntries;
        public AdvancedTextFieldType Textfield;

        public List<CPos> SelectedCells = new List<CPos>();
        public List<Actor> SelectedActor = new List<Actor>();

        // Selection
        public bool Selected;

        public readonly EditorViewportControllerWidget Editor;

        public string WidgetName = "General Widget";

        /*
        public List<AdvancedTextFieldType> TextList = new List<AdvancedTextFieldType>();
        */

        [ObjectCreator.UseCtor]
        public SimpleNodeWidget(NodeEditorNodeScreenWidget screen)
        {
            Editor = screen.Snw.Parent.Get<EditorViewportControllerWidget>("MAP_EDITOR");

            this.Screen = screen;

            PosX = screen.CorrectCenterCoordinates.X;
            PosY = screen.CorrectCenterCoordinates.Y;

            SetOffsetPosX = screen.CenterCoordinates.X;
            SetOffsetPosY = screen.CenterCoordinates.Y;

            GridPosX = PosX - screen.CenterCoordinates.X + OffsetPosX;
            GridPosY = PosY - screen.CenterCoordinates.Y + OffsetPosY;

            Bounds = new Rectangle(GridPosX + OffsetPosX, GridPosY + OffsetPosY, 200 + SizeX, 150 + SizeY);
            // Bounds = new Rectangle(0, 0, 0, 0);

            /*
            Textfield = new AdvancedTextFieldType();
            AddChild(Textfield);
            */
        }

        /*
        public void AddInConnection()
        {
            var inRecangle = new Rectangle(RenderBounds.X + GridPosX + RenderBounds.Width - 10, RenderBounds.Y + GridPosY + RenderBounds.Height / 2, 20, 20);
            var inconnection = new InConnection(ConnectionType.ActorInfo, this);
            InConnections.Add(new Tuple<Rectangle, InConnection>(inRecangle, inconnection));
        }

        public void RemoveInConnection()
        {
            if (InConnections.LastOrDefault() != null)
                InConnections.Remove(InConnections.Last());
        }

        public void RemoveOutConnection()
        {
            if (OutConnections.LastOrDefault() != null)
                OutConnections.Remove(OutConnections.Last());
        }

        public void AddOutConnection()
        {
            var outRecangle = new Rectangle(RenderBounds.X + GridPosX - 10, RenderBounds.Y + GridPosY + RenderBounds.Height / 2, 20, 20);
            var outConnection = new OutConnection(ConnectionType.ActorInfo, this);
            OutConnections.Add(new Tuple<Rectangle, OutConnection>(outRecangle, outConnection));
        }
        */

        public override void Tick()
        {
            // Visible = false;
            // if (screen.RenderBounds.Contains(new int2(GridPosX, GridPosY)))
            //    Visible = true;

            GridPosX = PosX - Screen.CenterCoordinates.X + OffsetPosX;
            GridPosY = PosY - Screen.CenterCoordinates.Y + OffsetPosY;

            SizeY = Math.Max(InConnections.Count, OutConnections.Count) * 35;
            Bounds = new Rectangle(GridPosX, GridPosY, 200 + SizeX, 150 + SizeY);
            // Bounds = new Rectangle(0, 0, 0, 0);

            WidgetBackground = new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6, RenderBounds.Height + 6);
            DragBar = new Rectangle(RenderBounds.X + 1, RenderBounds.Y + 1, RenderBounds.Width - 27, 25);
            DeleteButton = new Rectangle(RenderBounds.X + RenderBounds.Width - 26, RenderBounds.Y + 1, 25, 25);
            WidgetEntries = new Rectangle(RenderBounds.X + 1, RenderBounds.Y + 27, RenderBounds.Width - 2, RenderBounds.Height - 28);
            FreeWidgetEntries = new Rectangle(5, 27 + 26, WidgetEntries.Width - 10, WidgetEntries.Height - 28 - 26);

            /*
            var n = 0;
            foreach (var textfield in TextList)
            {
                textfield.Bounds = new Rectangle(5, 27 + 26 + 25 * n, RenderBounds.Width - 10, 25);
                n += 1;
            }

            AddInput = new Rectangle(WidgetEntries.X + 5, WidgetEntries.Y + 5, 25, 25);
            RemoveInput = new Rectangle(WidgetEntries.X + 31, WidgetEntries.Y + 5, 25, 25);
            AddOutput = new Rectangle(WidgetEntries.X + WidgetEntries.Width - 30, WidgetEntries.Y + 5, 25, 25);
            RemoveOutput = new Rectangle(WidgetEntries.X + WidgetEntries.Width - 61, WidgetEntries.Y + 5, 25, 25);
            */
            // handle Out- and Inputs

            var splitHeight = RenderBounds.Height / (InConnections.Count + 1);
            for (int i = 0; i < InConnections.Count; i++)
            {
                var rect = new Rectangle(RenderBounds.X - 15, RenderBounds.Y + splitHeight * (i + 1), 20, 20);
                InConnections[i].InWidgetPosition = rect;
            }

            splitHeight = RenderBounds.Height / (OutConnections.Count + 1);
            for (int i = 0; i < OutConnections.Count; i++)
            {
                var rect = new Rectangle(RenderBounds.X + RenderBounds.Width - 5, RenderBounds.Y + splitHeight * (i + 1), 20, 20);
                OutConnections[i].InWidgetPosition = rect;
            }

            foreach (var connection in OutConnections)
            {
                if (connection.Out != null && connection.Out.In == connection)
                    connection.Out.In = connection;
            }
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
            // Debug
            WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height), Color.Brown);
            // Outer
            if (Selected)
                WidgetUtils.FillRectWithColor(new Rectangle(WidgetBackground.X - 1, WidgetBackground.Y - 1, WidgetBackground.Width + 2, WidgetBackground.Height + 2), Color.Blue);

            WidgetUtils.DrawPanel(Background, WidgetBackground);
            // WidgetUtils.FillRectWithColor(WidgetBackground, Color.Black);
            // Drag Leiste
            // WidgetUtils.FillRectWithColor(DragBar, Color.DarkGray);
            // WidgetUtils.FillRectWithColor(DeleteButton, Color.DarkRed);
            // WidgetUtils.FillRectWithColor(WidgetEntries, Color.DarkGray);

            WidgetUtils.DrawPanel(BackgroundDrag, DragBar);
            WidgetUtils.DrawPanel(BackgroundCross, DeleteButton);
            WidgetUtils.DrawPanel(BackgroundEntries, WidgetEntries);

            //InconnectioNButtons
            WidgetUtils.FillRectWithColor(AddOutput, Color.DarkGray);
            Screen.Snw.FontRegular.DrawTextWithShadow("+", new float2(AddOutput.X + 2, AddOutput.Y + 2),
                Color.White, Color.Black, 2);

            WidgetUtils.FillRectWithColor(RemoveOutput, Color.DarkGray);
            Screen.Snw.FontRegular.DrawTextWithShadow("-", new float2(RemoveOutput.X + 2, RemoveOutput.Y + 2),
                Color.White, Color.Black, 2);

            WidgetUtils.FillRectWithColor(AddInput, Color.DarkGray);
            Screen.Snw.FontRegular.DrawTextWithShadow("+", new float2(AddInput.X + 2, AddInput.Y + 2),
                Color.White, Color.Black, 2);

            WidgetUtils.FillRectWithColor(RemoveInput, Color.DarkGray);
            Screen.Snw.FontRegular.DrawTextWithShadow("-", new float2(RemoveInput.X + 2, RemoveInput.Y + 2),
                Color.White, Color.Black, 2);

            var text = "X: " + (OffsetPosX + SetOffsetPosX) + " Y: " + (OffsetPosY + SetOffsetPosY);
            Screen.Snw.FontRegular.DrawTextWithShadow(text,
                new float2(WidgetBackground.X + WidgetBackground.Width - Screen.Snw.FontRegular.Measure(text).X - 10, WidgetBackground.Y + WidgetBackground.Height - 25),
                Color.White, Color.Black, 1);

            Screen.Snw.FontRegular.DrawTextWithShadow(WidgetName,
                new float2(DragBar.X + 2, DragBar.Y + 2),
                Color.White, Color.Black, 1);

            for (int i = 0; i < InConnections.Count; i++)
            {
                WidgetUtils.FillEllipseWithColor(
                    new Rectangle(InConnections[i].InWidgetPosition.X - 1, InConnections[i].InWidgetPosition.Y - 1, InConnections[i].InWidgetPosition.Width + 2,
                        InConnections[i].InWidgetPosition.Width + 2), Color.Black);
                WidgetUtils.FillEllipseWithColor(InConnections[i].InWidgetPosition, InConnections[i].color);
                WidgetUtils.FillEllipseWithColor(
                    new Rectangle(InConnections[i].InWidgetPosition.X + 2, InConnections[i].InWidgetPosition.Y + 2, InConnections[i].InWidgetPosition.Width - 4,
                        InConnections[i].InWidgetPosition.Width - 4), Color.Black);
            }

            for (int i = 0; i < OutConnections.Count; i++)
            {
                WidgetUtils.FillEllipseWithColor(
                    new Rectangle(OutConnections[i].InWidgetPosition.X - 1, OutConnections[i].InWidgetPosition.Y - 1, OutConnections[i].InWidgetPosition.Width + 2,
                        OutConnections[i].InWidgetPosition.Width + 2), Color.Black);
                WidgetUtils.FillEllipseWithColor(OutConnections[i].InWidgetPosition, OutConnections[i].color);
                WidgetUtils.FillEllipseWithColor(
                    new Rectangle(OutConnections[i].InWidgetPosition.X + 2, OutConnections[i].InWidgetPosition.Y + 2, OutConnections[i].InWidgetPosition.Width - 4,
                        OutConnections[i].InWidgetPosition.Width - 4), Color.Black);
            }

            for (int i = 0; i < InConnections.Count; i++)
            {
                if (InConnections[i].In != null)
                {
                    var found = false;
                    Point conin = InConnections[i].InWidgetPosition.Location;
                    Point conout = conin;
                    foreach (var node in Screen.Nodes)
                    {
                        for (int j = 0; j < node.OutConnections.Count; j++)
                        {
                            if (node.OutConnections[j] == InConnections[i].In)
                            {
                                conout = node.OutConnections[j].InWidgetPosition.Location;
                                found = true;
                                break;
                            }
                        }

                        if (found)
                            break;
                    }

                    if (found)
                    {
                        Game.Renderer.RgbaColorRenderer.DrawLine(
                            new int2(conout.X + 10, conout.Y + 10),
                            new int2(conin.X + 10, conin.Y + 10),
                            2, InConnections[i].color);
                    }
                }
            }
        }
    }
}