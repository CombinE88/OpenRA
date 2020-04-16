using System.Linq;
using OpenRA.Mods.Common.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeOnEnteredFootPrint : NodeWidget
    {
        public TriggerNodeOnEnteredFootPrint(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen,
            nodeInfo)
        {
        }
    }

    public class TriggerLogicEnteredFootPrint : NodeLogic
    {
        bool repeat;
        bool triggerOnEnter;

        public TriggerLogicEnteredFootPrint(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
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

            if (InConnections.First(ic => ic.ConnectionTyp == ConnectionType.CellArray).In == null ||
                !InConnections.First(ic => ic.ConnectionTyp == ConnectionType.CellArray).In.CellArray.Any())
                throw new YamlException(NodeId + ": Cell Array not connected");

            if (InConnections.Any() &&
                (InConnections.First(ic => ic.ConnectionTyp == ConnectionType.PlayerGroup).In == null
                 || InConnections.First(ic => ic.ConnectionTyp == ConnectionType.PlayerGroup).In.PlayerGroup == null
                 || !InConnections.First(ic => ic.ConnectionTyp == ConnectionType.PlayerGroup).In.PlayerGroup.Any()))
                throw new YamlException(NodeId + "player Group not connected");

            var actors = self.World.Actors
                .Where(a => !a.IsDead
                            && a.IsInWorld
                            && a.TraitOrDefault<Mobile>() != null
                            && InConnections.First(ic => ic.ConnectionTyp == ConnectionType.PlayerGroup).In.PlayerGroup
                                .Contains(a.Owner.PlayerReference)
                            && InConnections.First(ic => ic.ConnectionTyp == ConnectionType.CellArray).In.CellArray
                                .Contains(a.Location))
                .ToArray();

            if (actors.Any() && !triggerOnEnter)
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