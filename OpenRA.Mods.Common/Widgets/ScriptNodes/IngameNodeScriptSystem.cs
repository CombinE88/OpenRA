using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
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
        readonly List<NodeInfo> nodesInfos = new List<NodeInfo>();
        public readonly List<VariableInfo> VariableInfos = new List<VariableInfo>();

        public readonly World World;
        bool initialized;
        public List<NodeLogic> NodeLogics = new List<NodeLogic>();
        int ticker;

        public WorldRenderer WorldRenderer;

        public IngameNodeScriptSystem(ActorInitializer init)
        {
            World = init.Self.World;
        }

        void ITick.Tick(Actor self)
        {
            if (!initialized)
                return;

            foreach (var node in NodeLogics)
            {
                node.Tick(self);
                node.ExecuteTick(self);
            }

            if (ticker == 3)
                foreach (var logic in NodeLogics.Where(l => l.NodeType == "TriggerWorldLoaded"))
                    logic.Execute(self.World);

            if (ticker < 4) ticker++;
        }

        public void WorldLoaded(World w, WorldRenderer wr)
        {
            WorldRenderer = wr;

            foreach (var kv in w.Map.NodeDefinitions.Where(def => def.Key.Split('@').First() == "Variable"))
                SetUpVariables(kv);

            foreach (var kv in w.Map.NodeDefinitions.Where(def => def.Key.Split('@').First() != "Variable"))
                AddNodeLogic(kv);

            NodeLogics = NodeLibrary.InitializeNodes(this, nodesInfos);

            foreach (var node in NodeLogics) node.AddOutConnectionReferences();

            foreach (var node in NodeLogics) node.AddInConnectionReferences();

            foreach (var node in NodeLogics) node.DoAfterConnections();

            initialized = true;
        }

        void SetUpVariables(MiniYamlNode vars)
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

        void AddNodeLogic(MiniYamlNode nodes)
        {
            var infos = nodes.Key.Split('@');
            var nodeName = infos.First();
            var nodeId = infos.Last();

            var nodeType = nodes.Value.Value;

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
                        nodeInfo.Method = node.Value.Value;
                        break;
                    }
                    case "Item":
                    {
                        nodeInfo.Item = node.Value.Value;
                        break;
                    }
                    case "VariableReference":
                        nodeInfo.VariableReference = node.Value.Value;
                        break;
                }

                if (node.Key.Contains("In@"))
                {
                    var inConReference = new InConReference();
                    inCons.Add(inConReference);

                    inConReference.ConnectionId = node.Key.Split('@').Last();

                    foreach (var inCon in node.Value.ToDictionary())
                    {
                        if (inCon.Key == "ConnectionType")
                        {
                            var values = (ConnectionType[]) Enum.GetValues(typeof(ConnectionType));
                            inConReference.ConTyp = values.First(e => e.ToString() == inCon.Value.Value);
                        }

                        if (inCon.Key.Contains("Node@"))
                        {
                            inConReference.WidgetNodeReference = inCon.Value.Value;
                            inConReference.WidgetReferenceId = inCon.Key.Split('@').Last();
                        }
                    }
                }
                else if (node.Key.Contains("Out@"))
                {
                    var outConReference = new OutConReference();
                    outCons.Add(outConReference);

                    outConReference.ConnectionId = node.Key.Split('@').Last();

                    foreach (var outConnection in node.Value.ToDictionary())
                    {
                        if (outConnection.Key == "ConnectionType")
                        {
                            var values = (ConnectionType[]) Enum.GetValues(typeof(ConnectionType));
                            outConReference.ConTyp = values.First(e => e.ToString() == outConnection.Value.Value);
                        }

                        if (outConnection.Key.Contains("String")) outConReference.String = outConnection.Value.Value;

                        if (outConnection.Key.Contains("Strings"))
                            outConReference.Strings = outConnection.Value.Value.Split(',');

                        if (outConnection.Key.Contains("Player"))
                        {
                            var player = World.Players.FirstOrDefault(p => p.InternalName == outConnection.Value.Value);
                            outConReference.Player = player != null ? player.PlayerReference : null;
                        }

                        if (outConnection.Key.Contains("Players"))
                        {
                            var playNames = outConnection.Value.Value.Split(',');

                            outConReference.PlayerGroup = playNames.Select(playname =>
                                World.Players.First(p => p.InternalName == playname).PlayerReference).ToArray();
                        }

                        if (outConnection.Key.Contains("ActorInfo"))
                            outConReference.ActorInfo = World.Map.Rules.Actors[outConnection.Value.Value];

                        if (outConnection.Key.Contains("ActorInfos"))
                        {
                            var actorNames = outConnection.Value.Value.Split(',');

                            outConReference.ActorInfos =
                                actorNames.Select(name => World.Map.Rules.Actors[name]).ToArray();
                        }

                        if (outConnection.Key.Contains("Actor")) outConReference.ActorId = outConnection.Value.Value;

                        if (outConnection.Key.Contains("Actors"))
                            outConReference.ActorIds = outConnection.Value.Value.Split(',');

                        if (outConnection.Key.Contains("Location"))
                        {
                            var pos = outConnection.Value.Value.Split(',');
                            var x = 0;
                            var y = 0;
                            int.TryParse(pos[0], out x);
                            int.TryParse(pos[1], out y);
                            outConReference.Location = new CPos(x, y);
                        }

                        if (outConnection.Key.Contains("Num"))
                        {
                            var num = 0;
                            int.TryParse(outConnection.Value.Value, out num);
                            outConReference.Number = num;
                        }

                        if (!outConnection.Key.Contains("Cells")) continue;
                        {
                            var cells = outConnection.Value.Value.Split('|');
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

            nodesInfos.Add(nodeInfo);
        }
    }
}