using System;
using System.Collections.Generic;
using System.Drawing;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class InConnection
    {
        public readonly BasicNodeWidget Widget;
        public readonly string ConnecitonName;

        public Rectangle InWidgetPosition;

        public ConnectionType conTyp = ConnectionType.Undefined;
        public OutConnection In;
        public Color color;

        public bool Boolean = false;
        public ActorInfo ActorInfo = null;
        public PlayerReference Player = null;
        public CPos Location = CPos.Zero;
        public List<CPos> CellArray = new List<CPos>();
        public int Number = 0;
        public string String = "";
        public string[] Strings = { "" };

        public InConnection(ConnectionType conectionType, BasicNodeWidget widget)
        {
            conTyp = conectionType;
            Widget = widget;

            switch (conectionType)
            {
                case ConnectionType.Undefined:
                    color = Color.Black;
                    break;
                case ConnectionType.Actor:
                    color = Color.CornflowerBlue;
                    break;
                case ConnectionType.ActorList:
                    color = Color.DarkBlue;
                    break;
                case ConnectionType.ActorInfo:
                    color = Color.Indigo;
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
                case ConnectionType.LocationRange:
                    color = Color.MediumVioletRed;
                    break;
                case ConnectionType.CellArray:
                    color = Color.Violet;
                    break;
                case ConnectionType.CellPath:
                    color = Color.Violet;
                    break;
                case ConnectionType.Integer:
                    color = Color.Green;
                    break;
                case ConnectionType.Universal:
                    color = Color.White;
                    break;
                case ConnectionType.String:
                    color = Color.SlateGray;
                    break;
                case ConnectionType.Strings:
                    color = Color.DarkSlateGray;
                    break;
            }

            ConnecitonName = "Input" + (widget.InConnections.Count + 1);
        }
    }

    public class OutConnection
    {
        public readonly string ConnecitonName;
        public readonly ConnectionType conTyp = ConnectionType.Undefined;
        public readonly BasicNodeWidget Widget;

        public Rectangle InWidgetPosition;

        public InConnection Out;
        public Color color;

        public bool Boolean = false;
        public ActorInfo ActorInfo = null;
        public PlayerReference Player = null;
        public Nullable<CPos> Location = null;
        public List<CPos> CellArray = new List<CPos>();
        public Nullable<int> Number = null;
        public string String = null;
        public string[] Strings = { };

        public OutConnection(ConnectionType conectionType, BasicNodeWidget widget)
        {
            conTyp = conectionType;
            Widget = widget;
            switch (conectionType)
            {
                case ConnectionType.Undefined:
                    color = Color.Black;
                    break;
                case ConnectionType.Actor:
                    color = Color.CornflowerBlue;
                    break;
                case ConnectionType.ActorList:
                    color = Color.DarkBlue;
                    break;
                case ConnectionType.ActorInfo:
                    color = Color.Indigo;
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
                case ConnectionType.LocationRange:
                    color = Color.MediumVioletRed;
                    break;
                case ConnectionType.CellArray:
                    color = Color.Violet;
                    break;
                case ConnectionType.CellPath:
                    color = Color.Violet;
                    break;
                case ConnectionType.Integer:
                    color = Color.Green;
                    break;
                case ConnectionType.Universal:
                    color = Color.White;
                    break;
                case ConnectionType.String:
                    color = Color.SlateGray;
                    break;
                case ConnectionType.Strings:
                    color = Color.DarkSlateGray;
                    break;
            }

            ConnecitonName = "Output" + (widget.InConnections.Count + 1);
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
        LocationRange,
        CellArray,
        CellPath,
        Integer,
        Universal,
        ActorList,
        String,
        Strings
    }
}