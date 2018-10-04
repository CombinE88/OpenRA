using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Scripting;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes
{
    public class FunctionNodeReinforcements : NodeWidget
    {
        public FunctionNodeReinforcements(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class FunctionLogicReinforcements : NodeLogic
    {
        public FunctionLogicReinforcements(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            if (InConnections.First(c => c.ConTyp == ConnectionType.Player).In == null || InConnections.First(c => c.ConTyp == ConnectionType.Player).In.Player == null)
                throw new YamlException(NodeId + "Reinforcement Player not connected");

            if (InConnections.First(c => c.ConTyp == ConnectionType.ActorInfos).In == null || InConnections.First(c => c.ConTyp == ConnectionType.ActorInfos).In.ActorInfos == null ||
                !InConnections.First(c => c.ConTyp == ConnectionType.ActorInfos).In.ActorInfos.Any())
                throw new YamlException(NodeId + "Reinforcement ActorGroup not connected or empty");


            if (InConnections.First(c => c.ConTyp == ConnectionType.CellPath).In == null ||
                InConnections.First(c => c.ConTyp == ConnectionType.CellPath).In.CellArray == null ||
                !InConnections.First(c => c.ConTyp == ConnectionType.CellPath).In.CellArray.Any())
                throw new YamlException(NodeId + "Reinforcement Entry Path not connected or empty");

            if (NodeType == NodeType.Reinforcements)
            {

                List<string> actors = new List<string>();
                foreach (var act in InConnections.First(c => c.ConTyp == ConnectionType.ActorInfos).In.ActorInfos)
                {
                    actors.Add(act.Name);
                }

                var inNumber = InConnections.First(c => c.ConTyp == ConnectionType.Integer).In.Number;

                new ReinforcementsGlobal(null).Reinforce(
                    world.Players.First(p => p.InternalName == InConnections.First(c => c.ConTyp == ConnectionType.Player).In.Player.Name),
                    actors.ToArray(),
                    InConnections.First(c => c.ConTyp == ConnectionType.CellPath).In.CellArray.ToArray(),
                    inNumber != null ? inNumber.Value : 25);
            }

            if (NodeType == NodeType.ReinforcementsWithTransport)
            {
                if (InConnections.Last(c => c.ConTyp == ConnectionType.CellPath).In == null ||
                    InConnections.Last(c => c.ConTyp == ConnectionType.CellPath).In.CellArray == null ||
                    !InConnections.Last(c => c.ConTyp == ConnectionType.CellPath).In.CellArray.Any())
                    throw new YamlException(NodeId + "Reinforcement Exit Path not connected or empty");

                if (InConnections.First(c => c.ConTyp == ConnectionType.ActorInfo).In == null || InConnections.First(c => c.ConTyp == ConnectionType.ActorInfo).In.ActorInfo == null)
                    throw new YamlException(NodeId + "Reinforcement Player not connected");

                List<string> actors = new List<string>();
                foreach (var act in InConnections.First(c => c.ConTyp == ConnectionType.ActorInfos).In.ActorInfos)
                {
                    actors.Add(act.Name);
                }

                new ReinforcementsGlobal(null).ReinforceWithTransport(
                    world.Players.First(p => p.InternalName == InConnections.First(c => c.ConTyp == ConnectionType.Player).In.Player.Name),
                    InConnections.First(c => c.ConTyp == ConnectionType.ActorInfo).In.ActorInfo.Name,
                    actors.ToArray(),
                    InConnections.First(c => c.ConTyp == ConnectionType.CellPath).In.CellArray.ToArray(),
                    InConnections.Last(c => c.ConTyp == ConnectionType.CellPath).In.CellArray.ToArray());
            }
        }
    }
}