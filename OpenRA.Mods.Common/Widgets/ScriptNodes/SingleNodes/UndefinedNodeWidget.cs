using System;
using System.Collections.Generic;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ConditionNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes;
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
                    "UiCompleteObjective", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(UiLogicUiSettings),
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
                // TOOD: DoMultiple is missing, keep it missing? Find usecase first
                {
                    "TimedExecution", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TimedExecutionLogic),
                        Nesting = new[] {"Functions"},
                        Name = "Timed Execution",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    "UiFailObjective", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(UiLogicUiSettings),
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
                },
                {
                    "CompareActor", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(ProvideCondition),
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
                        LogicClass = typeof(ProvideCondition),
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
                        LogicClass = typeof(ProvideCondition),
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
                        LogicClass = typeof(ProvideCondition),
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
                        LogicClass = typeof(ProvideCondition),
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
                        LogicClass = typeof(ProvideCondition),
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
                        LogicClass = typeof(ProvideCondition),
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
                        LogicClass = typeof(ProvideCondition),
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
                        LogicClass = typeof(ProvideCondition),
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
                        LogicClass = typeof(ProvideCondition),
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
                        LogicClass = typeof(ProvideCondition),
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