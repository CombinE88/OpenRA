using System;
using System.Collections.Generic;
using System.Drawing;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{


    public class InConnection
    {
        public readonly SimpleNodeWidget Widget;
        public readonly string ConnecitonName;

        public Rectangle InWidgetPosition;

        public ConnectionType conTyp = ConnectionType.Undefined;
        public OutConnection In;
        public Color color;

        public bool Boolean = false;
        public Actor Actor = null;
        public ActorInfo ActorInfo = null;
        public PlayerReference Player = null;
        public CPos Location = CPos.Zero;
        public List<CPos> CellArray = new List<CPos>();
        public int Number = 0;

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
                case ConnectionType.Player:
                    color = Color.Brown;
                    break;
                case ConnectionType.Location:
                    color = Color.BlueViolet;
                    break;
                case ConnectionType.CellArray:
                    color = Color.Violet;
                    break;
                case ConnectionType.Integer:
                    color = Color.Green;
                    break;
            }

            ConnecitonName = "Input" + (widget.InConnections.Count + 1);
        }
    }

    public class OutConnection
    {
        public readonly string ConnecitonName;
        public readonly ConnectionType conTyp = ConnectionType.Undefined;
        public readonly SimpleNodeWidget Widget;

        public Rectangle InWidgetPosition;

        public InConnection Out;
        public Color color;

        public bool Boolean = false;
        public Actor Actor = null;
        public ActorInfo ActorInfo = null;
        public PlayerReference Player = null;
        public CPos Location = CPos.Zero;
        public List<CPos> CellArray = new List<CPos>();
        public int Number = 0;

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
                case ConnectionType.Location:
                    color = Color.BlueViolet;
                    break;
                case ConnectionType.CellArray:
                    color = Color.Violet;
                    break;
                case ConnectionType.Integer:
                    color = Color.Green;
                    break;
            }

            ConnecitonName = "Input" + (widget.InConnections.Count + 1);
        }
    }

    public enum ConnectionType
    {
        Undefined,
        Actor,
        Boolean,
        ActorInfo,
        Player,
        Location,
        CellArray,
        Integer
    }
}