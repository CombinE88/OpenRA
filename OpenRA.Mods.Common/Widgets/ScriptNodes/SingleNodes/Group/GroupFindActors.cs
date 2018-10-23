using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group
{
    public class GroupFindActorsLogic : NodeLogic
    {
        public GroupFindActorsLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            var outCon = OutConnections.First(c => c.ConTyp == ConnectionType.ActorList);

            if (NodeType == NodeType.FinActorsInCircle)
            {
                var integ = InConnections.First(c => c.ConTyp == ConnectionType.LocationRange).In;

                if (integ == null)
                    throw new YamlException(NodeId + "FindActorsInCircle Location and Range not connected");

                outCon.ActorGroup = world.FindActorsInCircle(world.Map.CenterOfCell(integ.Location.Value), WDist.FromCells(integ.Number.Value))
                    .Where(a => !a.IsDead && a.IsInWorld).ToArray();

                if (outCon.ActorGroup.Any())
                {
                    var oCon = OutConnections.FirstOrDefault(o => o.ConTyp == ConnectionType.Exec);
                    if (oCon != null)
                    {
                        foreach (var node in Insc.NodeLogics.Where(n => n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec) != null))
                        {
                            var inCon = node.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && c.In == oCon);
                            if (inCon != null)
                                inCon.Execute = true;
                        }
                    }
                }
            }
            else if (NodeType == NodeType.FindActorsOnFootprint)
            {
                var integ = InConnections.First(c => c.ConTyp == ConnectionType.CellArray).In;

                if (integ == null)
                    throw new YamlException(NodeId + "Cell Array not connected");

                outCon.ActorGroup = world.Actors.Where(a => !a.IsDead && a.IsInWorld && integ.CellArray.Contains(a.Location)).ToArray();

                if (outCon.ActorGroup.Any())
                {
                    var oCon = OutConnections.FirstOrDefault(o => o.ConTyp == ConnectionType.Exec);
                    if (oCon != null)
                    {
                        foreach (var node in Insc.NodeLogics.Where(n => n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec) != null))
                        {
                            var inCon = node.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && c.In == oCon);
                            if (inCon != null)
                                inCon.Execute = true;
                        }
                    }
                }
            }
        }
    }
}