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
            InConTexts.Add("Player Group");
            InConTexts.Add("Footprint Cells");
            InConTexts.Add("Repeatable");
        }
    }

    public class TriggerLogicEnteredFoodPrint : NodeLogic
    {
        bool triggerOnEnter;
        bool repeat;

        public TriggerLogicEnteredFoodPrint(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            var exeNodes = Insc.NodeLogics.Where(n => n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && c.In == OutConnections.First()) != null);
            foreach (var node in exeNodes)
            {
                node.Execute(world);
            }
        }

        public override void DoAfterConnections()
        {
            var boolean = InConnections.FirstOrDefault(ic => ic.ConTyp == ConnectionType.Boolean);
            repeat = boolean.In != null;
        }

        public override void Tick(Actor self)
        {
            if ((triggerOnEnter && !repeat))
                return;

            var actors = self.World.Actors
                .Where(a => !a.IsDead
                            && a.IsInWorld
                            && a.TraitOrDefault<Mobile>() != null
                            && InConnections.First(ic => ic.ConTyp == ConnectionType.PlayerGroup).In.PlayerGroup.Contains(a.Owner.PlayerReference)
                            && InConnections.First(ic => ic.ConTyp == ConnectionType.CellArray).In.CellArray.Contains(a.Location))
                .ToArray();

            if (triggerOnEnter && !repeat)
                return;

            if (actors.Any() && !triggerOnEnter)
            {
                triggerOnEnter = true;
                var exeNodes = Insc.NodeLogics.Where(n => n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && c.In == OutConnections.First()) != null);
                foreach (var node in exeNodes)
                {
                    node.Execute(self.World);
                }
            }
            else if (!actors.Any())
            {
                triggerOnEnter = false;
            }
        }
    }
}