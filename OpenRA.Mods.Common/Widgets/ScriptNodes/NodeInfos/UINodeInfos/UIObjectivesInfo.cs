using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.UINodeInfos
{
    public class UiObjectivesInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "UiNewObjective", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"User Interface", "Objectives"},
                        Name = "Create new Objective",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.PlayerGroup, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Objective, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        DropDownButtonWidget methodSelection;
        string selectedMethod;


        public UiObjectivesInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            var singlePlayer = logic.GetLinkedConnectionFromInConnection(ConnectionType.Player, 0);
            var playerGroup = logic.GetLinkedConnectionFromInConnection(ConnectionType.PlayerGroup, 0);
            var objective = logic.GetLinkedConnectionFromInConnection(ConnectionType.Objective, 0);

            var text = logic.GetLinkedConnectionFromInConnection(ConnectionType.String, 0);

            if (singlePlayer == null && playerGroup == null)
                Debug.WriteLine(NodeId + "Ui New Mission needs either a single player or group");

            if (text == null || text.String == null)
                Debug.WriteLine(NodeId + "Ui String not connected");

            if (singlePlayer != null)
            {
                var player = world.Players.First(pl => pl.InternalName == singlePlayer.String);
                var mo = player.PlayerActor.Trait<MissionObjectives>();
                objective.Number = mo.Add(player, text.String,
                    Item == "Primary" ? ObjectiveType.Primary : ObjectiveType.Secondary);
                objective.Player = player.PlayerReference;
            }
            else if (playerGroup != null)
            {
                var players = new List<Player>();
                foreach (var playdef in playerGroup.PlayerGroup)
                    players.Add(world.Players.First(pl => pl.InternalName == playdef.Name));

                foreach (var player in players)
                {
                    var mo = player.PlayerActor.Trait<MissionObjectives>();
                    objective.Number = mo.Add(player, text.String,
                        Item == "Primary" ? ObjectiveType.Primary : ObjectiveType.Secondary);
                }

                objective.PlayerGroup = playerGroup.PlayerGroup;
            }

            NodeLogic.ForwardExec(logic);
        }

        public override void WidgetInitialize(NodeWidget widget)
        {
            Item = "Primary";

            var method = new List<string>
            {
                "Primary",
                "Secondary"
            };

            selectedMethod = Item;
            methodSelection = new DropDownButtonWidget(widget.Screen.NodeScriptContainerWidget.ModData);

            Func<string, ScrollItemWidget, ScrollItemWidget> setupItem2 = (option, template) =>
            {
                var item = ScrollItemWidget.Setup(template, () => selectedMethod == option, () =>
                {
                    selectedMethod = option;

                    methodSelection.Text = selectedMethod.ToString();
                    Item = selectedMethod;
                });

                item.Get<LabelWidget>("LABEL").GetText = () => option.ToString();

                return item;
            };

            methodSelection.OnClick = () =>
            {
                methodSelection.ShowDropDown("LABEL_DROPDOWN_TEMPLATE", 270, method, setupItem2);
            };
            methodSelection.Text = selectedMethod;
            widget.AddChild(methodSelection);
            methodSelection.Bounds =
                new Rectangle(widget.FreeWidgetEntries.X, widget.FreeWidgetEntries.Y + 50,
                    widget.FreeWidgetEntries.Width, 25);
        }

        public override void WidgetAddOutConConstructor(OutConnection connection, NodeWidget widget)
        {
            base.WidgetAddOutConConstructor(connection, widget);
            if (Item != null)
            {
                selectedMethod = Item;
                methodSelection.Text = Item;
            }
        }
    }
}