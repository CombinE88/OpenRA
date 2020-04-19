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
                    newNode.AddInConnection(new InConnection(connection.Item1, newNode));

            if (NodeConstructorInfo.NodeConstructorInfos[nodeType].OutConnections != null)
                foreach (var connection in NodeConstructorInfo.NodeConstructorInfos[nodeType].OutConnections)
                    newNode.AddOutConnection(new OutConnection(connection.Item1, newNode));

            return newNode;
        }

        public static List<NodeLogic> InitializeNodes(IngameNodeScriptSystem inss, List<NodeInfo> nodesInfos)
        {
            var nodeList = new List<NodeLogic>();

            foreach (var nodeInfo in nodesInfos)
            {
                var instanceType = NodeConstructorInfo.NodeConstructorInfos[nodeInfo.NodeType].LogicClass;

                var newLogicNode = (NodeLogic) instanceType
                    .GetConstructor(new[] {typeof(NodeEditorNodeScreenWidget), typeof(NodeInfo)})
                    .Invoke(new object[] {nodeInfo, inss});

                nodeList.Add(newLogicNode);
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
        public Type LogicClass;
        public List<Tuple<ConnectionType, string>> InConnections;
        public List<Tuple<ConnectionType, string>> OutConnections;
    }

    public static class NodeConstructorInfo
    {
        public static readonly Dictionary<NodeType, NodeContructor> NodeConstructorInfos =
            new Dictionary<NodeType, NodeContructor>
            {
                {
                    NodeType.MapInfoNode, new NodeContructor
                    {
                        ConstructorClass = typeof(MapInfoNode),
                        LogicClass = typeof(MapInfoLogicNode)
                    }
                },
                {
                    NodeType.MapInfoActorInfo, new NodeContructor
                    {
                        ConstructorClass = typeof(MapInfoActorInfoNode),
                        LogicClass = typeof(MapInfoLogicNode),

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfo, "Output: yaml name of choosen Actortype")
                        }
                    }
                },
                {
                    NodeType.MapInfoActorReference, new NodeContructor
                    {
                        ConstructorClass = typeof(MapInfoActorsonMap),
                        LogicClass = typeof(NodeLogic),

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, "")
                        }
                    }
                },
                {
                    NodeType.TriggerCreateTimer, new NodeContructor
                    {
                        ConstructorClass = typeof(TriggerNodeCreateTimer),
                        LogicClass = typeof(TriggerLogicCreateTimer),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Repeatable, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.TimerConnection, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.TimerReset, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(TimerLogics),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.TimerConnection, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.TimerStart, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(TimerLogics),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.TimerConnection, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.TimerStop, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(TimerLogics),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.TimerConnection, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.TriggerWorldLoaded, new NodeContructor
                    {
                        ConstructorClass = typeof(TriggerNodeWorldLoaded),
                        LogicClass = typeof(TriggerLogicWorldLoaded),

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.TriggerTick, new NodeContructor
                    {
                        ConstructorClass = typeof(TriggerNodeTick),
                        LogicClass = typeof(TriggerLogicTick),

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.TriggerOnEnteredFootprint, new NodeContructor
                    {
                        ConstructorClass = typeof(TriggerNodeOnEnteredFootPrint),
                        LogicClass = typeof(TriggerLogicEnteredFootPrint),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.PlayerGroup, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.CellArray, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Repeatable, ""),
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
                    NodeType.TriggerOnEnteredRange, new NodeContructor
                    {
                        ConstructorClass = typeof(TriggerNodeOnEnteredRange),
                        LogicClass = typeof(TriggerLogicOnEnteredRange),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.PlayerGroup, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Repeatable, ""),
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
                    NodeType.TriggerOnIdle, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(TriggerOnIdle),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Repeatable, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.TriggerOnKilled, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(TriggerOnKilled),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
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
                    NodeType.TriggerOnAllKilled, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(TriggerOnAllKilled),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Repeatable, ""),
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
                    NodeType.ActorCreateActor, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeCreateActor),
                        LogicClass = typeof(ActorCreateActorLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfo, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.ActorGetInformations, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(GetActorInformationsLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfo, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Player, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, "")
                        }
                    }
                },
                {
                    NodeType.GroupPlayerGroup, new NodeContructor
                    {
                        ConstructorClass = typeof(GroupPlayerGroup),
                        LogicClass = typeof(GroupPlayerLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.PlayerGroup, "")
                        }
                    }
                },
                {
                    NodeType.GroupActorGroup, new NodeContructor
                    {
                        ConstructorClass = typeof(GroupActorGroup),
                        LogicClass = typeof(GroupActorLogic),

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, "")
                        }
                    }
                },
                {
                    NodeType.GroupActorInfoGroup, new NodeContructor
                    {
                        ConstructorClass = typeof(GroupActorInfoGroup),
                        LogicClass = typeof(GroupActorInfoLogic),

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfoArray, "")
                        }
                    }
                },
                {
                    NodeType.ActorKill, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        LogicClass = typeof(ActorLogicQueueAbility),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.ActorRemove, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        LogicClass = typeof(ActorLogicQueueAbility),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.ActorChangeOwner, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        LogicClass = typeof(ActorLogicQueueAbility),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Player, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.ActorQueueMove, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        LogicClass = typeof(ActorLogicQueueAbility),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.ActorQueueAttack, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        LogicClass = typeof(ActorLogicQueueAbility),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.ActorQueueHunt, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        LogicClass = typeof(ActorLogicQueueAbility),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Repeatable, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.ActorQueueAttackMoveActivity, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        LogicClass = typeof(ActorLogicQueueAbility),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
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
                    NodeType.ActorQueueSell, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        LogicClass = typeof(ActorLogicQueueAbility),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Repeatable, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.ActorQueueFindResources, new NodeContructor
                    {
                        ConstructorClass = typeof(ActorNodeQueueAbility),
                        LogicClass = typeof(ActorLogicQueueAbility),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.Reinforcements, new NodeContructor
                    {
                        ConstructorClass = typeof(FunctionNodeReinforcements),
                        LogicClass = typeof(FunctionLogicReinforcements),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfoArray, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.CellPath, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.ReinforcementsWithTransport, new NodeContructor
                    {
                        ConstructorClass = typeof(FunctionNodeReinforcements),
                        LogicClass = typeof(FunctionLogicReinforcements),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfo, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfoArray, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.CellPath, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.CellPath, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.UiPlayNotification, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
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
                    NodeType.UiPlaySound, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
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
                    NodeType.UiRadarPing, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
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
                    NodeType.UiTextMessage, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
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
                    NodeType.UiAddMissionText, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
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
                    NodeType.ArithmeticsOr, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
                        LogicClass = typeof(ArithmeticBasicLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Repeatable, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.ArithmeticsAnd, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
                        LogicClass = typeof(ArithmeticBasicLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Repeatable, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.CreateEffect, new NodeContructor
                    {
                        ConstructorClass = typeof(UiNodeUiSettings),
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
                {
                    NodeType.CompareActors, new NodeContructor
                    {
                        ConstructorClass = typeof(ArithmecCompareNode),
                        LogicClass = typeof(ArithmecCompareLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Universal, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Universal, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Universal, "")
                        }
                    }
                },
                {
                    NodeType.DoMultiple, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(DoRepeatingNodeLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.Count, new NodeContructor
                    {
                        ConstructorClass = typeof(GetCountNode),
                        LogicClass = typeof(GetCountNodeLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Universal, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "")
                        }
                    }
                },
                {
                    NodeType.ArithmeticsMath, new NodeContructor
                    {
                        ConstructorClass = typeof(ArithmeticMathNode),
                        LogicClass = typeof(ArithmeticMathNodeLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "")
                        }
                    }
                },
                {
                    NodeType.UiNewObjective, new NodeContructor
                    {
                        ConstructorClass = typeof(UiObjectivesNode),
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
                {
                    NodeType.UiCompleteObjective, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(UiLogicUiSettings),

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
                    NodeType.UiFailObjective, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(UiLogicUiSettings),

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
                    NodeType.FindActorsOnFootprint, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(GroupFindActorsLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.CellArray, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.FinActorsInCircle, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(GroupFindActorsLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.LocationRange, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.FilterActorGroup, new NodeContructor
                    {
                        ConstructorClass = typeof(FilterActorListByNode),
                        LogicClass = typeof(FilterActorListByLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.CheckCondition, new NodeContructor
                    {
                        ConstructorClass = typeof(CheckConditionNode),
                        LogicClass = typeof(CheckConditionLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Condition, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.CompareActor, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(ProvideCondition),

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
                    NodeType.CompareActorInfo, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(ProvideCondition),

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
                    NodeType.IsAlive, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(ProvideCondition),

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
                    NodeType.IsDead, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(ProvideCondition),

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
                    NodeType.CompareNumber, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(ProvideCondition),

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
                    NodeType.IsPlaying, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(ProvideCondition),

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
                    NodeType.IsBot, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(ProvideCondition),

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
                    NodeType.IsHumanPlayer, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(ProvideCondition),

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
                    NodeType.IsNoncombatant, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(ProvideCondition),

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
                    NodeType.HasWon, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(ProvideCondition),

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
                    NodeType.HasLost, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(ProvideCondition),

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
                    NodeType.GlobalLightning, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(GlobalLightningNodeLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "Input Color: Red 0-256"),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "Input Color: Green 0-256"),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "Input Color: Blue 0-256"),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "Input Alpha: 0-256"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.SetCameraPosition, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(SetCameraPositionNode),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
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
                    NodeType.TimedExecution, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(TimedExecutionLogic),

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
                    NodeType.TextChoice, new NodeContructor
                    {
                        ConstructorClass = typeof(TextBoxSelectNode),
                        LogicClass = typeof(TextBoxSelectLogic),

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
                    NodeType.CameraRide, new NodeContructor
                    {
                        ConstructorClass = typeof(NodeWidget),
                        LogicClass = typeof(CameraRideNodeLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.GetVariable, new NodeContructor
                    {
                        ConstructorClass = typeof(GetVariableNode),
                        LogicClass = typeof(GetVariableLogic),

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, "")
                        }
                    }
                },
                {
                    NodeType.SetVariable, new NodeContructor
                    {
                        ConstructorClass = typeof(SetVariableNode),
                        LogicClass = typeof(SetVariableLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                }
            };
        
    }
}