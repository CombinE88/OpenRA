using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            foreach (var nodeInfo in nodeInfos)
            {
                Type choosenType;
                foreach (var type in Assembly.GetAssembly(typeof(BasicNodeWidget)).GetTypes().Where(type =>
                    type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeWidget))))
                {
                    var dictObject = type.GetField("NodeBuilder").GetValue(null);
                    var dictionary = (Dictionary<NodeType, BuildNodeConstructorInfo>) dictObject;

                    if (!dictionary.ContainsKey(nodeInfo.NodeType))
                        continue;

                    var createNode = (NodeWidget) type
                        .GetConstructor(new[] {typeof(NodeEditorNodeScreenWidget), typeof(NodeInfo)})
                        .Invoke(new object[] {nensw, nodeInfo});

                    nodes.Add(createNode);
                }
            }

            return nodes;
        }

        public static NodeWidget AddNode(NodeType nodeType, NodeEditorNodeScreenWidget nensw, string nodeId = null,
            string nodeName = null)
        {
            NodeWidget nodeWidget = null;
            var dictionary = new Dictionary<NodeType, BuildNodeConstructorInfo>();

            var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

            foreach (var type in Assembly.GetAssembly(typeof(BasicNodeWidget)).GetTypes().Where(type =>
                type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeWidget))))
            {
                var dictObject = type.GetField("NodeBuilder").GetValue(null);
                dictionary = (Dictionary<NodeType, BuildNodeConstructorInfo>) dictObject;

                if (!dictionary.ContainsKey(nodeType))
                    continue;

                if (dictionary[nodeType].WidgetType == typeof(TriggerNodeWorldLoaded) &&
                    nensw.Nodes.Any(n => n.GetType() == dictionary[nodeType].WidgetType))
                    continue;

                if (dictionary[nodeType].WidgetType == typeof(TriggerNodeTick) &&
                    nensw.Nodes.Any(n => n.GetType() == dictionary[nodeType].WidgetType))
                    continue;

                nodeWidget = (NodeWidget) type
                    .GetConstructor(new[] {typeof(NodeEditorNodeScreenWidget), typeof(NodeInfo)})
                    .Invoke(new object[] {nensw, nodeInfo});

                break;
            }

            if (nodeWidget != null && dictionary.Any())
            {
                if (!dictionary.Any())
                    return nodeWidget;

                var connectionDictionary = dictionary[nodeType];
                if (connectionDictionary.InConnections != null && connectionDictionary.InConnections.Any())
                    foreach (var connection in connectionDictionary.InConnections)
                        nodeWidget.AddInConnection(new InConnection(connection.Item1, nodeWidget));

                if (connectionDictionary.OutConnections != null && connectionDictionary.OutConnections != null)
                    foreach (var connection in connectionDictionary.OutConnections)
                        nodeWidget.AddOutConnection(new OutConnection(connection.Item1, nodeWidget));
            }

            return nodeWidget;
        }

        public static List<NodeLogic> InitializeNodes(IngameNodeScriptSystem inss, List<NodeInfo> nodeInfos)
        {
            var nodeList = new List<NodeLogic>();

            foreach (var nodeInfo in nodeInfos)
            {
                foreach (var type in Assembly.GetAssembly(typeof(NodeWidget)).GetTypes().Where(type =>
                    type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeWidget))))
                {
                    var dictObject = type.GetField("NodeBuilder").GetValue(null);
                    var dictionary = (Dictionary<NodeType, BuildNodeConstructorInfo>) dictObject;

                    if (!dictionary.ContainsKey(nodeInfo.NodeType) || dictionary[nodeInfo.NodeType].LogicClass == null)
                        continue;

                    var createNode = (NodeLogic) dictionary[nodeInfo.NodeType].LogicClass
                        .GetConstructor(new[] {typeof(NodeInfo), typeof(IngameNodeScriptSystem)})
                        .Invoke(new object[] {nodeInfo, inss});

                    nodeList.Add(createNode);
                }
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
}