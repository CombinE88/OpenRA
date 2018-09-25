using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes
{
    public class SimpleNodeWidget : Widget
    {
        protected NodeEditorNodeScreenWidget screen;

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
        public List<Tuple<Rectangle, InConnection>> InConnections = new List<Tuple<Rectangle, InConnection>>();
        public List<Tuple<Rectangle, OutConnection>> OutConnections = new List<Tuple<Rectangle, OutConnection>>();

        public Rectangle AddInput;
        public Rectangle AddOutput;
        public Rectangle RemoveInput;
        public Rectangle RemoveOutput;
        public Rectangle FreeWidgetEntries;
        public AdvancedTextFieldType Textfield;

        // Selection
        public bool Selected;

        readonly EditorViewportControllerWidget editor;

        /*
        public List<AdvancedTextFieldType> TextList = new List<AdvancedTextFieldType>();
        */

        [ObjectCreator.UseCtor]
        public SimpleNodeWidget(NodeEditorNodeScreenWidget screen)
        {
            editor = screen.Snw.Parent.Get<EditorViewportControllerWidget>("MAP_EDITOR");

            this.screen = screen;

            PosX = screen.CorrectCenterCoordinates.X;
            PosY = screen.CorrectCenterCoordinates.Y;

            SetOffsetPosX = screen.CenterCoordinates.X;
            SetOffsetPosY = screen.CenterCoordinates.Y;

            GridPosX = PosX - screen.CenterCoordinates.X + OffsetPosX;
            GridPosY = PosY - screen.CenterCoordinates.Y + OffsetPosY;

            Bounds = new Rectangle(GridPosX + OffsetPosX, GridPosY + OffsetPosY, 200 + SizeX, 150 + SizeY);

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

            GridPosX = PosX - screen.CenterCoordinates.X + OffsetPosX;
            GridPosY = PosY - screen.CenterCoordinates.Y + OffsetPosY;

            SizeY = Math.Max(InConnections.Count, OutConnections.Count) * 35;
            Bounds = new Rectangle(GridPosX, GridPosY, 200 + SizeX, 150 + SizeY);

            WidgetBackground = new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height);
            DragBar = new Rectangle(RenderBounds.X + 1, RenderBounds.Y + 1, RenderBounds.Width - 27, 25);
            DeleteButton = new Rectangle(RenderBounds.X + RenderBounds.Width - 26, RenderBounds.Y + 1, 25, 25);
            WidgetEntries = new Rectangle(RenderBounds.X + 1, RenderBounds.Y + 27, RenderBounds.Width - 2, RenderBounds.Height - 28);
            FreeWidgetEntries = new Rectangle(5, 27 + 26, RenderBounds.Width - 10, RenderBounds.Height - 28 - 26);

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
                InConnections[i] = new Tuple<Rectangle, InConnection>(rect, InConnections[i].Item2);
            }

            splitHeight = RenderBounds.Height / (OutConnections.Count + 1);
            for (int i = 0; i < OutConnections.Count; i++)
            {
                var rect = new Rectangle(RenderBounds.X + RenderBounds.Width - 10, RenderBounds.Y + splitHeight * (i + 1), 20, 20);
                OutConnections[i] = new Tuple<Rectangle, OutConnection>(rect, OutConnections[i].Item2);
            }

            foreach (var connection in InConnections)
            {
                if (connection.Item2.In != null)
                {
                    var widgetConnections = connection.Item2.In.Item2.Widget.OutConnections;
                    foreach (var outCons in widgetConnections)
                    {
                        if (connection.Item2.In != null && outCons.Item2 == connection.Item2.In.Item2 && outCons.Item2 == null)
                            connection.Item2.In = null;
                        else if (connection.Item2.In != null && outCons.Item2 == connection.Item2.In.Item2)
                            connection.Item2.In = outCons;
                    }
                }
            }
        }

        public override void Draw()
        {
            Game.Renderer.EnableScissor(screen.RenderBounds);

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
            screen.Snw.FontRegular.DrawTextWithShadow("+", new float2(AddOutput.X + 2, AddOutput.Y + 2),
                Color.White, Color.Black, 2);

            WidgetUtils.FillRectWithColor(new Rectangle(RemoveOutput.X - 1, RemoveOutput.Y - 1, RemoveOutput.Width + 1, RemoveOutput.Height + 1), Color.Black);
            WidgetUtils.FillRectWithColor(RemoveOutput, Color.DarkGray);
            screen.Snw.FontRegular.DrawTextWithShadow("-", new float2(RemoveOutput.X + 2, RemoveOutput.Y + 2),
                Color.White, Color.Black, 2);

            WidgetUtils.FillRectWithColor(new Rectangle(AddInput.X - 1, AddInput.Y - 1, AddInput.Width + 1, AddInput.Height + 1), Color.Black);
            WidgetUtils.FillRectWithColor(AddInput, Color.DarkGray);
            screen.Snw.FontRegular.DrawTextWithShadow("+", new float2(AddInput.X + 2, AddInput.Y + 2),
                Color.White, Color.Black, 2);

            WidgetUtils.FillRectWithColor(new Rectangle(RemoveInput.X - 1, RemoveInput.Y - 1, RemoveInput.Width + 1, RemoveInput.Height + 1), Color.Black);
            WidgetUtils.FillRectWithColor(RemoveInput, Color.DarkGray);
            screen.Snw.FontRegular.DrawTextWithShadow("-", new float2(RemoveInput.X + 2, RemoveInput.Y + 2),
                Color.White, Color.Black, 2);

            var text = "X: " + (OffsetPosX + SetOffsetPosX) + " Y: " + (OffsetPosY + SetOffsetPosY);
            screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(WidgetBackground.X + 2, WidgetBackground.Y + 2),
                Color.White, Color.Black, 1);

            foreach (var rectangle in InConnections)
            {
                WidgetUtils.FillEllipseWithColor(rectangle.Item1, rectangle.Item2.color);
            }

            foreach (var rectangle in OutConnections)
            {
                WidgetUtils.FillEllipseWithColor(rectangle.Item1, rectangle.Item2.color);
            }

            foreach (var connection in InConnections)
            {
                if (connection.Item2.In != null)
                    Game.Renderer.RgbaColorRenderer.DrawLine(
                        new int2(connection.Item1.X + 5, connection.Item1.Y + 5),
                        new int2(connection.Item2.In.Item1.X + 5, connection.Item2.In.Item1.Y + 5),
                        2, connection.Item2.color);
            }

            Game.Renderer.DisableScissor();
        }
    }

    public class InConnection
    {
        public ConnectionType conTyp = ConnectionType.Undefined;
        public readonly SimpleNodeWidget Widget;
        public Tuple<Rectangle, OutConnection> In;
        public Color color;
        public ConnectoItem Item;
        public readonly string ConnecitonName;

        public InConnection(ConnectionType conectionType, SimpleNodeWidget widget)
        {
            conTyp = conectionType;
            Widget = widget;

            switch (conectionType)
            {
                case ConnectionType.Actor:
                    color = Color.CornflowerBlue;
                    break;
                case ConnectionType.ActorInfo:
                    color = Color.Indigo;
                    break;
                case ConnectionType.Undefined:
                    color = Color.DarkSlateGray;
                    break;
                case ConnectionType.Boolean:
                    color = Color.DarkOliveGreen;
                    break;
            }

            ConnecitonName = "Input" + (widget.InConnections.Count + 1);
        }
    }

    public class OutConnection
    {
        public readonly ConnectionType conTyp = ConnectionType.Undefined;
        public readonly SimpleNodeWidget Widget;
        public Tuple<Rectangle, InConnection> Out;
        public Color color;
        public ConnectoItem Item;
        public readonly string ConnecitonName;

        public OutConnection(ConnectionType conectionType, SimpleNodeWidget widget)
        {
            conTyp = conectionType;
            Widget = widget;

            switch (conectionType)
            {
                case ConnectionType.Actor:
                    color = Color.CornflowerBlue;
                    break;
                case ConnectionType.ActorInfo:
                    color = Color.Indigo;
                    break;
                case ConnectionType.Undefined:
                    color = Color.DarkSlateGray;
                    break;
                case ConnectionType.Boolean:
                    color = Color.DarkOliveGreen;
                    break;
                case ConnectionType.Player:
                    color = Color.Brown;
                    break;
            }

            ConnecitonName = "Input" + (widget.InConnections.Count + 1);
        }
    }

    public class ConnectoItem
    {
        public bool Boolean = false;
        public Actor Actor = null;
        public ActorInfo ActorInfo = null;
        public PlayerReference Player = null;
        public CPos Location;
        public CPos[] CellArray;
        public int Number;
    }

    public enum ConnectionType
    {
        Undefined,
        Actor,
        Boolean,
        ActorInfo,
        Player,
        Location,
        Cell,
        CellArray,
        Integer
    }
}