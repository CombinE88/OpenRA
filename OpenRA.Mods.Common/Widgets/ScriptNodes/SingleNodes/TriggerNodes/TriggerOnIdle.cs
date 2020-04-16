using System.Collections.Generic;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerOnIdle : NodeLogic
    {
        readonly List<Actor> idleActors = new List<Actor>();

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
        }

        public override void Tick(Actor self)
        {
            if (idleActors.Any())
            {
                var newlist = idleActors.ToList();
                foreach (var act in newlist)
                    if (act.IsDead && !act.IsInWorld)
                        idleActors.Remove(act);

                var idles = idleActors.ToList();
                foreach (var actor in idles)
                    if (actor.IsIdle)
                    {
                        OutConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).Actor = actor;
                        ExecuteOnidle(self.World);

                        if (InConnections.First(ic => ic.ConnectionTyp == ConnectionType.Repeatable).In != null)
                            idleActors.Remove(actor);
                    }
            }
        }

        void ExecuteOnidle(World world)
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