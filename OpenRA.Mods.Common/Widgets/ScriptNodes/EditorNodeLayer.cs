using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
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

    public class EditorNodeLayer
    {
        public List<BasicNodeWidget> SimpleNodeWidgets = new List<BasicNodeWidget>();
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
            var dict = nodes.Value.ToDictionary();
            foreach (var node in dict)
            {
                var posX = node.Key == "PosX" ? node.Value.Value : "";
                var posY = node.Key == "PosY" ? node.Value.Value : "";
                var offsetX = node.Key == "OffsetX" ? node.Value.Value : "";
                var offsetY = node.Key == "OffsetY" ? node.Value.Value : "";

                var inCons = new List<InConReference>();
                var outCons = new List<OutConReference>();

                if (node.Key.Contains("In@"))
                {
                    var inCon = new InConReference();
                    inCons.Add(inCon);

                    foreach (var incon in node.Value.ToDictionary())
                    {
                        if (incon.Key == "NodeType")
                        {
                            //// TODO: use correct methode when knowing them adding the connectionType
                        }

                        if (incon.Key.Contains("Node"))
                        {
                            inCon.WidgetName = incon.Key;
                        }

                        if (incon.Key.Contains("Out"))
                        {
                            inCon.ConnecitonName = incon.Value.Value;
                            inCon.OutName = incon.Key;
                        }
                    }
                }
                else if (node.Key.Contains("Out@"))
                {
                    var outCon = new OutConReference();
                    outCons.Add(outCon);

                    foreach (var outcon in node.Value.ToDictionary())
                    {
                        if (outcon.Key == "NodeType")
                        {
                            //// TODO: use correct methode when knowing them adding the connectionType
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
                            outCon.Player = world.WorldActor.Trait<EditorActorLayer>().Players.Players.First(p => p.Key == outcon.Value.Value).Value;
                        }

                        if (outcon.Key.Contains("ActorInfo"))
                        {
                            outCon.ActorInfo = world.Map.Rules.Actors[outcon.Value.Value];
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
        }

        public List<MiniYamlNode> Save()
        {
            var nodes = new List<MiniYamlNode>();
            foreach (var snw in SimpleNodeWidgets)
            {
                nodes.Add(new MiniYamlNode(snw.ToString().Split('.').Last().Replace("Widget", "Node"), snw.NodeName, SaveEntries(snw)));
            }

            return nodes;
        }

        public List<MiniYamlNode> SaveEntries(BasicNodeWidget snw)
        {
            var nodes = new List<MiniYamlNode>();
            nodes.Add(new MiniYamlNode("PosX", snw.PosX.ToString()));
            nodes.Add(new MiniYamlNode("PosY", snw.PosY.ToString()));
            nodes.Add(new MiniYamlNode("OffsetX", snw.OffsetPosX.ToString()));
            nodes.Add(new MiniYamlNode("OffsetY", snw.OffsetPosY.ToString()));

            foreach (var outCon in snw.OutConnections)
            {
                nodes.Add(new MiniYamlNode("Out@" + outCon.ConnecitonName, outCon.ConnecitonName, OutConnections(outCon)));
            }

            foreach (var inCon in snw.InConnections)
            {
                List<MiniYamlNode> miniNode = new List<MiniYamlNode>();
                miniNode.Add(new MiniYamlNode("NodeType", inCon.conTyp.ToString()));
                if (inCon.In != null)
                    miniNode.Add(new MiniYamlNode(inCon.In.Widget.NodeName, inCon.In.ConnecitonName));

                nodes.Add(new MiniYamlNode("In@" + inCon.ConnecitonName, inCon.ConnecitonName, miniNode));
            }

            return nodes;
        }

        public List<MiniYamlNode> OutConnections(OutConnection outCon)
        {
            var nodes = new List<MiniYamlNode>();
            nodes.Add(new MiniYamlNode("NodeType", outCon.conTyp.ToString()));

            if (outCon.String != null)
                nodes.Add(new MiniYamlNode("String", outCon.String));
            if (outCon.Player != null)
                nodes.Add(new MiniYamlNode("Player", outCon.Player.Name));
            if (outCon.ActorInfo != null)
                nodes.Add(new MiniYamlNode("ActorInfo", outCon.ActorInfo.Name));
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

            if (outCon.Number != 0)
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
        public Nullable<int> PosX = null;
        public Nullable<int> PosY = null;
        public Nullable<int> OffsetPosX = null;
        public Nullable<int> OffsetPosY = null;
        public List<InConnection> InConnections = null;
        public List<OutConnection> OutConnections = null;

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
        public string ConnecitonName;
        public ConnectionType conTyp;
        public string WidgetName;

        public bool Boolean = false;
        public ActorInfo ActorInfo = null;
        public PlayerReference Player = null;
        public CPos Location = CPos.Zero;
        public List<CPos> CellArray = new List<CPos>();
        public int Number = 0;
        public string String = null;
        public string[] Strings = { };

        public OutConReference()
        {
        }
    }

    public class InConReference
    {
        public string ConnecitonName;
        public ConnectionType conTyp;
        public string WidgetName;

        public string OutName;

        public InConReference()
        {
        }
    }
}