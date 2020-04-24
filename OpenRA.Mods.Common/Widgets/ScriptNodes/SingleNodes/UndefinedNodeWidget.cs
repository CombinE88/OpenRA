using System;
using System.Collections.Generic;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ConditionNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes
{
    public class UndefinedNodeWidget : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TimerReset", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TimerLogics),
                        Nesting = new[] {"Timer"},
                        Name = "Reset Timer",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.TimerConnection, "Reference to the timer"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Resets the timer")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after the timer got reset")
                        }
                    }
                },
                {
                    "TimerStart", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TimerLogics),
                        Nesting = new[] {"Timer"},
                        Name = "Start Timer",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.TimerConnection, "Reference to the timer"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Starts the timer")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after the timer starts")
                        }
                    }
                },
                {
                    "TimerStop", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TimerLogics),
                        Nesting = new[] {"Timer"},
                        Name = "Stop Timer",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.TimerConnection, "Reference to the timer"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Stops the timer")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after the timer got stopped")
                        }
                    }
                },
                {
                    "TriggerOnAllKilled", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TriggerOnAllKilled),
                        Nesting = new[] {"Trigger"},
                        Name = "On All Actors Killed",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList,
                                "Actor group that fires the trigger"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Setup the trigger")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec,
                                "Runs when the trigger condition is met"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after the trigger has set up")
                        }
                    }
                },
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

        public UndefinedNodeWidget(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }
}