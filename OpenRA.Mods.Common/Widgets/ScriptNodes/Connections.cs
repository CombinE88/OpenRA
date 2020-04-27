using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class BasicConnection
    {
        public string Tooltip;
        public Color Color;
        public BasicNodeWidget Widget;
        public string ConnectionId;
        public ConnectionType ConnectionTyp;
        public Rectangle InWidgetPosition;

        public BasicConnection(ConnectionType connectionType, BasicNodeWidget widget = null)
        {
            ConnectionTyp = connectionType;
            Widget = widget;

            switch (connectionType)
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
                case ConnectionType.Enabled:
                    Color = Color.IndianRed;
                    break;

                // Specific Connections
                case ConnectionType.TimerConnection:
                    Color = Color.LawnGreen;
                    break;
                case ConnectionType.Objective:
                    Color = Color.PaleGreen;
                    break;
                case ConnectionType.Condition:
                    Color = Color.DarkRed;
                    break;
                case ConnectionType.Variable:
                    Color = Color.DimGray;
                    break;
            }
        }
    }

    public class InConnection : BasicConnection
    {
        public bool Execute = false;

        public OutConnection In;

        public InConnection(ConnectionType connectionType, BasicNodeWidget widget = null) : base(connectionType, widget)
        {
        }

        public void AddTooltip()
        {
            // TODO Fix tooltip
            /*var myPosition = Widget.InConnections.IndexOf(this);

            var dictObject = Widget.NodeInfo.GetType().GetField("NodeConstructorInformation").GetValue(null);
            var dictionary = (Dictionary<string, BuildNodeConstructorInfo>) dictObject;
            var getInConnections = dictionary[Widget.NodeType].InConnections;

            if (getInConnections == null || !getInConnections.Any() || getInConnections.Count < myPosition)
                return;

            Tooltip = getInConnections[myPosition].Item2;
            */
        }
    }

    public class OutConnection : BasicConnection
    {
        public Actor Actor = null;
        public Actor[] ActorGroup = null;
        public ActorInfo ActorInfo = null;
        public ActorInfo[] ActorInfos = null;
        public EditorActorPreview ActorPrev = null;
        public EditorActorPreview[] ActorPreviews = null;
        public bool Boolean = true;
        public List<CPos> CellArray = new List<CPos>();

        public CPos? Location = null;
        public NodeLogic Logic;
        public int? Number = null;

        public PlayerReference Player = null;
        public PlayerReference[] PlayerGroup = null;
        public string String = null;
        public string[] Strings = { };

        public InConnection Out;

        public OutConnection(ConnectionType connectionType, BasicNodeWidget widget = null) : base(connectionType,
            widget)
        {
        }

        public void AddTooltip()
        {
            // TODO Fix Tooltip
            /*
            var myPosition = Widget.OutConnections.IndexOf(this);

            var dictObject = Widget.GetType().GetField("NodeConstructorInformation").GetValue(null);
            var dictionary = (Dictionary<string, BuildNodeConstructorInfo>) dictObject;
            var getOutConnections = dictionary[Widget.NodeType].OutConnections;

            if (getOutConnections == null || !getOutConnections.Any() || getOutConnections.Count < myPosition)
                return;

            Tooltip = getOutConnections[myPosition].Item2;
            */
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
        CellArray,
        CellPath,
        Integer,
        ActorList,
        String,
        StringArray,
        Enabled,

        // Specific Connections
        TimerConnection,
        Objective,
        Condition,
        Variable
    }

    public enum VariableType
    {
        // Regular Variables
        Actor,
        ActorInfo,
        Player,
        PlayerGroup,
        Location,
        CellArray,
        CellPath,
        Integer,
        ActorList,
        Timer,
        Objective
    }
}