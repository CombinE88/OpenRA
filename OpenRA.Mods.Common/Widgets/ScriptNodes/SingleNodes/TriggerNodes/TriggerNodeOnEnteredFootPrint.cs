using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeOnEnteredFootPrint : NodeWidget
    {
        public TriggerNodeOnEnteredFootPrint(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class TriggerLogicEnteredFoodPrint : NodeLogic
    {
        bool triggerOnEnter;
        bool repeat;

        public TriggerLogicEnteredFoodPrint(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void DoAfterConnections()
        {
            var boolean = InConnections.FirstOrDefault(ic => ic.ConTyp == ConnectionType.Repeatable);
            repeat = boolean.In != null;
        }

        public override void Tick(Actor self)
        {
            if (triggerOnEnter && !repeat)
                return;

            if (InConnections.First(ic => ic.ConTyp == ConnectionType.CellArray).In == null ||
                !InConnections.First(ic => ic.ConTyp == ConnectionType.CellArray).In.CellArray.Any())
                throw new YamlException(NodeId + ": Cell Array not connected");

            if (InConnections.Any() && (InConnections.First(ic => ic.ConTyp == ConnectionType.PlayerGroup).In == null
                                        || InConnections.First(ic => ic.ConTyp == ConnectionType.PlayerGroup).In.PlayerGroup == null
                                        || !InConnections.First(ic => ic.ConTyp == ConnectionType.PlayerGroup).In.PlayerGroup.Any()))
                throw new YamlException(NodeId + "player Group not connected");

            var actors = self.World.Actors
                .Where(a => !a.IsDead
                            && a.IsInWorld
                            && a.TraitOrDefault<Mobile>() != null
                            && InConnections.First(ic => ic.ConTyp == ConnectionType.PlayerGroup).In.PlayerGroup.Contains(a.Owner.PlayerReference)
                            && InConnections.First(ic => ic.ConTyp == ConnectionType.CellArray).In.CellArray.Contains(a.Location))
                .ToArray();

            if (actors.Any() && !triggerOnEnter)
            {
                var exeNodes = Insc.NodeLogics.Where(n =>
                    n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && OutConnections
                                                            .Where(t => t.ConTyp == ConnectionType.Exec).Contains(c.In)) != null);;
                foreach (var node in exeNodes)
                {
                    node.Execute(self.World);
                }

                triggerOnEnter = true;
            }
            else if (!actors.Any() && repeat)
            {
                triggerOnEnter = false;
            }
        }
    }
}