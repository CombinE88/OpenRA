using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets;
using OpenRA.Mods.Common.Widgets.ScriptNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class UiNodeUiSettings : NodeWidget
    {
        public UiNodeUiSettings(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class UiObjectivesNode : NodeWidget
    {
        CompareItem selectedMethode;
        DropDownButtonWidget methodeSelection;

        public UiObjectivesNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Item = CompareItem.Primary;

            List<CompareItem> methodes = new List<CompareItem>
            {
                CompareItem.Primary,
                CompareItem.Secondary
            };

            selectedMethode = Item.Value;
            methodeSelection = new DropDownButtonWidget(Screen.Snw.ModData);

            Func<CompareItem, ScrollItemWidget, ScrollItemWidget> setupItem2 = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedMethode == option, () =>
                {
                    selectedMethode = option;

                    methodeSelection.Text = selectedMethode.ToString();
                    Item = selectedMethode;
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.ToString();

                return item;
            };

            methodeSelection.OnClick = () => { methodeSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, methodes, setupItem2); };

            methodeSelection.Text = selectedMethode.ToString();

            AddChild(methodeSelection);

            methodeSelection.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            if (NodeInfo.Item != null)
            {
                selectedMethode = NodeInfo.Item.Value;
                methodeSelection.Text = NodeInfo.Item.Value.ToString();
            }
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

                new RadarPing(() => true,
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
            else if (NodeType == NodeType.UiNewObjective)
            {
                var p = InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Player);
                var pg = InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.PlayerGroup);
                var ouCon = OutConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Objective);

                var strg = InConnections.First(ic => ic.ConTyp == ConnectionType.String);

                if (p.In == null && pg.In == null)
                    throw new YamlException(NodeId + "Ui New Mission needs either a single player or group");

                if (strg.In == null || strg.In.String == null)
                    throw new YamlException(NodeId + "Ui String not connected");

                if (p.In != null)
                {
                    var player = world.Players.First(pl => pl.InternalName == p.In.Player.Name);
                    var mo = player.PlayerActor.Trait<MissionObjectives>();
                    ouCon.Number = mo.Add(player, strg.In.String, Item == CompareItem.Primary ? ObjectiveType.Primary : ObjectiveType.Secondary);
                    ouCon.Player = p.In.Player;
                }
                else if (pg.In != null)
                {
                    var players = new List<Player>();
                    foreach (var playdef in pg.In.PlayerGroup)
                    {
                        players.Add(world.Players.First(pl => pl.InternalName == playdef.Name));
                    }

                    foreach (var player in players)
                    {
                        var mo = player.PlayerActor.Trait<MissionObjectives>();
                        ouCon.Number = mo.Add(player, strg.In.String, Item == CompareItem.Primary ? ObjectiveType.Primary : ObjectiveType.Secondary);
                    }

                    ouCon.PlayerGroup = pg.In.PlayerGroup;
                }
            }
            else if (NodeType == NodeType.UiCompleteObjective)
            {
                var obj = InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Objective);

                if (obj.In == null)
                    throw new YamlException(NodeId + "Ui Complete mission needs an Objective input");

                if (obj.In.Player != null)
                {
                    var player = world.Players.First(pl => pl.InternalName == obj.In.Player.Name);
                    var mo = player.PlayerActor.Trait<MissionObjectives>();
                    mo.MarkCompleted(player, obj.In.Number.Value);
                }
                else if (obj.In.PlayerGroup != null)
                {
                    var players = new List<Player>();
                    foreach (var playdef in obj.In.PlayerGroup)
                    {
                        players.Add(world.Players.First(pl => pl.InternalName == playdef.Name));
                    }

                    foreach (var player in players)
                    {
                        var mo = player.PlayerActor.Trait<MissionObjectives>();
                        mo.MarkCompleted(player, obj.In.Number.Value);
                    }
                }
            }
            else if (NodeType == NodeType.UIFailObjective)
            {
                var obj = InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Objective);

                if (obj.In == null)
                    throw new YamlException(NodeId + "Ui Complete mission needs an Objective input");

                if (obj.In.Player != null)
                {
                    var player = world.Players.First(pl => pl.InternalName == obj.In.Player.Name);
                    var mo = player.PlayerActor.Trait<MissionObjectives>();
                    mo.MarkFailed(player, obj.In.Number.Value);
                }
                else if (obj.In.PlayerGroup != null)
                {
                    var players = new List<Player>();
                    foreach (var playdef in obj.In.PlayerGroup)
                    {
                        players.Add(world.Players.First(pl => pl.InternalName == playdef.Name));
                    }

                    foreach (var player in players)
                    {
                        var mo = player.PlayerActor.Trait<MissionObjectives>();
                        mo.MarkFailed(player, obj.In.Number.Value);
                    }
                }
            }
        }
    }
}