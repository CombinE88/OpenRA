using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class UiNodeUiSettings : NodeWidget
    {
        public UiNodeUiSettings(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class UiLogicUiSettings : NodeLogic
    {
        public UiLogicUiSettings(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            if (NodeType == NodeType.UiPlayNotification)
            {
                if (InConnections.First(ic => ic.ConTyp == ConnectionType.PlayerGroup).In == null
                    || InConnections.First(ic => ic.ConTyp == ConnectionType.PlayerGroup).In.PlayerGroup == null
                    || !InConnections.First(ic => ic.ConTyp == ConnectionType.PlayerGroup).In.PlayerGroup.Any())
                    throw new YamlException(NodeId + "Ui Player Group not connected");

                if (InConnections.First(ic => ic.ConTyp == ConnectionType.String).In == null
                    || InConnections.First(ic => ic.ConTyp == ConnectionType.String).In.String == null)
                    throw new YamlException(NodeId + "Ui Type Notification not connected");

                var speech = "Speech";

                if (InConnections.Last(ic => ic.ConTyp == ConnectionType.String).In != null
                    && InConnections.Last(ic => ic.ConTyp == ConnectionType.String).In.String != null)
                    speech = InConnections.Last(ic => ic.ConTyp == ConnectionType.String).In.String;

                foreach (var player in InConnections.First(c => c.ConTyp == ConnectionType.PlayerGroup).In.PlayerGroup.ToArray())
                {
                    Game.Sound.PlayNotification(
                        world.Map.Rules,
                        world.Players.First(p => p.InternalName == player.Name), speech, InConnections.First(ic => ic.ConTyp == ConnectionType.String).In.String, null);
                }
            }
            else if (NodeType == NodeType.UiPlaySound)
            {
                if (InConnections.First(ic => ic.ConTyp == ConnectionType.String).In == null
                    || InConnections.First(ic => ic.ConTyp == ConnectionType.String).In.String == null)
                    throw new YamlException(NodeId + "Ui Player Group not connected");

                if (InConnections.First(ic => ic.ConTyp == ConnectionType.Location).In == null
                    || InConnections.First(ic => ic.ConTyp == ConnectionType.Location).In.Location == null)
                    throw new YamlException(NodeId + "Ui Location not connected");

                var sound = InConnections.First(ic => ic.ConTyp == ConnectionType.String).In.String;

                Game.Sound.Play(SoundType.World, sound, world.Map.CenterOfCell(InConnections.First(ic => ic.ConTyp == ConnectionType.Location).In.Location.Value));
            }
            else if (NodeType == NodeType.UiRadarPing)
            {
                if (InConnections.First(ic => ic.ConTyp == ConnectionType.Location).In == null
                    || InConnections.First(ic => ic.ConTyp == ConnectionType.Location).In.Location == null)
                    throw new YamlException(NodeId + "Ui Location not connected");

                var radarPing = new RadarPing(() => true,
                    world.Map.CenterOfCell(InConnections.First(ic => ic.ConTyp == ConnectionType.Location).In.Location.Value),
                    Color.White,
                    25,
                    200,
                    15,
                    4,
                    0.12f);
            }
            else if (NodeType == NodeType.UiTextMessage)
            {
                if (InConnections.First(ic => ic.ConTyp == ConnectionType.String).In == null
                    || InConnections.First(ic => ic.ConTyp == ConnectionType.String).In.String == null)
                    throw new YamlException(NodeId + "Ui first string not connected");

                if (InConnections.Last(ic => ic.ConTyp == ConnectionType.String).In == null
                    || InConnections.Last(ic => ic.ConTyp == ConnectionType.String).In.String == null)
                    throw new YamlException(NodeId + "Ui message string not connected");

                Game.AddChatLine(Color.CornflowerBlue,
                    InConnections.First(ic => ic.ConTyp == ConnectionType.String).In.String,
                    InConnections.Last(ic => ic.ConTyp == ConnectionType.String).In.String);
            }
            else if (NodeType == NodeType.UiAddMissionText)
            {
                if (InConnections.First(ic => ic.ConTyp == ConnectionType.String).In == null
                    || InConnections.First(ic => ic.ConTyp == ConnectionType.String).In.String == null)
                    throw new YamlException(NodeId + "Ui String not connected");

                var luaLabel = Ui.Root.Get("INGAME_ROOT").Get<LabelWidget>("MISSION_TEXT");
                luaLabel.GetText = () => InConnections.First(ic => ic.ConTyp == ConnectionType.String).In.String;
            }
        }
    }
}