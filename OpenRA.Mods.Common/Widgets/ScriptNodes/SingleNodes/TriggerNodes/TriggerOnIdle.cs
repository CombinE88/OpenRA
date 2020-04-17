using System.Collections.Generic;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerOnIdle : NodeLogic
    {
        readonly List<Actor> idleActors = new List<Actor>();
        bool enabled;

        public TriggerOnIdle(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            var inCon = InConnections.First(ic => ic.ConnectionTyp == ConnectionType.Actor);

            if (inCon.In == null)
                throw new YamlException(NodeId + ": Actor not connected");

            if (!inCon.In.Actor.IsDead && inCon.In.Actor.IsInWorld)
                idleActors.Add(inCon.In.Actor);

            enabled = true;

            ForwardExec(this, 1);
        }

        public override void Tick(Actor self)
        {
            if (!enabled || !idleActors.Any(a => !a.IsDead))
                return;

            var newList = idleActors.ToList();
            foreach (var act in newList.Where(act => act.IsDead && !act.IsInWorld))
                idleActors.Remove(act);

            var idles = idleActors.ToList();
            foreach (var actor in idles.Where(actor => actor.IsIdle))
            {
                OutConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).Actor = actor;
                ForwardExec(this, 0);

                if (InConnections.First(ic => ic.ConnectionTyp == ConnectionType.Repeatable).In != null)
                    idleActors.Remove(actor);
            }
        }
    }
}