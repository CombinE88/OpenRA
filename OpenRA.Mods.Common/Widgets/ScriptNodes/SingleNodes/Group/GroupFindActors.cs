using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group
{
    public class GroupFindActorsLogic : NodeLogic
    {
        public GroupFindActorsLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            var outCon = OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList);

            if (NodeType == NodeType.FinActorsInCircle)
            {
                var integ = InConnections.First(c => c.ConnectionTyp == ConnectionType.LocationRange).In;

                if (integ == null)
                    throw new YamlException(NodeId + "FindActorsInCircle Location and Range not connected");

                outCon.ActorGroup = world.FindActorsInCircle(world.Map.CenterOfCell(integ.Location.Value),
                        WDist.FromCells(integ.Number.Value))
                    .Where(a => !a.IsDead && a.IsInWorld).ToArray();
            }
            else if (NodeType == NodeType.FindActorsOnFootprint)
            {
                var integ = InConnections.First(c => c.ConnectionTyp == ConnectionType.CellArray).In;

                if (integ == null)
                    throw new YamlException(NodeId + "Cell Array not connected");

                outCon.ActorGroup = world.Actors
                    .Where(a => !a.IsDead && a.IsInWorld && integ.CellArray.Contains(a.Location)).ToArray();
            }

            ForwardExec(this);
        }
    }
}