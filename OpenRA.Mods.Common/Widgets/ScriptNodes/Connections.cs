using System;
using System.Collections.Generic;
using System.Drawing;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class InConnection
    {
        public BasicNodeWidget Widget;
        public ConnectionType ConTyp;
        public string ConnectionId;

        public Rectangle InWidgetPosition;

        public OutConnection In;
        public Color Color;

        public InConnection(ConnectionType conectionType, BasicNodeWidget widget = null)
        {
            ConTyp = conectionType;
            Widget = widget;

            switch (conectionType)
            {
                case ConnectionType.Undefined:
                    Color = Color.Black;
                    break;
                case ConnectionType.Actor:
                    Color = Color.CornflowerBlue;
                    break;
                case ConnectionType.ActorList:
                    Color = Color.DarkBlue;
                    break;
                case ConnectionType.ActorInfo:
                    Color = Color.SandyBrown;
                    break;
                case ConnectionType.Exec:
                    Color = Color.White;
                    break;
                case ConnectionType.Player:
                    Color = Color.Brown;
                    break;
                case ConnectionType.PlayerGroup:
                    Color = Color.SaddleBrown;
                    break;
                case ConnectionType.Location:
                    Color = Color.BlueViolet;
                    break;
                case ConnectionType.LocationRange:
                    Color = Color.MediumVioletRed;
                    break;
                case ConnectionType.CellArray:
                    Color = Color.Violet;
                    break;
                case ConnectionType.CellPath:
                    Color = Color.Violet;
                    break;
                case ConnectionType.Integer:
                    Color = Color.Green;
                    break;
                case ConnectionType.Universal:
                    Color = Color.Khaki;
                    break;
                case ConnectionType.String:
                    Color = Color.SlateGray;
                    break;
                case ConnectionType.Strings:
                    Color = Color.DarkSlateGray;
                    break;
                case ConnectionType.Boolean:
                    Color = Color.IndianRed;
                    break;
            }
        }
    }

    public class OutConnection
    {
        public ConnectionType ConTyp;
        public BasicNodeWidget Widget;
        public string ConnectionId;

        public Rectangle InWidgetPosition;

        public InConnection Out;
        public Color Color;

        public Actor Actor = null;
        public Actor[] ActorGroup = null;
        public ActorInfo ActorInfo = null;
        public PlayerReference Player = null;
        public PlayerReference[] PlayerGroup = null;
        public Nullable<CPos> Location = null;
        public List<CPos> CellArray = new List<CPos>();
        public Nullable<int> Number = null;
        public string String = null;
        public string[] Strings = { };
        public bool Boolean = true;

        public OutConnection(ConnectionType conectionType, BasicNodeWidget widget = null)
        {
            ConTyp = conectionType;
            Widget = widget;
            switch (conectionType)
            {
                case ConnectionType.Undefined:
                    Color = Color.Black;
                    break;
                case ConnectionType.Actor:
                    Color = Color.CornflowerBlue;
                    break;
                case ConnectionType.ActorList:
                    Color = Color.DarkBlue;
                    break;
                case ConnectionType.ActorInfo:
                    Color = Color.SandyBrown;
                    break;
                case ConnectionType.Exec:
                    Color = Color.White;
                    break;
                case ConnectionType.Player:
                    Color = Color.Brown;
                    break;
                case ConnectionType.PlayerGroup:
                    Color = Color.SaddleBrown;
                    break;
                case ConnectionType.Location:
                    Color = Color.BlueViolet;
                    break;
                case ConnectionType.LocationRange:
                    Color = Color.MediumVioletRed;
                    break;
                case ConnectionType.CellArray:
                    Color = Color.Violet;
                    break;
                case ConnectionType.CellPath:
                    Color = Color.Violet;
                    break;
                case ConnectionType.Integer:
                    Color = Color.Green;
                    break;
                case ConnectionType.Universal:
                    Color = Color.Khaki;
                    break;
                case ConnectionType.String:
                    Color = Color.SlateGray;
                    break;
                case ConnectionType.Strings:
                    Color = Color.DarkSlateGray;
                    break;
                case ConnectionType.Boolean:
                    Color = Color.IndianRed;
                    break;
            }
        }
    }

    public enum ConnectionType
    {
        Undefined,
        Actor,
        Exec,
        ActorInfo,
        Player,
        PlayerGroup,
        Location,
        LocationRange,
        CellArray,
        CellPath,
        Integer,
        Universal,
        ActorList,
        String,
        Strings,
        Boolean
    }
}