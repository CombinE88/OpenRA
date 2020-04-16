using System.Linq;
using OpenRA.Mods.Common.Traits;

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
        bool repeat;
        bool triggerOnEnter;

        public TriggerLogicOnEnteredRange(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
            nodeInfo, ingameNodeScriptSystem)
        {
        }

        public override void DoAfterConnections()
        {
            var boolean = InConnections.FirstOrDefault(ic => ic.ConnectionTyp == ConnectionType.Repeatable);
            repeat = boolean.In != null;
        }

        public override void Tick(Actor self)
        {
            if (triggerOnEnter && !repeat)
                return;

            if (InConnections.First(ic => ic.ConnectionTyp == ConnectionType.LocationRange).In == null
                || InConnections.First(ic => ic.ConnectionTyp == ConnectionType.LocationRange).In.Location == null
                || InConnections.First(ic => ic.ConnectionTyp == ConnectionType.LocationRange).In.Number == null)
                throw new YamlException(NodeId + "Location and Range not connected");

            if (InConnections.First(ic => ic.ConnectionTyp == ConnectionType.PlayerGroup).In == null ||
                !InConnections.First(ic => ic.ConnectionTyp == ConnectionType.PlayerGroup).In.PlayerGroup.Any())
                throw new YamlException(NodeId + "player Group not connected");

            var actors = self.World
                .FindActorsInCircle(
                    self.World.Map.CenterOfCell(InConnections
                        .First(ic => ic.ConnectionTyp == ConnectionType.LocationRange).In.Location.Value),
                    WDist.FromCells(InConnections.First(ic => ic.ConnectionTyp == ConnectionType.LocationRange).In
                        .Number.Value))
                .Where(a => !a.IsDead
                            && a.IsInWorld
                            && a.TraitOrDefault<Mobile>() != null
                            && InConnections.First(ic => ic.ConnectionTyp == ConnectionType.PlayerGroup).In.PlayerGroup
                                .Contains(a.Owner.PlayerReference))
                .ToArray();

            if (!triggerOnEnter && actors.Any())
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

                triggerOnEnter = true;
            }
            else if (!actors.Any() && repeat)
            {
                triggerOnEnter = false;
            }
        }
    }
}