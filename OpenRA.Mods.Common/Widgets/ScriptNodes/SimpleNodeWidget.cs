using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using OpenRA.Primitives;
using OpenRA.Server;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class SimpleNodeWidget : Widget
    {
        NodeEditorNodeScreenWidget screen;

        // Node Coordiantions in the System
        public int GridPosX;
        public int GridPosY;
        public int PosX;
        public int PosY;
        public int SizeX = 200;
        public int SizeY = 150;
        public int OffsetPosX;
        public int OffsetPosY;

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

        [ObjectCreator.UseCtor]
        public SimpleNodeWidget(NodeEditorNodeScreenWidget screen)
        {
            this.screen = screen;

            PosX = screen.CorrectCenterCoordinates.X;
            PosY = screen.CorrectCenterCoordinates.Y;


            GridPosX = PosX - screen.CorrectCenterCoordinates.X + OffsetPosX;
            GridPosY = PosY - screen.CorrectCenterCoordinates.Y + OffsetPosY;

            Bounds = new Rectangle(GridPosX + OffsetPosX, GridPosY + OffsetPosY, SizeX, SizeY);

            AddBasicNodePoints();
        }

        public virtual void AddBasicNodePoints()
        {
            var inRecangle = new Rectangle(RenderBounds.X + GridPosX + RenderBounds.Width - 10, RenderBounds.Y + GridPosY + RenderBounds.Height / 2, 20, 20);
            var inconnection = new InConnection(ConnectionType.ActorInfo, this);
            InConnections.Add(new Tuple<Rectangle, InConnection>(inRecangle, inconnection));

            var outRecangle = new Rectangle(RenderBounds.X + GridPosX - 10, RenderBounds.Y + GridPosY + RenderBounds.Height / 2, 20, 20);
            var outConnection = new OutConnection(ConnectionType.ActorInfo, this);
            OutConnections.Add(new Tuple<Rectangle, OutConnection>(outRecangle, outConnection));
        }

        public override void Tick()
        {
            // Visible = false;
            // if (screen.RenderBounds.Contains(new int2(GridPosX, GridPosY)))
            //    Visible = true;

            GridPosX = PosX - screen.CorrectCenterCoordinates.X + OffsetPosX;
            GridPosY = PosY - screen.CorrectCenterCoordinates.Y + OffsetPosY;

            Bounds = new Rectangle(GridPosX, GridPosY, SizeX, SizeY);

            WidgetBackground = new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width + 2, RenderBounds.Height + 2);
            DragBar = new Rectangle(RenderBounds.X + 1, RenderBounds.Y + 1, SizeX - 27, 25);
            DeleteButton = new Rectangle(RenderBounds.X + SizeX - 26, RenderBounds.Y + 2, 25, 25);
            WidgetEntries = new Rectangle(RenderBounds.X + 1, RenderBounds.Y + 27, SizeX, SizeY - 27);


            // handle Out- and Inputs

            var splitHeight = RenderBounds.Height / (InConnections.Count + 1);
            for (int i = 0; i < InConnections.Count; i++)
            {
                var rect = new Rectangle(RenderBounds.X - 10, RenderBounds.Y + splitHeight * (i + 1), 20, 20);
                InConnections[i] = new Tuple<Rectangle, InConnection>(rect, InConnections[i].Item2);
            }

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
                        if (outCons.Item2 == connection.Item2.In.Item2)
                            connection.Item2.In = outCons;
                    }
                }
            }

            Connection();
        }

        public virtual void Connection()
        {
            if (InConnections.First().Item2.conTyp == OutConnections.First().Item2.conTyp)
                OutConnections.First().Item2.Item = InConnections.First().Item2.Item;
        }

        public override void Draw()
        {
            // Debug
            WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height), Color.Brown);
            // Outer
            WidgetUtils.FillRectWithColor(WidgetBackground, Color.Black);
            // Drag Leiste
            WidgetUtils.FillRectWithColor(DragBar, Color.DarkGray);
            WidgetUtils.FillRectWithColor(DeleteButton, Color.DarkRed);
            WidgetUtils.FillRectWithColor(WidgetEntries, Color.DarkGray);

            var text = "X: " + OffsetPosX + " Y: " + OffsetPosY;
            screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(WidgetBackground.X + 2, WidgetBackground.Y + 2),
                Color.White, Color.Black, 1);

            foreach (var rectangle in InConnections)
            {
                WidgetUtils.FillEllipseWithColor(rectangle.Item1, rectangle.Item2.color);
            }

            foreach (var rectangle in OutConnections)
            {
                WidgetUtils.FillEllipseWithColor(rectangle.Item1, rectangle.Item2.color);
                ;
            }

            DrawInconnection();

            foreach (var connection in InConnections)
            {
                if (connection.Item2.In != null)
                    Game.Renderer.RgbaColorRenderer.DrawLine(
                        new int2(connection.Item1.X + 5, connection.Item1.Y + 5),
                        new int2(connection.Item2.In.Item1.X + 5, connection.Item2.In.Item1.Y + 5),
                        2, connection.Item2.color);
            }
        }

        public void DrawInconnection()
        {
        }
    }

    public class InConnection
    {
        public readonly ConnectionType conTyp = ConnectionType.Undefined;
        public readonly SimpleNodeWidget Widget;
        public Tuple<Rectangle, OutConnection> In;
        public Color color;
        public ConnectoItem Item;

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
        }
    }

    public class OutConnection
    {
        public readonly ConnectionType conTyp = ConnectionType.Undefined;
        public readonly SimpleNodeWidget Widget;
        public Tuple<Rectangle, InConnection> Out;
        public Color color;
        public ConnectoItem Item;

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
            }
        }
    }

    public class ConnectoItem
    {
        public bool Boolean = false;
        public Actor Actor = null;
        public ActorInfo ActorInfo = null;
    }

    public enum ConnectionType
    {
        Undefined,
        Actor,
        Boolean,
        ActorInfo
    }
}