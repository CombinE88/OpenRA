using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes;
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
        readonly World world;
        public List<NodeInfo> NodeInfos = new List<NodeInfo>();
        public List<VariableInfo> VariableInfos = new List<VariableInfo>();

        public EditorNodeLayer(Actor self, EditorNodeLayerInfo info)
        {
            world = self.World;
        }

        public void WorldLoaded(World world, WorldRenderer wr)
        {
            if (world.Type != WorldType.Editor)
                return;

            foreach (var kv in world.Map.NodeDefinitions.Where(def => def.Key.Split('@').First() == "Variable"))
                AddVariables(kv);
            foreach (var kv in world.Map.NodeDefinitions.Where(def => def.Key.Split('@').First() != "Variable"))
                AddNodeInfos(kv);
        }

        void AddVariables(MiniYamlNode vars)
        {
            var infos = vars.Key.Split('@');
            var varType = infos.Last();

            var values = (VariableType[]) Enum.GetValues(typeof(VariableType));

            var varInfo = new VariableInfo
            {
                VariableName = varType,
                VarType = values.First(e => e.ToString() == vars.Value.Value)
            };

            VariableInfos.Add(varInfo);
        }

        void AddNodeInfos(MiniYamlNode nodes)
        {
            var infos = nodes.Key.Split('@');
            var nodeName = infos.First();
            var nodeId = infos.Last();

            var nodeTypes = (NodeType[]) Enum.GetValues(typeof(NodeType));
            
            var nodeType = nodeTypes.First(e => e.ToString() == nodes.Value.Value);

            var nodeInfo = new NodeInfo(nodeType, nodeId, nodeName);

            var inCons = new List<InConReference>();
            var outCons = new List<OutConReference>();

            var dict = nodes.Value.ToDictionary();
            foreach (var node in dict)
            {
                switch (node.Key)
                {
                    case "Pos":
                    {
                        int offsetX;
                        int offsetY;
                        int.TryParse(node.Value.Value.Split(',').First(), out offsetX);
                        int.TryParse(node.Value.Value.Split(',').Last(), out offsetY);
                        nodeInfo.OffsetPosX = offsetX;
                        nodeInfo.OffsetPosY = offsetY;
                        break;
                    }
                    case "Method":
                    {
                        var methodes = (CompareMethod[]) Enum.GetValues(typeof(CompareMethod));
                        nodeInfo.Method = methodes.First(e => e.ToString() == node.Value.Value);
                        break;
                    }
                    case "Item":
                    {
                        var item = (CompareItem[]) Enum.GetValues(typeof(CompareItem));
                        nodeInfo.Item = item.First(e => e.ToString() == node.Value.Value);
                        break;
                    }
                    case "VariableReference":
                        nodeInfo.VariableReference = node.Value.Value;
                        break;
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
                            var values = (ConnectionType[]) Enum.GetValues(typeof(ConnectionType));
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
                    var outConReference = new OutConReference();
                    outCons.Add(outConReference);

                    outConReference.ConnectionId = node.Key.Split('@').Last();

                    foreach (var outCon in node.Value.ToDictionary())
                    {
                        if (outCon.Key == "ConnectionType")
                        {
                            var values = (ConnectionType[]) Enum.GetValues(typeof(ConnectionType));
                            outConReference.ConTyp = values.First(e => e.ToString() == outCon.Value.Value);
                        }

                        if (outCon.Key.Contains("String")) outConReference.String = outCon.Value.Value;

                        if (outCon.Key.Contains("Strings")) outConReference.Strings = outCon.Value.Value.Split(',');

                        if (outCon.Key.Contains("Players"))
                        {
                            var playNames = outCon.Value.Value.Split(',');

                            outConReference.PlayerGroup = playNames.Select(playerName =>
                                world.WorldActor.Trait<EditorActorLayer>().Players.Players
                                    .First(p => p.Key == playerName).Value).ToArray();
                        }

                        if (outCon.Key.Contains("Player"))
                            outConReference.Player = world.WorldActor.Trait<EditorActorLayer>().Players.Players
                                .First(p => p.Key == outCon.Value.Value).Value;

                        if (outCon.Key.Contains("ActorInfo"))
                            outConReference.ActorInfo = world.Map.Rules.Actors[outCon.Value.Value];

                        if (outCon.Key.Contains("ActorInfos"))
                        {
                            var actorNames = outCon.Value.Value.Split(',');

                            outConReference.ActorInfos =
                                actorNames.Select(name => world.Map.Rules.Actors[name]).ToArray();
                        }

                        if (outCon.Key.Contains("Actor"))
                            outConReference.ActorPreview = world.WorldActor.Trait<EditorActorLayer>().Previews
                                .FirstOrDefault(prev => prev.ID == outCon.Value.Value);

                        if (outCon.Key.Contains("Actors"))
                        {
                            var actorIds = outCon.Value.Value.Split(',');
                            var prevList = world.WorldActor.Trait<EditorActorLayer>().Previews;

                            outConReference.ActorPreviews = actorIds
                                .Select(name => prevList.FirstOrDefault(a => a.ID == name))
                                .Where(actorRef => actorRef != null).ToArray();
                        }

                        if (outCon.Key.Contains("Location"))
                        {
                            var pos = outCon.Value.Value.Split(',');
                            var x = 0;
                            var y = 0;
                            int.TryParse(pos[0], out x);
                            int.TryParse(pos[1], out y);
                            outConReference.Location = new CPos(x, y);
                        }

                        if (outCon.Key.Contains("Num"))
                        {
                            var num = 0;
                            int.TryParse(outCon.Value.Value, out num);
                            outConReference.Number = num;
                        }

                        if (outCon.Key.Contains("Cells"))
                        {
                            var cells = outCon.Value.Value.Split('|');
                            foreach (var cell in cells)
                            {
                                var pos = cell.Split(',');
                                var x = 0;
                                var y = 0;
                                int.TryParse(pos[0], out x);
                                int.TryParse(pos[1], out y);
                                outConReference.CellArray.Add(new CPos(x, y));
                            }
                        }
                    }
                }
            }

            nodeInfo.OutConnectionsReference = outCons;
            nodeInfo.InConnectionsReference = inCons;

            NodeInfos.Add(nodeInfo);
        }

        public List<MiniYamlNode> Save()
        {
            var nodes = NodeInfos.Select(nodeInfo => new MiniYamlNode(nodeInfo.NodeName + "@" + nodeInfo.NodeId,
                nodeInfo.NodeType.ToString(), SaveEntries(nodeInfo))).ToList();
            nodes.AddRange(VariableInfos.Select(variableInfo =>
                new MiniYamlNode("Variable@" + variableInfo.VariableName, variableInfo.VarType.ToString())));

            return nodes;
        }

        List<MiniYamlNode> SaveEntries(NodeInfo nodeInfo)
        {
            var nodes = new List<MiniYamlNode>
            {
                new MiniYamlNode("Pos", nodeInfo.OffsetPosX + "," + nodeInfo.OffsetPosY)
            };

            if (nodeInfo.Method != null)
                nodes.Add(new MiniYamlNode("Methode", nodeInfo.Method.ToString()));

            if (nodeInfo.Item != null)
                nodes.Add(new MiniYamlNode("Item", nodeInfo.Item.ToString()));

            if (nodeInfo.VariableReference != null)
                nodes.Add(new MiniYamlNode("VariableReference", nodeInfo.VariableReference));

            nodes.AddRange(nodeInfo.OutConnectionsReference.Select(outCon =>
                new MiniYamlNode("Out@" + outCon.ConnectionId, "", OutConnections(outCon))));

            foreach (var inCon in nodeInfo.InConnectionsReference)
            {
                var miniNode = new List<MiniYamlNode>
                {
                    new MiniYamlNode("ConnectionType", inCon.ConTyp.ToString())
                };
                if (inCon.WidgetNodeReference != null)
                    miniNode.Add(new MiniYamlNode("Node@" + inCon.WidgetReferenceId, inCon.WidgetNodeReference));

                nodes.Add(new MiniYamlNode("In@" + inCon.ConnectionId, "", miniNode));
            }

            return nodes;
        }

        static List<MiniYamlNode> OutConnections(OutConReference outCon)
        {
            var nodes = new List<MiniYamlNode>();
            nodes.Add(new MiniYamlNode("ConnectionType", outCon.ConTyp.ToString()));

            if (outCon.String != null)
                nodes.Add(new MiniYamlNode("String", outCon.String));
            if (outCon.Player != null)
                nodes.Add(new MiniYamlNode("Player", outCon.Player.Name));

            if (outCon.PlayerGroup != null && outCon.PlayerGroup.Any())
            {
                var text = "";
                foreach (var play in outCon.PlayerGroup)
                    if (play == outCon.PlayerGroup.Last())
                        text += play.Name;
                    else
                        text += play.Name + ",";

                nodes.Add(new MiniYamlNode("Players", text));
            }

            if (outCon.ActorInfo != null)
                nodes.Add(new MiniYamlNode("ActorInfo", outCon.ActorInfo.Name));
            if (outCon.ActorInfos != null && outCon.ActorInfos.Any())
            {
                var text = "";
                foreach (var play in outCon.ActorInfos)
                    if (play == outCon.ActorInfos.Last())
                        text += play.Name;
                    else
                        text += play.Name + ",";

                nodes.Add(new MiniYamlNode("ActorInfos", text));
            }

            if (outCon.ActorPreview != null) nodes.Add(new MiniYamlNode("Actor", outCon.ActorPreview.ID));

            if (outCon.ActorPreviews != null && outCon.ActorPreviews.Any())
            {
                var text = "";
                foreach (var play in outCon.ActorPreviews)
                    if (play == outCon.ActorPreviews.Last())
                        text += play.ID;
                    else
                        text += play.ID + ",";

                nodes.Add(new MiniYamlNode("Actors", text));
            }

            if (outCon.Location != null)
                nodes.Add(new MiniYamlNode("Location", outCon.Location.Value.X + "," + outCon.Location.Value.Y));
            if (outCon.CellArray.Any())
            {
                var text = "";
                foreach (var cell in outCon.CellArray)
                    if (cell != outCon.CellArray.Last())
                        text += cell + "|";
                    else
                        text += cell.ToString();

                nodes.Add(new MiniYamlNode("Cells", text));
            }

            if (outCon.Number != null)
                nodes.Add(new MiniYamlNode("Num", outCon.Number.ToString()));
            if (outCon.Strings.Any())
            {
                var text = "";
                foreach (var str in outCon.Strings)
                    if (str != outCon.Strings.Last())
                        text += str + ",";
                    else
                        text += str;

                nodes.Add(new MiniYamlNode("Strings", text));
            }

            return nodes;
        }
    }

    public class VariableInfo
    {
        public Actor Actor = null;
        public Actor[] ActorGroup = null;
        public string ActorId = null;
        public string[] ActorIds = null;
        public ActorInfo ActorInfo = null;
        public ActorInfo[] ActorInfos = null;
        public EditorActorPreview ActorPreview = null;
        public EditorActorPreview[] ActorPreviews = null;
        public List<CPos> CellArray = new List<CPos>();
        public CPos? Location = null;
        public int? Number = null;
        public PlayerReference Player = null;
        public PlayerReference[] PlayerGroup = null;
        public TriggerLogicCreateTimer Timer = null;
        public string VariableName;
        public VariableType VarType;
    }

    public class NodeInfo
    {
        public List<InConReference> InConnectionsReference;
        public CompareItem? Item;

        public CompareMethod? Method;
        public string NodeId;
        public string NodeName;
        public NodeType NodeType;
        public int? OffsetPosX;
        public int? OffsetPosY;
        public List<OutConReference> OutConnectionsReference;
        public string VariableReference;

        public NodeInfo(
            NodeType nodeType,
            string nodeId,
            string nodeName)
        {
            NodeType = nodeType;
            NodeName = nodeName;
            NodeId = nodeId;
        }
    }

    public class OutConReference
    {
        public string ActorId = null;
        public string[] ActorIds = null;

        public ActorInfo ActorInfo;
        public ActorInfo[] ActorInfos;
        public EditorActorPreview ActorPreview;
        public EditorActorPreview[] ActorPreviews;
        public List<CPos> CellArray = new List<CPos>();
        public string ConnectionId;
        public ConnectionType ConTyp;
        public CPos? Location;
        public int? Number;
        public PlayerReference Player;
        public PlayerReference[] PlayerGroup;
        public string String;
        public string[] Strings = { };
    }

    public class InConReference
    {
        public string ConnectionId;
        public ConnectionType ConTyp;
        public string WidgetNodeReference;

        public string WidgetReferenceId;
    }

    public enum CompareItem
    {
        Health,
        Damage,
        Speed,
        LocationX,
        LocationY,
        Primary,
        Secondary,
        Owner,
        Building,
        Unit,
        Aircraft,
        ActorTypes,
        IsIdle
    }

    public enum CompareMethod
    {
        Max,
        Min,
        Divide,
        Multiply,
        Add,
        Subtract,
        All,
        PlayerIsPlaying,
        AliveActors,
        Contains,
        ContainsNot,
        True,
        False
    }
}