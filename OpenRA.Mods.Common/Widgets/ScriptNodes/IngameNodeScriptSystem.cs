using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class IngameNodeScriptSystemInfo : ITraitInfo
    {
        public object Create(ActorInitializer init)
        {
            return new IngameNodeScriptSystem(init);
        }
    }

    public class IngameNodeScriptSystem : IWorldLoaded, ITick
    {
        public List<NodeLogic> NodeLogics = new List<NodeLogic>();
        List<NodeInfo> nodesInfos = new List<NodeInfo>();

        World world;
        bool initialized = false;

        public IngameNodeScriptSystem(ActorInitializer init)
        {
            world = init.Self.World;
        }

        public void WorldLoaded(World w, WorldRenderer wr)
        {
            foreach (var kv in w.Map.NodeDefinitions)
                Add(kv);

            InitializeNodes();

            initialized = true;

            foreach (var logic in NodeLogics.Where(l => l.NodeType == NodeType.TriggerWorldLoaded))
            {
                logic.Execute(w);
            }
        }

        void Add(MiniYamlNode nodes)
        {
            string[] infos = nodes.Key.Split('@');
            string nodeName = infos.First();
            string nodeId = infos.Last();

            NodeType[] nodeTypes = (NodeType[])Enum.GetValues(typeof(NodeType));
            NodeType nodeType = nodeTypes.First(e => e.ToString() == nodes.Value.Value);

            NodeInfo nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

            var inCons = new List<InConReference>();
            var outCons = new List<OutConReference>();

            var dict = nodes.Value.ToDictionary();
            foreach (var node in dict)
            {
                if (node.Key == "Pos")
                {
                    int offsetX;
                    int offsetY;
                    int.TryParse(node.Value.Value.Split(',').First(), out offsetX);
                    int.TryParse(node.Value.Value.Split(',').Last(), out offsetY);
                    nodeInfo.OffsetPosX = offsetX;
                    nodeInfo.OffsetPosY = offsetY;
                }


                if (node.Key.Contains("In@"))
                {
                    var inCon = new InConReference();
                    inCons.Add(inCon);

                    inCon.ConnectionId = node.Key.Split('@').Last();

                    foreach (var incon in node.Value.ToDictionary())
                    {
                        if (incon.Key == "ConnectionType")
                        {
                            ConnectionType[] values = (ConnectionType[])Enum.GetValues(typeof(ConnectionType));
                            inCon.ConTyp = values.First(e => e.ToString() == incon.Value.Value);
                        }

                        if (incon.Key.Contains("Node@"))
                        {
                            inCon.WidgetNodeReference = incon.Value.Value;
                            inCon.WidgetReferenceId = incon.Key.Split('@').Last();
                        }
                    }
                }
                else if (node.Key.Contains("Out@"))
                {
                    var outCon = new OutConReference();
                    outCons.Add(outCon);

                    outCon.ConnectionId = node.Key.Split('@').Last();

                    foreach (var outcon in node.Value.ToDictionary())
                    {
                        if (outcon.Key == "ConnectionType")
                        {
                            ConnectionType[] values = (ConnectionType[])Enum.GetValues(typeof(ConnectionType));
                            outCon.ConTyp = values.First(e => e.ToString() == outcon.Value.Value);
                        }

                        if (outcon.Key.Contains("String"))
                        {
                            outCon.String = outcon.Value.Value;
                        }

                        if (outcon.Key.Contains("Strings"))
                        {
                            outCon.Strings = outcon.Value.Value.Split(',');
                        }

                        if (outcon.Key.Contains("Player"))
                        {
                            var player = world.Players.FirstOrDefault(p => p.InternalName == outcon.Value.Value);
                            outCon.Player = player != null ? player.PlayerReference : null;
                        }

                        if (outcon.Key.Contains("Players"))
                        {
                            var playNames = outcon.Value.Value.Split(',');
                            List<PlayerReference> list = new List<PlayerReference>();
                            foreach (var playname in playNames)
                            {
                                list.Add(world.Players.First(p => p.InternalName == playname).PlayerReference);
                            }

                            outCon.PlayerGroup = list.ToArray();
                        }

                        if (outcon.Key.Contains("ActorInfo"))
                        {
                            outCon.ActorInfo = world.Map.Rules.Actors[outcon.Value.Value];
                        }

                        if (outcon.Key.Contains("ActorInfos"))
                        {
                            var actorNames = outcon.Value.Value.Split(',');
                            List<ActorInfo> actorList = new List<ActorInfo>();
                            foreach (var name in actorNames)
                            {
                                var actorRef = world.Map.Rules.Actors[name];
                                actorList.Add(actorRef);
                            }

                            outCon.ActorInfos = actorList.ToArray();
                        }

                        if (outcon.Key.Contains("Location"))
                        {
                            string[] pos = outcon.Value.Value.Split(',');
                            var x = 0;
                            var y = 0;
                            int.TryParse(pos[0], out x);
                            int.TryParse(pos[1], out y);
                            outCon.Location = new CPos(x, y);
                        }

                        if (outcon.Key.Contains("Num"))
                        {
                            int num = 0;
                            int.TryParse(outcon.Value.Value, out num);
                            outCon.Number = num;
                        }

                        if (outcon.Key.Contains("Cells"))
                        {
                            string[] cells = outcon.Value.Value.Split('|');
                            foreach (var cell in cells)
                            {
                                string[] pos = cell.Split(',');
                                var x = 0;
                                var y = 0;
                                int.TryParse(pos[0], out x);
                                int.TryParse(pos[1], out y);
                                outCon.CellArray.Add(new CPos(x, y));
                            }
                        }
                    }
                }
            }

            nodeInfo.OutConnections = outCons;
            nodeInfo.InConnections = inCons;

            nodesInfos.Add(nodeInfo);
        }

        public void InitializeNodes()
        {
            foreach (var nodeinfo in nodesInfos)
            {
                //  Info Logics
                if (nodeinfo.NodeType == NodeType.MapInfoNode)
                {
                    var newNode = new MapInfoLogicNode(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }

                // Trigger Logics
                else if (nodeinfo.NodeType == NodeType.TriggerWorldLoaded)
                {
                    var newNode = new TriggerLogicWorldLoaded(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerTick)
                {
                    var newNode = new TriggerLogicTick(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerOnEnteredFootprint)
                {
                    var newNode = new TriggerLogicEnteredFoodPrint(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.TriggerCreateTimer)
                {
                    var newNode = new TriggerLogicCreateTimer(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }

                //  Group Logics
                else if (nodeinfo.NodeType == NodeType.GroupPlayerGroup)
                {
                    var newNode = new GroupPlayerLogic(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.GroupActorGroup)
                {
                    var newNode = new GroupActorLogic(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.GroupActorInfoGroup)
                {
                    var newNode = new GroupActorInfoLogic(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }

                //  Actor Logics
                else if (nodeinfo.NodeType == NodeType.ActorCreateActor)
                {
                    var newNode = new ActorCreateActorLogic(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueMove)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueAttack)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueHunt)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueAttackMoveActivity)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueSell)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorQueueFindResources)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorKill)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ActorRemove)
                {
                    var newNode = new ActorLogicQueueAbility(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.Reinforcements)
                {
                    var newNode = new FunctionLogicReinforcements(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
                else if (nodeinfo.NodeType == NodeType.ReinforcementsWithTransport)
                {
                    var newNode = new FunctionLogicReinforcements(nodeinfo, this);
                    NodeLogics.Add(newNode);
                }
            }

            foreach (var node in NodeLogics)
            {
                node.AddOutConnectionReferences();
            }

            foreach (var node in NodeLogics)
            {
                node.AddInConnectionReferences();
            }

            foreach (var node in NodeLogics)
            {
                node.DoAfterConnections();
            }
        }

        public void Tick(Actor self)
        {
            if (!initialized)
                return;

            foreach (var node in NodeLogics)
            {
                node.Tick(self);
            }
        }
    }
}