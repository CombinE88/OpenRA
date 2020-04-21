using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class UiNodeUiSettings : NodeWidget
    {
        public static Dictionary<NodeType, BuildNodeConstructorInfo> NodeBuilder =
            new Dictionary<NodeType, BuildNodeConstructorInfo>()
            {
                {
                    NodeType.UiPlayNotification, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(UiLogicUiSettings),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.PlayerGroup, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.UiPlaySound, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(UiLogicUiSettings),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.UiRadarPing, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(UiLogicUiSettings),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.UiTextMessage, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(UiLogicUiSettings),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.UiAddMissionText, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(UiLogicUiSettings),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.CreateEffect, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(FunctionCreateEffectLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        public UiNodeUiSettings(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class UiObjectivesNode : NodeWidget
    {
        public static Dictionary<NodeType, BuildNodeConstructorInfo> NodeBuilder =
            new Dictionary<NodeType, BuildNodeConstructorInfo>()
            {
                {
                    NodeType.UiNewObjective, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(UiLogicUiSettings),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.PlayerGroup, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Objective, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };


        readonly DropDownButtonWidget methodSelection;
        CompareItem selectedMethod;

        public UiObjectivesNode(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            Item = CompareItem.Primary;

            var method = new List<CompareItem>
            {
                CompareItem.Primary,
                CompareItem.Secondary
            };

            selectedMethod = Item.Value;
            methodSelection = new DropDownButtonWidget(Screen.NodeScriptContainerWidget.ModData);

            Func<CompareItem, ScrollItemWidget, ScrollItemWidget> setupItem2 = (option, template) =>
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
            methodSelection.Text = selectedMethod.ToString();
            AddChild(methodSelection);
            methodSelection.Bounds =
                new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);
            if (NodeInfo.Item != null)
            {
                selectedMethod = NodeInfo.Item.Value;
                methodSelection.Text = NodeInfo.Item.Value.ToString();
            }
        }
    }

    public class UiLogicUiSettings : NodeLogic
    {
        public UiLogicUiSettings(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            switch (NodeType)
            {
                case NodeType.UiPlayNotification:
                {
                    var playerGroup = GetLinkedConnectionFromInConnection(ConnectionType.PlayerGroup, 0);
                    if (playerGroup == null || playerGroup.PlayerGroup == null || !playerGroup.PlayerGroup.Any())
                        Debug.WriteLine(NodeId + "Ui Player Group not connected");

                    var inputString = GetLinkedConnectionFromInConnection(ConnectionType.String, 0);
                    if (inputString == null || inputString.String == null)
                        Debug.WriteLine(NodeId + "Ui Type Notification not connected");

                    var speech = "Speech";
                    var voiceSound = GetLinkedConnectionFromInConnection(ConnectionType.String, 1);
                    if (voiceSound != null && voiceSound.String != null)
                        speech = voiceSound.String;

                    foreach (var player in playerGroup.PlayerGroup.ToArray())
                        Game.Sound.PlayNotification(
                            world.Map.Rules,
                            world.Players.First(p => p.InternalName == player.Name), speech,
                            inputString.String, null);
                    break;
                }
                case NodeType.UiPlaySound:
                {
                    var inputString = GetLinkedConnectionFromInConnection(ConnectionType.String, 0);
                    if (inputString == null || inputString.String == null)
                        Debug.WriteLine(NodeId + "Ui Player Group not connected");

                    var location = GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
                    if (location == null || location.Location == null)
                        Debug.WriteLine(NodeId + "Ui Location not connected");

                    Game.Sound.Play(SoundType.World, inputString.String,
                        world.Map.CenterOfCell(location.Location.Value));
                    break;
                }
                case NodeType.UiRadarPing:
                {
                    var location = GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
                    if (location == null || location.Location == null)
                        Debug.WriteLine(NodeId + "Ui Location not connected");

                    new RadarPing(() => true,
                        world.Map.CenterOfCell(location.Location.Value),
                        Color.White,
                        25,
                        200,
                        15,
                        4,
                        0.12f);
                    break;
                }
                case NodeType.UiTextMessage:
                {
                    var inputString = GetLinkedConnectionFromInConnection(ConnectionType.String, 0);
                    if (inputString == null || inputString.String == null)
                        Debug.WriteLine(NodeId + "Ui first string not connected");

                    var inputsecondString = GetLinkedConnectionFromInConnection(ConnectionType.String, 1);
                    if (inputsecondString == null || inputsecondString.String == null)
                        Debug.WriteLine(NodeId + "Ui message string not connected");

                    Game.AddChatLine(Color.CornflowerBlue, inputString.String, inputsecondString.String);
                    break;
                }
                case NodeType.UiAddMissionText:
                {
                    var inputString = GetLinkedConnectionFromInConnection(ConnectionType.String, 0);
                    if (inputString == null || inputString.String == null)
                        Debug.WriteLine(NodeId + "Ui String not connected");

                    var luaLabel = Ui.Root.Get("INGAME_ROOT").Get<LabelWidget>("MISSION_TEXT");
                    luaLabel.GetText = () => inputString.String;
                    break;
                }
                case NodeType.UiNewObjective:
                {
                    var singlePlayer = GetLinkedConnectionFromInConnection(ConnectionType.Player, 0);
                    var playerGroup = GetLinkedConnectionFromInConnection(ConnectionType.PlayerGroup, 0);
                    var objective = GetLinkedConnectionFromInConnection(ConnectionType.Objective, 0);

                    var text = GetLinkedConnectionFromInConnection(ConnectionType.String, 0);

                    if (singlePlayer == null && playerGroup == null)
                        Debug.WriteLine(NodeId + "Ui New Mission needs either a single player or group");

                    if (text == null || text.String == null)
                        Debug.WriteLine(NodeId + "Ui String not connected");

                    if (singlePlayer != null)
                    {
                        var player = world.Players.First(pl => pl.InternalName == singlePlayer.String);
                        var mo = player.PlayerActor.Trait<MissionObjectives>();
                        objective.Number = mo.Add(player, text.String,
                            Item == CompareItem.Primary ? ObjectiveType.Primary : ObjectiveType.Secondary);
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
                                Item == CompareItem.Primary ? ObjectiveType.Primary : ObjectiveType.Secondary);
                        }

                        objective.PlayerGroup = playerGroup.PlayerGroup;
                    }

                    break;
                }
                case NodeType.UiCompleteObjective:
                {
                    var obj = GetLinkedConnectionFromInConnection(ConnectionType.Objective, 0);

                    if (obj == null)
                        Debug.WriteLine(NodeId + "Ui Complete mission needs an Objective input");

                    if (obj.Player != null)
                    {
                        var player = world.Players.First(pl => pl.InternalName == obj.Player.Name);
                        var mo = player.PlayerActor.Trait<MissionObjectives>();
                        mo.MarkCompleted(player, obj.Number.Value);
                    }
                    else if (obj.PlayerGroup != null)
                    {
                        var players = new List<Player>();
                        foreach (var playdef in obj.PlayerGroup)
                            players.Add(world.Players.First(pl => pl.InternalName == playdef.Name));

                        foreach (var player in players)
                        {
                            var mo = player.PlayerActor.Trait<MissionObjectives>();
                            mo.MarkCompleted(player, obj.Number.Value);
                        }
                    }

                    break;
                }
                case NodeType.UiFailObjective:
                {
                    var obj = GetLinkedConnectionFromInConnection(ConnectionType.Objective, 0);

                    if (obj == null)
                    {
                        Debug.WriteLine(NodeId + "Ui Complete mission needs an Objective input");
                        return;
                    }

                    if (obj.Player != null)
                    {
                        var player = world.Players.First(pl => pl.InternalName == obj.Player.Name);
                        var mo = player.PlayerActor.Trait<MissionObjectives>();
                        mo.MarkFailed(player, obj.Number.Value);
                    }
                    else if (obj.PlayerGroup != null)
                    {
                        var players = new List<Player>();
                        foreach (var playdef in obj.PlayerGroup)
                            players.Add(world.Players.First(pl => pl.InternalName == playdef.Name));

                        foreach (var player in players)
                        {
                            var mo = player.PlayerActor.Trait<MissionObjectives>();
                            mo.MarkFailed(player, obj.Number.Value);
                        }
                    }

                    break;
                }
            }

            ForwardExec(this);
        }
    }
}