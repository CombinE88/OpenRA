using System.Collections.Generic;
using System.Diagnostics;
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
            var inCon = GetLinkedConnectionFromInConnection(ConnectionType.Actor, 0);

            if (inCon == null)
            {
                Debug.WriteLine(NodeId + ": Actor not connected");
                return;
            }

            if (!inCon.Actor.IsDead && inCon.Actor.IsInWorld)
                idleActors.Add(inCon.Actor);

            enabled = true;

            ForwardExec(this, 1);
        }

        public override void Tick(Actor self)
        {
            if (!enabled || idleActors.All(a => a.IsDead))
                return;

            var newList = idleActors.ToList();
            foreach (var act in newList.Where(act => act.IsDead && !act.IsInWorld))
                idleActors.Remove(act);

            var idles = idleActors.ToList();
            foreach (var actor in idles.Where(actor => actor.IsIdle))
            {
                OutConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).Actor = actor;
                ForwardExec(this, 0);

                if (GetLinkedConnectionFromInConnection(ConnectionType.Enabled, 0) != null)
                    idleActors.Remove(actor);
            }
        }
    }
}