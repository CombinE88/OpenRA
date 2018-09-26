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
        public List<Rectangle> InConnectionsR = new List<Rectangle>();
        public List<OutConnection> OutConnections = new List<OutConnection>();
        public List<Rectangle> OutConnectionsR = new List<Rectangle>();

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

            WidgetBackground = new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height);
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
                var rect = new Rectangle(RenderBounds.X - 10, RenderBounds.Y + splitHeight * (i + 1), 20, 20);
                InConnectionsR[i] = rect;
            }

            splitHeight = RenderBounds.Height / (OutConnections.Count + 1);
            for (int i = 0; i < OutConnections.Count; i++)
            {
                var rect = new Rectangle(RenderBounds.X + RenderBounds.Width - 10, RenderBounds.Y + splitHeight * (i + 1), 20, 20);
                OutConnectionsR[i] = rect;
            }

            for (int i = 0; i < InConnections.Count; i++)
            {
                if (InConnections[i].In != null)
                {
                    var widgetConnections = InConnections[i].In.Out.Widget.OutConnections;
                    foreach (var outCons in widgetConnections)
                    {
                        if (InConnections[i].In != null && outCons.Out == InConnections[i].In.Out && outCons.Out == null)
                            InConnections[i].In = null;
                        else if (InConnections[i].In != null && outCons.Out == InConnections[i].In.Out)
                            InConnections[i].In = outCons;
                    }
                }
            }
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

        public virtual void DrawExtra()
        {
        }

        public override void Draw()
        {
            Game.Renderer.EnableScissor(Screen.RenderBounds);

            DrawExtra();

            // Debug
            WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height), Color.Brown);
            // Outer
            if (Selected)
                WidgetUtils.FillRectWithColor(new Rectangle(WidgetBackground.X - 1, WidgetBackground.Y - 1, WidgetBackground.Width + 2, WidgetBackground.Height + 2), Color.Blue);

            WidgetUtils.FillRectWithColor(WidgetBackground, Color.Black);
            // Drag Leiste
            WidgetUtils.FillRectWithColor(DragBar, Color.DarkGray);
            WidgetUtils.FillRectWithColor(DeleteButton, Color.DarkRed);
            WidgetUtils.FillRectWithColor(WidgetEntries, Color.DarkGray);

            //InconnectioNButtons
            WidgetUtils.FillRectWithColor(new Rectangle(AddOutput.X - 1, AddOutput.Y - 1, AddOutput.Width + 1, AddOutput.Height + 1), Color.Black);
            WidgetUtils.FillRectWithColor(AddOutput, Color.DarkGray);
            Screen.Snw.FontRegular.DrawTextWithShadow("+", new float2(AddOutput.X + 2, AddOutput.Y + 2),
                Color.White, Color.Black, 2);

            WidgetUtils.FillRectWithColor(new Rectangle(RemoveOutput.X - 1, RemoveOutput.Y - 1, RemoveOutput.Width + 1, RemoveOutput.Height + 1), Color.Black);
            WidgetUtils.FillRectWithColor(RemoveOutput, Color.DarkGray);
            Screen.Snw.FontRegular.DrawTextWithShadow("-", new float2(RemoveOutput.X + 2, RemoveOutput.Y + 2),
                Color.White, Color.Black, 2);

            WidgetUtils.FillRectWithColor(new Rectangle(AddInput.X - 1, AddInput.Y - 1, AddInput.Width + 1, AddInput.Height + 1), Color.Black);
            WidgetUtils.FillRectWithColor(AddInput, Color.DarkGray);
            Screen.Snw.FontRegular.DrawTextWithShadow("+", new float2(AddInput.X + 2, AddInput.Y + 2),
                Color.White, Color.Black, 2);

            WidgetUtils.FillRectWithColor(new Rectangle(RemoveInput.X - 1, RemoveInput.Y - 1, RemoveInput.Width + 1, RemoveInput.Height + 1), Color.Black);
            WidgetUtils.FillRectWithColor(RemoveInput, Color.DarkGray);
            Screen.Snw.FontRegular.DrawTextWithShadow("-", new float2(RemoveInput.X + 2, RemoveInput.Y + 2),
                Color.White, Color.Black, 2);

            var text = "X: " + (OffsetPosX + SetOffsetPosX) + " Y: " + (OffsetPosY + SetOffsetPosY);
            Screen.Snw.FontRegular.DrawTextWithShadow(text,
                new float2(WidgetBackground.X + WidgetBackground.Width - Screen.Snw.FontRegular.Measure(text).X + 2, WidgetBackground.Y + WidgetBackground.Height - 25),
                Color.White, Color.Black, 1);

            Screen.Snw.FontRegular.DrawTextWithShadow(WidgetName,
                new float2(DragBar.X + 2, DragBar.Y + 2),
                Color.White, Color.Black, 1);

            for (int i = 0; i < InConnections.Count; i++)
            {
                WidgetUtils.FillEllipseWithColor(InConnectionsR[i], InConnections[i].color);
            }

            for (int i = 0; i < OutConnections.Count; i++)
            {
                WidgetUtils.FillEllipseWithColor(OutConnectionsR[i], OutConnections[i].color);
            }

            for (int i = 0; i < InConnections.Count; i++)
            {
                if (InConnections[i].In != null)
                {
                    var found = false;
                    Point conin = InConnectionsR[i].Location;
                    Point conout = conin;
                    foreach (var node in Screen.Nodes)
                    {
                        for (int j = 0; j < node.OutConnections.Count; j++)
                        {
                            if (node.OutConnections[j] == InConnections[i].In)
                            {
                                conout = node.OutConnectionsR[j].Location;
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
                            new int2(conout.X + 5, conout.Y + 5),
                            new int2(conin.X + 5, conin.Y + 5),
                            2, InConnections[i].color);
                    }
                }
            }


            Game.Renderer.DisableScissor();
        }
    }
}