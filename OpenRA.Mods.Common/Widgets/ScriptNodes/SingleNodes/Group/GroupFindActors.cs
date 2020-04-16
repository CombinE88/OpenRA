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

                if (outCon.ActorGroup.Any())
                {
                    var oCon = OutConnections.FirstOrDefault(o => o.ConnectionTyp == ConnectionType.Exec);
                    if (oCon != null)
                        foreach (var node in IngameNodeScriptSystem.NodeLogics.Where(n =>
                            n.InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Exec) != null))
                        {
                            var inCon = node.InConnections.FirstOrDefault(c =>
                                c.ConnectionTyp == ConnectionType.Exec && c.In == oCon);
                            if (inCon != null)
                                inCon.Execute = true;
                        }
                }
            }
            else if (NodeType == NodeType.FindActorsOnFootprint)
            {
                var integ = InConnections.First(c => c.ConnectionTyp == ConnectionType.CellArray).In;

                if (integ == null)
                    throw new YamlException(NodeId + "Cell Array not connected");

                outCon.ActorGroup = world.Actors
                    .Where(a => !a.IsDead && a.IsInWorld && integ.CellArray.Contains(a.Location)).ToArray();

                if (outCon.ActorGroup.Any())
                {
                    var oCon = OutConnections.FirstOrDefault(o => o.ConnectionTyp == ConnectionType.Exec);
                    if (oCon != null)
                        foreach (var node in IngameNodeScriptSystem.NodeLogics.Where(n =>
                            n.InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Exec) != null))
                        {
                            var inCon = node.InConnections.FirstOrDefault(c =>
                                c.ConnectionTyp == ConnectionType.Exec && c.In == oCon);
                            if (inCon != null)
                                inCon.Execute = true;
                        }
                }
            }
        }
    }
}