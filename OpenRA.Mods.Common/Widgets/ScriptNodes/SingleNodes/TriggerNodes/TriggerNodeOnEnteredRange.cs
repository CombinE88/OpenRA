using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeOnEnteredRange : NodeWidget
    {
        public TriggerNodeOnEnteredRange(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class TriggerLogicOnEnteredRange : NodeLogic
    {
        bool triggerOnEnter;
        bool repeat;

        public TriggerLogicOnEnteredRange(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
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
            var boolean = InConnections.FirstOrDefault(ic => ic.ConTyp == ConnectionType.Repeatable);
            repeat = boolean.In != null;
        }

        public override void Tick(Actor self)
        {
            if (triggerOnEnter && !repeat)
                return;

            if (InConnections.First(ic => ic.ConTyp == ConnectionType.LocationRange).In == null
                || InConnections.First(ic => ic.ConTyp == ConnectionType.LocationRange).In.Location == null
                || InConnections.First(ic => ic.ConTyp == ConnectionType.LocationRange).In.Number == null)
                throw new YamlException(NodeId + "Location and Range not connected");

            if (InConnections.First(ic => ic.ConTyp == ConnectionType.PlayerGroup).In == null ||
                !InConnections.First(ic => ic.ConTyp == ConnectionType.PlayerGroup).In.PlayerGroup.Any())
                throw new YamlException(NodeId + "player Group not connected");

            var actors = self.World
                .FindActorsInCircle(self.World.Map.CenterOfCell(InConnections.First(ic => ic.ConTyp == ConnectionType.LocationRange).In.Location.Value),
                    WDist.FromCells(InConnections.First(ic => ic.ConTyp == ConnectionType.LocationRange).In.Number.Value))
                .Where(a => !a.IsDead
                            && a.IsInWorld
                            && a.TraitOrDefault<Mobile>() != null
                            && InConnections.First(ic => ic.ConTyp == ConnectionType.PlayerGroup).In.PlayerGroup.Contains(a.Owner.PlayerReference))
                .ToArray();

            if (triggerOnEnter && !repeat)
                return;

            if (actors.Any() && !triggerOnEnter)
            {
                var exeNodes = Insc.NodeLogics.Where(n =>
                    n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && OutConnections
                                                            .Where(t => t.ConTyp == ConnectionType.Exec).Contains(c.In)) != null);
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