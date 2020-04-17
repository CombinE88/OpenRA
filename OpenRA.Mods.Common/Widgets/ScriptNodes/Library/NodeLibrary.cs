using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ConditionNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Variables;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.Library
{
    public static class NodeLibrary
    {
        public static List<NodeWidget> LoadInNodes(NodeEditorNodeScreenWidget nensw, List<NodeInfo> nodeInfos)
        {
            var nodes = new List<NodeWidget>();
            foreach (var nodeinfo in nodeInfos)
            {
                var instanceType = NodeConstructorInfo.NodeConstructorInfos[nodeinfo.NodeType].ConstructorClass;

                var newNode = (NodeWidget) instanceType
                    .GetConstructor(new[] {typeof(NodeEditorNodeScreenWidget), typeof(NodeInfo)})
                    .Invoke(new object[] {nensw, nodeinfo});
                nodes.Add(newNode);
            }

            return nodes;
        }

        public static NodeWidget AddNode(NodeType nodeType, NodeEditorNodeScreenWidget nensw, string nodeId = null,
            string nodeName = null)
        {
            if (nodeType == NodeType.TriggerWorldLoaded &&
                nensw.Nodes.Any(n => n.NodeType == NodeType.TriggerWorldLoaded))
                return null;

            if (nodeType == NodeType.TriggerTick &&
                nensw.Nodes.Any(n => n.NodeType == NodeType.TriggerTick))
                return null;

            var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

            var instanceType = NodeConstructorInfo.NodeConstructorInfos[nodeType].ConstructorClass;

            var newNode = (NodeWidget) instanceType
                .GetConstructor(new[] {typeof(NodeEditorNodeScreenWidget), typeof(NodeInfo)})
                .Invoke(new object[] {nensw, nodeInfo});

            if (NodeConstructorInfo.NodeConstructorInfos[nodeType].InConnections != null)
                foreach (var connection in NodeConstructorInfo.NodeConstructorInfos[nodeType].InConnections)
                    newNode.AddInConnection(new InConnection(connection, newNode));

            if (NodeConstructorInfo.NodeConstructorInfos[nodeType].OutConnections != null)
                foreach (var connection in NodeConstructorInfo.NodeConstructorInfos[nodeType].OutConnections)
                    newNode.AddOutConnection(new OutConnection(connection, newNode));

            return newNode;
        }

        public static List<NodeLogic> InitializeNodes(IngameNodeScriptSystem inss, List<NodeInfo> nodesInfos)
        {
            var nodeList = new List<NodeLogic>();
            foreach (var nodeinfo in nodesInfos)
                if (nodeinfo.NodeType == NodeType.MapInfoNode)
                {
                    var newNode = new MapInfoLogicNode(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerWorldLoaded)
                {
                    var newNode = new TriggerLogicWorldLoaded(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerTick)
                {
                    var newNode = new TriggerLogicTick(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerOnEnteredFootprint)
                {
                    var newNode = new TriggerLogicEnteredFootPrint(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerOnEnteredRange)
                {
                    var newNode = new TriggerLogicOnEnteredRange(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerCreateTimer)
                {
                    var newNode = new TriggerLogicCreateTimer(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.GroupPlayerGroup)
                {
                    var newNode = new GroupPlayerLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.GroupActorGroup)
                {
                    var newNode = new GroupActorLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.GroupActorInfoGroup)
                {
                    var newNode = new GroupActorInfoLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorCreateActor)
                {
                    var newNode = new ActorCreateActorLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueMove)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueAttack)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueHunt)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueAttackMoveActivity)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueSell)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueFindResources)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorKill)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorRemove)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorChangeOwner)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.Reinforcements)
                {
                    var newNode = new FunctionLogicReinforcements(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ReinforcementsWithTransport)
                {
                    var newNode = new FunctionLogicReinforcements(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.MapInfoActorInfo)
                {
                    var newNode = new MapInfoLogicNode(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiPlayNotification)
                {
                    var newNode = new UiLogicUiSettings(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiPlaySound)
                {
                    var newNode = new UiLogicUiSettings(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiRadarPing)
                {
                    var newNode = new UiLogicUiSettings(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiTextMessage)
                {
                    var newNode = new UiLogicUiSettings(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiAddMissionText)
                {
                    var newNode = new UiLogicUiSettings(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.MapInfoActorInfo)
                {
                    var newNode = new NodeLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.MapInfoActorReference)
                {
                    var newNode = new NodeLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerOnIdle)
                {
                    var newNode = new TriggerOnIdle(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerOnKilled)
                {
                    var newNode = new TriggerOnKilled(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerOnAllKilled)
                {
                    var newNode = new TriggerOnAllKilled(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TimerReset)
                {
                    var newNode = new TimerLogics(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TimerStart)
                {
                    var newNode = new TimerLogics(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TimerStop)
                {
                    var newNode = new TimerLogics(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ArithmeticsOr)
                {
                    var newNode = new ArithmeticBasicLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ArithmeticsAnd)
                {
                    var newNode = new ArithmeticBasicLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.CreateEffect)
                {
                    var newNode = new FunctionCreateEffectLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorGetInformations)
                {
                    var newNode = new GetActorInformationsLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.CompareActors)
                {
                    var newNode = new ArithmecCompareLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.DoMultiple)
                {
                    var newNode = new DoRepeatingNodeLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.Count)
                {
                    var newNode = new GetCountNodeLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ArithmeticsMath)
                {
                    var newNode = new ArithmeticMathNodeLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiNewObjective)
                {
                    var newNode = new UiLogicUiSettings(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiFailObjective)
                {
                    var newNode = new UiLogicUiSettings(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiCompleteObjective)
                {
                    var newNode = new UiLogicUiSettings(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.FindActorsOnFootprint)
                {
                    var newNode = new GroupFindActorsLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueFindResources)
                {
                    var newNode = new GroupFindActorsLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.FilterActorGroup)
                {
                    var newNode = new FilterActorListByLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.CheckCondition)
                {
                    var newNode = new CheckConditionLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.CompareActor)
                {
                    var newNode = new ProvideCondition(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.CompareNumber)
                {
                    var newNode = new ProvideCondition(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.CompareActorInfo)
                {
                    var newNode = new ProvideCondition(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.IsAlive)
                {
                    var newNode = new ProvideCondition(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.IsDead)
                {
                    var newNode = new ProvideCondition(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.IsPlaying)
                {
                    var newNode = new ProvideCondition(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.IsBot)
                {
                    var newNode = new ProvideCondition(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.IsHumanPlayer)
                {
                    var newNode = new ProvideCondition(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.IsNoncombatant)
                {
                    var newNode = new ProvideCondition(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.HasWon)
                {
                    var newNode = new ProvideCondition(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.HasLost)
                {
                    var newNode = new ProvideCondition(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.GlobalLightning)
                {
                    var newNode = new ProvideCondition(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.SetCameraPosition)
                {
                    var newNode = new SetCameraPositionNode(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TimedExecution)
                {
                    var newNode = new TimedExecutionLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TextChoice)
                {
                    var newNode = new TextBoxSelectLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.CameraRide)
                {
                    var newNode = new CameraRideNodeLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.SetVariable)
                {
                    var newNode = new SetVariableLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.GetVariable)
                {
                    var newNode = new GetVariableLogic(nodeinfo, inss);
                    nodeList.Add(newNode);
                }

            return nodeList;
        }
    }

    public enum NodeType
    {
        // MapInfo
        MapInfoNode,
        MapInfoActorInfo,
        MapInfoActorReference,

        // Actor
        ActorCreateActor,
        ActorGetInformations,
        ActorQueueMove,
        ActorQueueAttack,
        ActorQueueHunt,
        ActorQueueAttackMoveActivity,
        ActorQueueSell,
        ActorQueueFindResources,
        ActorKill,
        ActorRemove,
        ActorChangeOwner,

        // Trigger,
        TriggerWorldLoaded,
        TriggerCreateTimer,
        TimerStart,
        TimerReset,
        TimerStop,
        TriggerTick,
        TriggerOnEnteredFootprint,
        TriggerOnEnteredRange,
        TriggerOnIdle,
        TriggerOnKilled,

        // Actor Groups
        GroupPlayerGroup,
        GroupActorGroup,
        GroupActorInfoGroup,
        TriggerOnAllKilled,
        FinActorsInCircle,
        FindActorsOnFootprint,
        FilterActorGroup,

        // Arithmetic
        ArithmeticsAnd,
        ArithmeticsOr,
        CompareActors,
        DoMultiple,
        Count,
        ArithmeticsMath,

        // Complex Functions
        Reinforcements,
        ReinforcementsWithTransport,
        CreateEffect,
        TimedExecution,

        // UI Nodes
        UiPlayNotification,
        UiPlaySound,
        UiRadarPing,
        UiTextMessage,
        UiAddMissionText,
        UiNewObjective,
        UiCompleteObjective,
        UiFailObjective,
        GlobalLightning,
        SetCameraPosition,
        CameraRide,
        TextChoice,

        // Conditions
        CheckCondition,
        CompareActor,
        CompareNumber,
        CompareActorInfo,
        IsAlive,
        IsDead,
        IsPlaying,
        HasWon,
        HasLost,
        IsBot,
        IsHumanPlayer,
        IsNoncombatant,

        // Variables
        SetVariable,
        GetVariable
    }

    public class NodeContructor
    {
        public Type ConstructorClass;
        public List<ConnectionType> InConnections;
        public List<ConnectionType> OutConnections;
    }

    public static class NodeConstructorInfo
    {
        public static Dictionary<NodeType, NodeContructor> NodeConstructorInfos =
            new Dictionary<NodeType, NodeContructor>
            {
                {
                    NodeType.MapInfoNode, new NodeContructor
                    {
                        ConstructorClass = typeof(MapInfoNode)
                    }
                },
                {
                    NodeType.MapInfoActorInfo, new NodeContructor
                    {
                        ConstructorClass = typeof(MapInfoActorInfoNode),
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.ActorInfo
                        }
                    }
                },
                {
                    NodeType.MapInfoActorReference, new NodeContructor
                    {
                        ConstructorClass = typeof(MapInfoActorsonMap),
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.ActorList
                        }
                    }
                },
                {
                    NodeType.TriggerCreateTimer, new NodeContructor
                    {
                        ConstructorClass = typeof(TriggerNodeCreateTimer),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Integer,
                            ConnectionType.Repeatable,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.TimerConnection,
                            ConnectionType.Exec,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.TimerReset, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.TimerConnection,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.TimerStart, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.TimerConnection,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.TimerStop, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.TimerConnection,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.TriggerWorldLoaded, new NodeContructor
                    {
                        ConstructorClass = typeof(TriggerNodeWorldLoaded),
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.TriggerTick, new NodeContructor
                    {
                        ConstructorClass = typeof(TriggerNodeTick),
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.TriggerOnEnteredFootprint, new NodeContructor
                    {
                        ConstructorClass = typeof(TriggerNodeOnEnteredFootPrint),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.PlayerGroup,
                            ConnectionType.CellArray,
                            ConnectionType.Repeatable,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.TriggerOnEnteredRange, new NodeContructor
                    {
                        ConstructorClass = typeof(TriggerNodeOnEnteredRange),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.PlayerGroup,
                            ConnectionType.Location,
                            ConnectionType.Integer,
                            ConnectionType.Repeatable
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.TriggerOnIdle, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.Repeatable,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.Exec,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.TriggerOnKilled, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.ActorList,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.TriggerOnAllKilled, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.ActorList,
                            ConnectionType.Repeatable,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ActorCreateActor, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeCreateActor),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.ActorInfo,
                            ConnectionType.Player,
                            ConnectionType.Location,
                            ConnectionType.Integer,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ActorGetInformations, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.ActorInfo,
                            ConnectionType.Location,
                            ConnectionType.Player
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor
                        }
                    }
                },
                {
                    NodeType.GroupPlayerGroup, new NodeContructor
                    {
                        ConstructorClass = typeof(GroupPlayerGroup),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Player
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.PlayerGroup
                        }
                    }
                },
                {
                    NodeType.GroupActorGroup, new NodeContructor
                    {
                        ConstructorClass = typeof(GroupActorGroup),
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.ActorList
                        }
                    }
                },
                {
                    NodeType.GroupActorInfoGroup, new NodeContructor
                    {
                        ConstructorClass = typeof(GroupActorInfoGroup),
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.ActorInfoArray
                        }
                    }
                },
                {
                    NodeType.ActorKill, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.ActorList,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ActorRemove, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.ActorList,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ActorChangeOwner, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.ActorList,
                            ConnectionType.Exec,
                            ConnectionType.Player
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ActorQueueMove, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.ActorList,
                            ConnectionType.Location,
                            ConnectionType.Integer,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ActorQueueAttack, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.ActorList,
                            ConnectionType.Location,
                            ConnectionType.Integer,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ActorQueueHunt, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.ActorList,
                            ConnectionType.Repeatable,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ActorQueueAttackMoveActivity, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.ActorList,
                            ConnectionType.Location,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ActorQueueSell, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.ActorList,
                            ConnectionType.Repeatable,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ActorQueueFindResources, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.ActorList,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.Reinforcements, new NodeContructor
                    {
                        ConstructorClass = typeof(FunctionNodeReinforcements),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Player,
                            ConnectionType.ActorInfoArray,
                            ConnectionType.CellPath,
                            ConnectionType.Integer,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.ActorList,
                            ConnectionType.Exec,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ReinforcementsWithTransport, new NodeContructor
                    {
                        ConstructorClass = typeof(FunctionNodeReinforcements),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Player,
                            ConnectionType.ActorInfo,
                            ConnectionType.ActorInfoArray,
                            ConnectionType.CellPath,
                            ConnectionType.CellPath,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.ActorList,
                            ConnectionType.Exec,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.UiPlayNotification, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.PlayerGroup,
                            ConnectionType.String,
                            ConnectionType.String,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.UiPlaySound, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Location,
                            ConnectionType.String,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.UiRadarPing, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Location,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.UiTextMessage, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.String,
                            ConnectionType.String,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.UiAddMissionText, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.String,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ArithmeticsOr, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec,
                            ConnectionType.Exec,
                            ConnectionType.Repeatable
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.ArithmeticsAnd, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec,
                            ConnectionType.Exec,
                            ConnectionType.Repeatable
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.CreateEffect, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Location,
                            ConnectionType.String,
                            ConnectionType.String,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.CompareActors, new NodeContructor
                    {
                        ConstructorClass = typeof(ArithmecCompareNode),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Universal,
                            ConnectionType.Universal
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Universal
                        }
                    }
                },
                {
                    NodeType.DoMultiple, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Integer,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.Count, new NodeContructor
                    {
                        ConstructorClass = typeof(GetCountNode),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Universal
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Integer
                        }
                    }
                },
                {
                    NodeType.ArithmeticsMath, new NodeContructor
                    {
                        ConstructorClass = typeof(ArithmeticMathNode),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Integer,
                            ConnectionType.Integer
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Integer
                        }
                    }
                },
                {
                    NodeType.UiNewObjective, new NodeContructor
                    {
                        ConstructorClass = typeof(UiObjectivesNode),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.String,
                            ConnectionType.Exec,
                            ConnectionType.Player,
                            ConnectionType.PlayerGroup
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Objective,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.UiCompleteObjective, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Objective,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.UiFailObjective, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Objective,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.FindActorsOnFootprint, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.CellArray,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.ActorList,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.FinActorsInCircle, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.LocationRange,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.ActorList,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.FilterActorGroup, new NodeContructor
                    {
                        ConstructorClass = typeof(FilterActorListByNode),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Player,
                            ConnectionType.ActorList,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.ActorList,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.CheckCondition, new NodeContructor
                    {
                        ConstructorClass = typeof(CheckConditionNode),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec,
                            ConnectionType.Condition
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.CompareActor, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.Actor
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Condition
                        }
                    }
                },
                {
                    NodeType.CompareActorInfo, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.ActorInfo,
                            ConnectionType.ActorInfo
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Condition
                        }
                    }
                },
                {
                    NodeType.IsAlive, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Condition
                        }
                    }
                },
                {
                    NodeType.IsDead, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Condition
                        }
                    }
                },
                {
                    NodeType.CompareNumber, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Integer,
                            ConnectionType.Integer
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Condition
                        }
                    }
                },
                {
                    NodeType.IsPlaying, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Player
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Condition
                        }
                    }
                },
                {
                    NodeType.IsBot, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Player
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Condition
                        }
                    }
                },
                {
                    NodeType.IsHumanPlayer, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Player
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Condition
                        }
                    }
                },
                {
                    NodeType.IsNoncombatant, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Player
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Condition
                        }
                    }
                },
                {
                    NodeType.HasWon, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Player
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Condition
                        }
                    }
                },
                {
                    NodeType.HasLost, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Player
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Condition
                        }
                    }
                },
                {
                    NodeType.GlobalLightning, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Integer,
                            ConnectionType.Integer,
                            ConnectionType.Integer,
                            ConnectionType.Integer,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.SetCameraPosition, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Player,
                            ConnectionType.Location,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.TimedExecution, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Integer,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec,
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.TextChoice, new NodeContructor
                    {
                        ConstructorClass = typeof(TextBoxSelectNode),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec,
                            ConnectionType.String
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.CameraRide, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Location,
                            ConnectionType.Location,
                            ConnectionType.Integer,
                            ConnectionType.Player,
                            ConnectionType.Exec,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                },
                {
                    NodeType.GetVariable, new NodeContructor
                    {
                        ConstructorClass = typeof(GetVariableNode),
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor
                        }
                    }
                },
                {
                    NodeType.SetVariable, new NodeContructor
                    {
                        ConstructorClass = typeof(SetVariableNode),
                        InConnections = new List<ConnectionType>
                        {
                            ConnectionType.Actor,
                            ConnectionType.Exec
                        },
                        OutConnections = new List<ConnectionType>
                        {
                            ConnectionType.Exec
                        }
                    }
                }
            };
    }
}