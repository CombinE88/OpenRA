using System.Collections.Generic;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerOnKilled : NodeLogic
    {
        List<Actor> actors = new List<Actor>();

        public TriggerOnKilled(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            var inCon = InConnections.First(ic => ic.ConTyp == ConnectionType.Actor);

            if (inCon.In == null)
                throw new YamlException(NodeId + ": Actor not connected");

            if (!inCon.In.Actor.IsDead && inCon.In.Actor.IsInWorld && !actors.Contains(inCon.In.Actor))
                actors.Add(inCon.In.Actor);
        }

        public override void Tick(Actor self)
        {
            if (actors.Any())
            {
                var idles = actors.ToList();
                foreach (var actor in idles)
                {
                    if (actor.IsDead && actors.Contains(actor))
                    {
                        ExecuteOnDeath(self.World);
                        actors.Remove(actor);
                    }
                }
            }
        }

        void ExecuteOnDeath(World world)
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

    public class TriggerOnAllKilled : NodeLogic
    {
        List<List<Actor>> actors = new List<List<Actor>>();

        public TriggerOnAllKilled(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            var inCon = InConnections.First(ic => ic.ConTyp == ConnectionType.ActorList);

            if (inCon.In == null)
                throw new YamlException(NodeId + ": Actor not connected");

            if (inCon.In.ActorGroup.Any(a => !a.IsDead) && inCon.In.Actor.IsInWorld)
                actors.Add(inCon.In.ActorGroup.ToList());
        }

        public override void Tick(Actor self)
        {
            if (actors.Any())
            {
                var group = actors.ToList();
                foreach (var list in group)
                {
                    if (list.Count(a => !a.IsDead && a.IsInWorld) < 1)
                    {
                        ExecuteOnDeath(self.World);
                        actors.Remove(list);
                    }
                }
            }
        }


        void ExecuteOnDeath(World world)
        {
            var exeNodes = Insc.NodeLogics.Where(n => n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && c.In == OutConnections.First()) != null);
            foreach (var node in exeNodes)
            {
                node.Execute(world);
            }
        }

    }
}