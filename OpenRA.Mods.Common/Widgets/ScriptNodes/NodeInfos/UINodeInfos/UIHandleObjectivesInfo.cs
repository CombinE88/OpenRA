using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.UINodeInfos
{
    public class UIHandleObjectivesInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "UiCompleteObjective", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"User Interface", "Objectives"},
                        Name = "Complete Objective",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Objective, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    "UiFailObjective", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"User Interface", "Objectives"},
                        Name = "Fail Objective",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Objective, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                }
            };

        public UIHandleObjectivesInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId,
            nodeName)
        {
        }


        public override void LogicExecute(World world, NodeLogic logic)
        {
            switch (NodeType)
            {
                case "UiCompleteObjective":
                {
                    var obj = logic.GetLinkedConnectionFromInConnection(ConnectionType.Objective, 0);

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
                case "UiFailObjective":
                {
                    var obj = logic.GetLinkedConnectionFromInConnection(ConnectionType.Objective, 0);

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
        }
    }
}