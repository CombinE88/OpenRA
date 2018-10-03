using System;
using System.Collections.Generic;
using System.Drawing;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class InConnection
    {
        public readonly BasicNodeWidget Widget;
        public readonly ConnectionType ConTyp;
        public string ConnectionID;

        public Rectangle InWidgetPosition;

        public OutConnection In;
        public Color Color;

        public InConnection(ConnectionType conectionType, BasicNodeWidget widget)
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
                    Color = Color.Indigo;
                    break;
                case ConnectionType.Boolean:
                    Color = Color.DarkOliveGreen;
                    break;
                case ConnectionType.Player:
                    Color = Color.Brown;
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
                    Color = Color.White;
                    break;
                case ConnectionType.String:
                    Color = Color.SlateGray;
                    break;
                case ConnectionType.Strings:
                    Color = Color.DarkSlateGray;
                    break;
            }
        }
    }

    public class OutConnection
    {
        public readonly ConnectionType ConTyp;
        public readonly BasicNodeWidget Widget;
        public string ConnectionID;

        public Rectangle InWidgetPosition;

        public InConnection Out;
        public Color Color;

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
                    Color = Color.Indigo;
                    break;
                case ConnectionType.Boolean:
                    Color = Color.DarkOliveGreen;
                    break;
                case ConnectionType.Player:
                    Color = Color.Brown;
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
                    Color = Color.White;
                    break;
                case ConnectionType.String:
                    Color = Color.SlateGray;
                    break;
                case ConnectionType.Strings:
                    Color = Color.DarkSlateGray;
                    break;
            }
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