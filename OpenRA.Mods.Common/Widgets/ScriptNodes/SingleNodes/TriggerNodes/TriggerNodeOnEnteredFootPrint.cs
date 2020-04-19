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
        bool enabled;

        public TriggerLogicEnteredFootPrint(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
            nodeInfo, ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            enabled = true;
            ForwardExec(this, 1);
        }

        public override void DoAfterConnections()
        {
            var boolean = InConnections.FirstOrDefault(ic => ic.ConnectionTyp == ConnectionType.Repeatable);
            repeat = boolean.In != null;
        }

        public override void Tick(Actor self)
        {
            if (!enabled || triggerOnEnter && !repeat)
                return;

            var cellArray = GetLinkedConnectionFromInConnection(this, InConnections, ConnectionType.TimerConnection, 0);
            var playerGroup =
                GetLinkedConnectionFromInConnection(this, InConnections, ConnectionType.TimerConnection, 0);

            if (!cellArray.CellArray.Any() || !playerGroup.PlayerGroup.Any())
                return;

            var actors = self.World.Actors
                .Where(a => !a.IsDead
                            && a.IsInWorld
                            && a.TraitOrDefault<Mobile>() != null
                            && playerGroup.PlayerGroup
                                .Contains(a.Owner.PlayerReference)
                            && cellArray.CellArray
                                .Contains(a.Location))
                .ToArray();

            if (actors.Any() && !triggerOnEnter)
            {
                ForwardExec(this, 0);

                triggerOnEnter = true;
            }
            else if (!actors.Any() && repeat)
            {
                triggerOnEnter = false;
            }
        }
    }
}