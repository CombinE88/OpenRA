using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.UINodeInfos
{
    public class UiSettingsInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "UiPlayNotification", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"User Interface", "General UI"},
                        Name = "Play Notification",

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
                    "UiPlaySound", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"User Interface", "General UI"},
                        Name = "Play Sound",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Location, "Location to play the sound at"),
                            new Tuple<ConnectionType, string>(ConnectionType.String, "Sound"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    "UiRadarPing", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"User Interface", "General UI"},
                        Name = "Radar Ping",

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
                    "UiTextMessage", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"User Interface", "General UI"},
                        Name = "Text Message",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.String, "Message"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    "UiAddMissionText", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"User Interface", "General UI"},
                        Name = "Show Mission Text",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.String, "Mission text"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        public UiSettingsInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            switch (NodeType)
            {
                case "UiPlayNotification":
                {
                    var playerGroup = logic.GetLinkedConnectionFromInConnection(ConnectionType.PlayerGroup, 0);
                    if (playerGroup == null || playerGroup.PlayerGroup == null || !playerGroup.PlayerGroup.Any())
                        Debug.WriteLine(NodeId + "Ui Player Group not connected");

                    var inputString = logic.GetLinkedConnectionFromInConnection(ConnectionType.String, 0);
                    if (inputString == null || inputString.String == null)
                        Debug.WriteLine(NodeId + "Ui Type Notification not connected");

                    var speech = "Speech";
                    var voiceSound = logic.GetLinkedConnectionFromInConnection(ConnectionType.String, 1);
                    if (voiceSound != null && voiceSound.String != null)
                        speech = voiceSound.String;

                    foreach (var player in playerGroup.PlayerGroup.ToArray())
                        Game.Sound.PlayNotification(
                            world.Map.Rules,
                            world.Players.First(p => p.InternalName == player.Name), speech,
                            inputString.String, null);
                    break;
                }
                case "UiPlaySound":
                {
                    var inputString = logic.GetLinkedConnectionFromInConnection(ConnectionType.String, 0);
                    if (inputString == null || inputString.String == null)
                        Debug.WriteLine(NodeId + "Ui Player Group not connected");

                    var location = logic.GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
                    if (location == null || location.Location == null)
                        Debug.WriteLine(NodeId + "Ui Location not connected");

                    Game.Sound.Play(SoundType.World, inputString.String,
                        world.Map.CenterOfCell(location.Location.Value));
                    break;
                }
                case "UiRadarPing":
                {
                    var location = logic.GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
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
                case "UiTextMessage":
                {
                    var inputString = logic.GetLinkedConnectionFromInConnection(ConnectionType.String, 0);
                    if (inputString == null || inputString.String == null)
                        Debug.WriteLine(NodeId + "Ui first string not connected");

                    var inputsecondString = logic.GetLinkedConnectionFromInConnection(ConnectionType.String, 1);
                    if (inputsecondString == null || inputsecondString.String == null)
                        Debug.WriteLine(NodeId + "Ui message string not connected");

                    Game.AddChatLine(Color.CornflowerBlue, inputString.String, inputsecondString.String);
                    break;
                }
                case "UiAddMissionText":
                {
                    var inputString = logic.GetLinkedConnectionFromInConnection(ConnectionType.String, 0);
                    if (inputString == null || inputString.String == null)
                        Debug.WriteLine(NodeId + "Ui String not connected");

                    var luaLabel = Ui.Root.Get("INGAME_ROOT").Get<LabelWidget>("MISSION_TEXT");
                    luaLabel.GetText = () => inputString.String;
                    break;
                }
            }

            NodeLogic.ForwardExec(logic);
        }
    }
}