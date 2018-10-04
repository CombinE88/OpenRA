using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class EditorNodeLayerInfo : ITraitInfo
    {
        public object Create(ActorInitializer init)
        {
            return new EditorNodeLayer(init.Self, this);
        }
    }

    public class EditorNodeLayer : IWorldLoaded
    {
        public List<NodeInfo> NodeInfo = new List<NodeInfo>();
        World world;

        public EditorNodeLayer(Actor self, EditorNodeLayerInfo info)
        {
            world = self.World;
        }

        public void WorldLoaded(World world, WorldRenderer wr)
        {
            if (world.Type != WorldType.Editor)
                return;

            foreach (var kv in world.Map.NodeDefinitions)
                Add(kv);
        }

        void Add(MiniYamlNode nodes)
        {
            string[] infos = nodes.Key.Split('@');
            string nodeName = infos.First();
            string nodeID = infos.Last();

            NodeType[] nodeTypes = (NodeType[])Enum.GetValues(typeof(NodeType));
            NodeType nodeType = nodeTypes.First(e => e.ToString() == nodes.Value.Value);

            NodeInfo nodeInfo = new NodeInfo(nodeType, nodeID, nodeName);

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

                        if (outcon.Key.Contains("Players"))
                        {
                            var playNames = outcon.Value.Value.Split(',');
                            List<PlayerReference> list = new List<PlayerReference>();
                            foreach (var playname in playNames)
                            {
                                list.Add(world.WorldActor.Trait<EditorActorLayer>().Players.Players.First(p => p.Key == playname).Value);
                            }

                            outCon.PlayerGroup = list.ToArray();
                        }

                        if (outcon.Key.Contains("Player"))
                        {
                            outCon.Player = world.WorldActor.Trait<EditorActorLayer>().Players.Players.First(p => p.Key == outcon.Value.Value).Value;
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

            NodeInfo.Add(nodeInfo);
        }

        public List<MiniYamlNode> Save()
        {
            var nodes = new List<MiniYamlNode>();
            foreach (var nodeInfo in NodeInfo)
            {
                nodes.Add(new MiniYamlNode(nodeInfo.NodeName + "@" + nodeInfo.NodeID, nodeInfo.NodeType.ToString(), SaveEntries(nodeInfo)));
            }

            return nodes;
        }

        public List<MiniYamlNode> SaveEntries(NodeInfo nodeInfo)
        {
            var nodes = new List<MiniYamlNode>();
            nodes.Add(new MiniYamlNode("Pos", nodeInfo.OffsetPosX.ToString() + "," + nodeInfo.OffsetPosY.ToString()));
            foreach (var outCon in nodeInfo.OutConnections)
            {
                nodes.Add(new MiniYamlNode("Out@" + outCon.ConnectionId, "", OutConnections(outCon)));
            }

            foreach (var inCon in nodeInfo.InConnections)
            {
                List<MiniYamlNode> miniNode = new List<MiniYamlNode>();
                miniNode.Add(new MiniYamlNode("ConnectionType", inCon.ConTyp.ToString()));
                if (inCon.WidgetNodeReference != null)
                    miniNode.Add(new MiniYamlNode("Node@" + inCon.WidgetReferenceId, inCon.WidgetNodeReference));

                nodes.Add(new MiniYamlNode("In@" + inCon.ConnectionId, "", miniNode));
            }

            return nodes;
        }

        public List<MiniYamlNode> OutConnections(OutConReference outCon)
        {
            var nodes = new List<MiniYamlNode>();
            nodes.Add(new MiniYamlNode("ConnectionType", outCon.ConTyp.ToString()));

            if (outCon.String != null)
                nodes.Add(new MiniYamlNode("String", outCon.String));
            if (outCon.Player != null)
                nodes.Add(new MiniYamlNode("Player", outCon.Player.Name));
            if (outCon.PlayerGroup.Any())
            {
                string text = "";
                foreach (var play in outCon.PlayerGroup)
                {
                    if (play != outCon.PlayerGroup.Last())
                        text += play.Name;
                    else
                        text += play.Name + ",";
                }

                nodes.Add(new MiniYamlNode("Players", text));
            }

            if (outCon.ActorInfo != null)
                nodes.Add(new MiniYamlNode("ActorInfo", outCon.ActorInfo.Name));
            if (outCon.ActorInfos.Any())
            {
                string text = "";
                foreach (var play in outCon.ActorInfos)
                {
                    if (play != outCon.ActorInfos.Last())
                        text += play.Name;
                    else
                        text += play.Name + ",";
                }

                nodes.Add(new MiniYamlNode("ActorInfos", text));
            }

            if (outCon.Location != null)
                nodes.Add(new MiniYamlNode("Location", outCon.Location.Value.X + "," + outCon.Location.Value.Y));
            if (outCon.CellArray.Any())
            {
                var text = "";
                foreach (var cell in outCon.CellArray)
                {
                    if (cell != outCon.CellArray.Last())
                        text += cell.ToString() + "|";
                    else
                        text += cell.ToString();
                }

                nodes.Add(new MiniYamlNode("Cells", text));
            }

            if (outCon.Number != null)
                nodes.Add(new MiniYamlNode("Num", outCon.Number.ToString()));
            if (outCon.Strings.Any())
            {
                var text = "";
                foreach (var str in outCon.Strings)
                {
                    if (str != outCon.Strings.Last())
                        text += str + ",";
                    else
                        text += str;
                }

                nodes.Add(new MiniYamlNode("Strings", text));
            }

            return nodes;
        }
    }

    public class NodeInfo
    {
        public NodeType NodeType;
        public string NodeName;
        public string NodeID;
        public Nullable<int> OffsetPosX = null;
        public Nullable<int> OffsetPosY = null;
        public List<InConReference> InConnections = null;
        public List<OutConReference> OutConnections = null;

        public NodeInfo(
            NodeType nodeType,
            string nodeID,
            string nodeName)
        {
            NodeType = nodeType;
            NodeName = nodeName;
            NodeID = nodeID;
        }
    }

    public class OutConReference
    {
        public string ConnectionId;
        public ConnectionType ConTyp;

        public ActorInfo ActorInfo = null;
        public ActorInfo[] ActorInfos = null;
        public PlayerReference Player = null;
        public PlayerReference[] PlayerGroup = null;
        public Nullable<CPos> Location = null;
        public List<CPos> CellArray = new List<CPos>();
        public Nullable<int> Number = null;
        public string String = null;
        public string[] Strings = { };

        public OutConReference()
        {
        }
    }

    public class InConReference
    {
        public string ConnectionId;
        public ConnectionType ConTyp;

        public string WidgetReferenceId;
        public string WidgetNodeReference;

        public InConReference()
        {
        }
    }
}