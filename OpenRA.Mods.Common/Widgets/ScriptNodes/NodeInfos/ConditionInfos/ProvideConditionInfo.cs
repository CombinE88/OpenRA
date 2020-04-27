using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.ConditionInfos
{
    public class ProvideConditionInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "CompareActor", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Conditions", "Actor Conditions"},
                        Name = "Same Actor",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        }
                    }
                },
                {
                    "CompareActorInfo", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Conditions", "Actor Conditions"},
                        Name = "Same Actor Type",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfo, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfo, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        }
                    }
                },
                {
                    "IsAlive", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Conditions", "Actor Conditions"},
                        Name = "is Alive",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        }
                    }
                },
                {
                    "IsDead", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Conditions", "Actor Conditions"},
                        Name = "is Dead",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        }
                    }
                },
                {
                    "CompareNumber", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Conditions"},
                        Name = "Equal Number",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        }
                    }
                },
                {
                    "IsPlaying", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Conditions", "Player Conditions"},
                        Name = "Still Playing",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        }
                    }
                },
                {
                    "IsBot", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Conditions", "Player Conditions"},
                        Name = "Is a Bot",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        }
                    }
                },
                {
                    "IsHumanPlayer", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Conditions", "Player Conditions"},
                        Name = "Is Human Player",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        }
                    }
                },
                {
                    "IsNoncombatant", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Conditions", "Player Conditions"},
                        Name = "Is Noncombatant",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        }
                    }
                },
                {
                    "HasWon", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Conditions", "Player Conditions"},
                        Name = "Has Won",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        }
                    }
                },
                {
                    "HasLost", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Conditions", "Player Conditions"},
                        Name = "Has Lost",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        }
                    }
                },
            };

        public ProvideConditionInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId,
            nodeName)
        {
        }

        public override bool LogicCheckCondition(World world, NodeLogic logic)
        {
            if (NodeType == "CompareActor")
            {
                var actCon1 = logic.GetLinkedConnectionFromInConnection(ConnectionType.Actor, 0);
                var actCon2 = logic.GetLinkedConnectionFromInConnection(ConnectionType.Actor, 1);

                if (actCon1 == null)
                    Debug.WriteLine(NodeId + "Actor 1 not connected");

                if (actCon2 == null)
                    Debug.WriteLine(NodeId + "Actor 2 not connected");

                if (actCon1.Actor == null || actCon2.Actor == null)
                    return false;

                return actCon1.Actor.Equals(actCon2.Actor);
            }

            if (NodeType == "CompareNumber")
            {
                var actCon1 = logic.GetLinkedConnectionFromInConnection(ConnectionType.Integer, 0);
                var actCon2 = logic.GetLinkedConnectionFromInConnection(ConnectionType.Integer, 1);

                if (actCon1 == null)
                {
                    Debug.WriteLine(NodeId + "Number 1 not connected");
                    return false;
                }

                if (actCon2 == null)
                {
                    Debug.WriteLine(NodeId + "Number 2 not connected");
                    return false;
                }

                if (actCon1.Number == null || actCon2.Number == null)
                    return false;

                return actCon1.Number.Value.Equals(actCon2.Number.Value);
            }

            if (NodeType == "CompareActorInfo")
            {
                var actCon1 = logic.GetLinkedConnectionFromInConnection(ConnectionType.ActorInfo, 0);
                var actCon2 = logic.GetLinkedConnectionFromInConnection(ConnectionType.ActorInfo, 1);

                if (actCon1 == null)
                {
                    Debug.WriteLine(NodeId + "Actor Info 1 not connected");
                    return false;
                }

                if (actCon2 == null)
                {
                    Debug.WriteLine(NodeId + "Actor Info 2 not connected");
                    return false;
                }

                if (actCon1.ActorInfo == null || actCon2.ActorInfo == null)
                    return false;

                return actCon1.ActorInfo.Equals(actCon2.ActorInfo);
            }

            if (NodeType == "IsAlive")
            {
                var actCon1 = logic.GetLinkedConnectionFromInConnection(ConnectionType.Actor, 0);

                if (actCon1 == null)
                {
                    Debug.WriteLine(NodeId + "Actor not connected");
                    return false;
                }

                if (actCon1.ActorInfo == null)
                    return false;

                return !actCon1.Actor.IsDead;
            }

            if (NodeType == "IsDead")
            {
                var actCon1 = logic.GetLinkedConnectionFromInConnection(ConnectionType.Actor, 0);

                if (actCon1 == null)
                {
                    Debug.WriteLine(NodeId + "Actor not connected");
                    return false;
                }

                if (actCon1.ActorInfo == null)
                    return false;

                return actCon1.Actor.IsDead;
            }

            if (NodeType == "IsPlaying"
                || NodeType == "IsBot"
                || NodeType == "IsHumanPlayer"
                || NodeType == "IsNoncombatant"
                || NodeType == "HasWon"
                || NodeType == "HasLost")
            {
                var actCon1 = logic.GetLinkedConnectionFromInConnection(ConnectionType.Player, 0);

                if (actCon1 == null)
                {
                    Debug.WriteLine(NodeId + "Player not connected");
                    return false;
                }

                if (actCon1.Player == null)
                    return false;

                var player = world.Players.FirstOrDefault(p => p.InternalName == actCon1.Player.Name);

                if (player == null)
                    return false;

                switch (NodeType)
                {
                    case "IsPlaying":
                        return player.WinState == WinState.Undefined && !player.Spectating;
                    case "IsBot":
                        return player.IsBot;
                    case "IsHumanPlayer":
                        return !player.IsBot && !player.NonCombatant && player.Playable;
                    case "IsNoncombatant":
                        return player.NonCombatant;
                    case "HasWon":
                        return player.WinState == WinState.Won;
                    case "HasLost":
                        return player.WinState == WinState.Lost;
                }
            }

            return false;
        }
    }
}