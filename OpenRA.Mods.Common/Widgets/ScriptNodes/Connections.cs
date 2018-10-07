using System;
using System.Collections.Generic;
using System.Drawing;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class InConnection
    {
        public BasicNodeWidget Widget;
        public NodeLogic Logic;
        public ConnectionType ConTyp;
        public string ConnectionId;

        public Rectangle InWidgetPosition;

        public OutConnection In;
        public Color Color;

        public InConnection(ConnectionType conectionType, BasicNodeWidget widget = null, NodeLogic nodeLogic = null)
        {
            ConTyp = conectionType;
            Widget = widget;
            Logic = nodeLogic;

            switch (conectionType)
            {
                // Irregular Connections
                case ConnectionType.Exec:
                    Color = Color.White;
                    break;
                case ConnectionType.Undefined:
                    Color = Color.Black;
                    break;
                case ConnectionType.Universal:
                    Color = Color.Khaki;
                    break;

                // Regular Connections
                case ConnectionType.Actor:
                    Color = Color.CornflowerBlue;
                    break;
                case ConnectionType.ActorList:
                    Color = Color.DeepSkyBlue;
                    break;
                case ConnectionType.ActorInfo:
                    Color = Color.SandyBrown;
                    break;
                case ConnectionType.ActorInfoArray:
                    Color = Color.RosyBrown;
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
                case ConnectionType.String:
                    Color = Color.SlateGray;
                    break;
                case ConnectionType.StringArray:
                    Color = Color.DarkSlateGray;
                    break;
                case ConnectionType.Repeatable:
                    Color = Color.IndianRed;
                    break;

                // Specific Connections
                case ConnectionType.TimerConnection:
                    Color = Color.LawnGreen;
                    break;
            }
        }
    }

    public class OutConnection
    {
        public ConnectionType ConTyp;
        public BasicNodeWidget Widget;
        public string ConnectionId;
        public NodeLogic Logic;

        public Rectangle InWidgetPosition;

        public InConnection Out;
        public Color Color;

        public Actor Actor = null;
        public Actor[] ActorGroup = null;
        public ActorInfo ActorInfo = null;
        public ActorInfo[] ActorInfos = null;
        public EditorActorPreview ActorPrev = null;
        public EditorActorPreview[] ActorPrevs = null;
        public PlayerReference Player = null;
        public PlayerReference[] PlayerGroup = null;
        public CPos? Location = null;
        public List<CPos> CellArray = new List<CPos>();
        public int? Number = null;
        public string String = null;
        public string[] Strings = { };
        public bool Boolean = true;

        public OutConnection(ConnectionType conectionType, BasicNodeWidget widget = null)
        {
            ConTyp = conectionType;
            Widget = widget;
            switch (conectionType)
            {
                // Irregular Connections
                case ConnectionType.Exec:
                    Color = Color.White;
                    break;
                case ConnectionType.Undefined:
                    Color = Color.Black;
                    break;
                case ConnectionType.Universal:
                    Color = Color.Khaki;
                    break;

                // Regular Connections
                case ConnectionType.Actor:
                    Color = Color.CornflowerBlue;
                    break;
                case ConnectionType.ActorList:
                    Color = Color.DeepSkyBlue;
                    break;
                case ConnectionType.ActorInfo:
                    Color = Color.SandyBrown;
                    break;
                case ConnectionType.ActorInfoArray:
                    Color = Color.RosyBrown;
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
                case ConnectionType.String:
                    Color = Color.SlateGray;
                    break;
                case ConnectionType.StringArray:
                    Color = Color.DarkSlateGray;
                    break;
                case ConnectionType.Repeatable:
                    Color = Color.IndianRed;
                    break;

                // Specific Connections
                case ConnectionType.TimerConnection:
                    Color = Color.LawnGreen;
                    break;
            }
        }
    }

    public enum ConnectionType
    {
        // Irregular Connections
        Undefined,
        Exec,
        Universal,

        // Regular Connections
        Actor,
        ActorInfo,
        ActorInfoArray,
        Player,
        PlayerGroup,
        Location,
        LocationRange,
        CellArray,
        CellPath,
        Integer,
        ActorList,
        String,
        StringArray,
        Repeatable,

        // Specific Connections
        TimerConnection
    }
}