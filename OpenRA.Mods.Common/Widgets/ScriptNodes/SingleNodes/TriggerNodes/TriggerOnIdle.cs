using System.Collections.Generic;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerOnIdle : NodeLogic
    {
        List<Actor> idleActors = new List<Actor>();

        public TriggerOnIdle(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            var inCon = InConnections.First(ic => ic.ConTyp == ConnectionType.Actor);

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
                {
                    if (act.IsDead && !act.IsInWorld)
                        idleActors.Remove(act);
                }

                var idles = idleActors.ToList();
                foreach (var actor in idles)
                {
                    if (actor.IsIdle)
                    {
                        OutConnections.First(c => c.ConTyp == ConnectionType.Actor).Actor = actor;
                        ExecuteOnidle(self.World);

                        if (InConnections.First(ic => ic.ConTyp == ConnectionType.Repeatable).In != null)
                            idleActors.Remove(actor);
                    }
                }
            }
        }

        void ExecuteOnidle(World world)
        {
            var exeNodes = Insc.NodeLogics.Where(n =>
                n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && OutConnections
                                                        .Where(t => t.ConTyp == ConnectionType.Exec).Contains(c.In)) != null);
            foreach (var node in exeNodes)
            {
                node.Execute(world);
            }
        }
    }
}