using System;
using System.Collections.Generic;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.Library
{
    public class NodeLibrary
    {
        public NodeLibrary()
        {
        }

        public List<NodeWidget> LoadInNodes(NodeEditorNodeScreenWidget nensw, List<NodeInfo> nodeInfos)
        {
            List<NodeWidget> nodes = new List<NodeWidget>();
            foreach (var nodeinfo in nodeInfos)
            {
                if (nodeinfo.NodeType == NodeType.MapInfoNode)
                {
                    var newNode = new MapInfoNode(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerCreateTimer)
                {
                    var newNode = new TriggerNodeCreateTimer(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerWorldLoaded)
                {
                    var newNode = new TriggerNodeWorldLoaded(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerTick)
                {
                    var newNode = new TriggerNodeTick(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerOnEnteredFootprint)
                {
                    var newNode = new TriggerNodeOnEnteredFootPrint(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerOnEnteredRange)
                {
                    var newNode = new TriggerNodeOnEnteredRange(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorCreateActor)
                {
                    var newNode = new ActorNodeCreateActor(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorGetInformations)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.GroupPlayerGroup)
                {
                    var newNode = new GroupPlayerGroup(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueMove)
                {
                    var newNode = new ActorNodeQueueAbility(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueAttack)
                {
                    var newNode = new ActorNodeQueueAbility(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueHunt)
                {
                    var newNode = new ActorNodeQueueAbility(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueAttackMoveActivity)
                {
                    var newNode = new ActorNodeQueueAbility(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueSell)
                {
                    var newNode = new ActorNodeQueueAbility(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueFindResources)
                {
                    var newNode = new ActorNodeQueueAbility(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorKill)
                {
                    var newNode = new ActorNodeQueueAbility(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorRemove)
                {
                    var newNode = new ActorNodeQueueAbility(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorChangeOwner)
                {
                    var newNode = new ActorNodeQueueAbility(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.Reinforcements)
                {
                    var newNode = new FunctionNodeReinforcements(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ReinforcementsWithTransport)
                {
                    var newNode = new FunctionNodeReinforcements(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.GroupActorGroup)
                {
                    var newNode = new GroupActorGroup(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.GroupActorInfoGroup)
                {
                    var newNode = new GroupActorInfoGroup(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.MapInfoActorInfoNode)
                {
                    var newNode = new MapInfoActorInfoNode(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiPlayNotification)
                {
                    var newNode = new UiNodeUiSettings(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiPlaySound)
                {
                    var newNode = new UiNodeUiSettings(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiRadarPing)
                {
                    var newNode = new UiNodeUiSettings(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiTextMessage)
                {
                    var newNode = new UiNodeUiSettings(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiAddMissionText)
                {
                    var newNode = new UiNodeUiSettings(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.MapInfoActorInfoNode)
                {
                    var newNode = new MapInfoActorsonMap(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.MapInfoActorReference)
                {
                    var newNode = new MapInfoActorsonMap(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerOnIdle)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerOnKilled)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerOnAllKilled)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TimerReset)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TimerStart)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TimerStop)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ArithmeticsOr)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ArithmeticsAnd)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.CreateEffect)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.CompareActors)
                {
                    var newNode = new ArithmecCompareNode(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.DoMultiple)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.CountNode)
                {
                    var newNode = new GetCountNode(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ArithmeticsMath)
                {
                    var newNode = new ArithmeticMathNode(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiCompleteObjective)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiCompleteObjective)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.UiNewObjective)
                {
                    var newNode = new UiObjectivesNode(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.FindActorsOnFootprint)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.FinActorsInCircle)
                {
                    var newNode = new NodeWidget(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.FilterActorGroup)
                {
                    var newNode = new FilterActorListByNode(nensw, nodeinfo);
                    nodes.Add(newNode);
                }
            }

            return nodes;
        }

        public NodeWidget AddNode(NodeType nodeType, NodeEditorNodeScreenWidget nensw, string nodeId = null, string nodeName = null)
        {
            NodeWidget newNode = null;

            if (nodeType == NodeType.MapInfoNode)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);
                newNode = new MapInfoNode(nensw, nodeInfo);
            }
            else if (nodeType == NodeType.MapInfoActorInfoNode)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);
                newNode = new MapInfoActorInfoNode(nensw, nodeInfo);

                newNode.AddOutConnection(new OutConnection(ConnectionType.ActorInfo, newNode));
            }
            else if (nodeType == NodeType.MapInfoActorReference)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);
                newNode = new MapInfoActorsonMap(nensw, nodeInfo);

                newNode.AddOutConnection(new OutConnection(ConnectionType.Actor, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.ActorList, newNode));
            }
            else if (nodeType == NodeType.TriggerCreateTimer)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new TriggerNodeCreateTimer(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Repeatable, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.TimerConnection, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.TimerReset)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new NodeWidget(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.TimerConnection, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.TimerStart)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new NodeWidget(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.TimerConnection, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.TimerStop)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new NodeWidget(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.TimerConnection, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.TriggerWorldLoaded)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new TriggerNodeWorldLoaded(nensw, nodeInfo);

                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.TriggerTick)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new TriggerNodeTick(nensw, nodeInfo);

                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.TriggerOnEnteredFootprint)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new TriggerNodeOnEnteredFootPrint(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.PlayerGroup, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.CellArray, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Repeatable, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.TriggerOnEnteredRange)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new TriggerNodeOnEnteredRange(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.PlayerGroup, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Location, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Repeatable, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.TriggerOnIdle)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new NodeWidget(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Repeatable, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Actor, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.TriggerOnKilled)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new NodeWidget(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.TriggerOnAllKilled)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new NodeWidget(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.ActorList, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Repeatable, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ActorCreateActor)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeCreateActor(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.ActorInfo, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Player, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Location, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Actor, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ActorGetInformations)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeCreateActor(nensw, nodeInfo);

                newNode.AddOutConnection(new OutConnection(ConnectionType.ActorInfo, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Location, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Player, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
            }
            else if (nodeType == NodeType.GroupPlayerGroup)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new GroupPlayerGroup(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Player, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.PlayerGroup, newNode));
            }
            else if (nodeType == NodeType.GroupActorGroup)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new GroupActorGroup(nensw, nodeInfo);

                newNode.AddOutConnection(new OutConnection(ConnectionType.ActorList, newNode));
            }
            else if (nodeType == NodeType.GroupActorInfoGroup)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new GroupActorInfoGroup(nensw, nodeInfo);

                newNode.AddOutConnection(new OutConnection(ConnectionType.ActorInfoArray, newNode));
            }
            else if (nodeType == NodeType.ActorKill)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorList, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ActorRemove)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorList, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ActorChangeOwner)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorList, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Player, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ActorQueueMove)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorList, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorList, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Location, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ActorQueueAttack)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorList, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ActorQueueHunt)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorList, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Repeatable, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ActorQueueAttackMoveActivity)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorList, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Location, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ActorQueueSell)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorList, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Repeatable, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ActorQueueFindResources)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ActorNodeQueueAbility(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Actor, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorList, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.Reinforcements)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new FunctionNodeReinforcements(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Player, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorInfoArray, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.CellPath, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.ActorList, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ReinforcementsWithTransport)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new FunctionNodeReinforcements(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Player, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorInfo, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorInfoArray, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.CellPath, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.CellPath, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.ActorList, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.UiPlayNotification)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new UiNodeUiSettings(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.PlayerGroup, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.String, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.String, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.UiPlaySound)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new UiNodeUiSettings(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Location, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.String, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.UiRadarPing)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new UiNodeUiSettings(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Location, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.UiTextMessage)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new UiNodeUiSettings(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.String, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.String, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.UiAddMissionText)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new UiNodeUiSettings(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.String, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ArithmeticsOr)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new UiNodeUiSettings(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Repeatable, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.ArithmeticsAnd)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new UiNodeUiSettings(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Repeatable, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.CreateEffect)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new UiNodeUiSettings(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Location, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.String, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.String, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.CompareActors)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ArithmecCompareNode(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Universal, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Universal, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Universal, newNode));
            }
            else if (nodeType == NodeType.DoMultiple)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new NodeWidget(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.CountNode)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new GetCountNode(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Universal, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Integer, newNode));
            }
            else if (nodeType == NodeType.ArithmeticsMath)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new ArithmeticMathNode(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Integer, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Integer, newNode));
            }
            else if (nodeType == NodeType.UiNewObjective)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new UiObjectivesNode(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.String, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Player, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.PlayerGroup, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Objective, newNode));
            }
            else if (nodeType == NodeType.UiCompleteObjective)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new NodeWidget(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Objective, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.UIFailObjective)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new NodeWidget(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Objective, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.FindActorsOnFootprint)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new NodeWidget(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.CellArray, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.ActorList, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.FinActorsInCircle)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new NodeWidget(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.LocationRange, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.ActorList, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }
            else if (nodeType == NodeType.FilterActorGroup)
            {
                var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

                newNode = new FilterActorListByNode(nensw, nodeInfo);

                newNode.AddInConnection(new InConnection(ConnectionType.Player, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.ActorInfoArray, newNode));
                newNode.AddInConnection(new InConnection(ConnectionType.Exec, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.ActorList, newNode));
                newNode.AddOutConnection(new OutConnection(ConnectionType.Exec, newNode));
            }

            return newNode;
        }

        public List<NodeLogic> InitializeNodes(IngameNodeScriptSystem inss, List<NodeInfo> nodesInfos)
        {
            var nodeList = new List<NodeLogic>();
            foreach (var nodeinfo in nodesInfos)
            {
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
                    var newNode = new TriggerLogicEnteredFoodPrint(nodeinfo, inss);
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
                else if (nodeinfo.NodeType == NodeType.MapInfoActorInfoNode)
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
                else if (nodeinfo.NodeType == NodeType.MapInfoActorInfoNode)
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
                else if (nodeinfo.NodeType == NodeType.CountNode)
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
                else if (nodeinfo.NodeType == NodeType.UIFailObjective)
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
            }

            return nodeList;
        }
    }

    public enum NodeType
    {
        // MapInfo
        MapInfoNode,
        MapInfoActorInfoNode,
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
        CountNode,
        ArithmeticsMath,

        // Complex Functions
        Reinforcements,
        ReinforcementsWithTransport,
        CreateEffect,

        // UI Nodes
        UiPlayNotification,
        UiPlaySound,
        UiRadarPing,
        UiTextMessage,
        UiAddMissionText,
        UiNewObjective,
        UiCompleteObjective,
        UIFailObjective
    }
}