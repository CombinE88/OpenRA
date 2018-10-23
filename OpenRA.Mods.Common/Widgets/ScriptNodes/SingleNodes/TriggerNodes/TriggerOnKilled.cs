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
            var oCon = OutConnections.FirstOrDefault(o => o.ConTyp == ConnectionType.Exec);
            if (oCon != null)
            {
                foreach (var node in Insc.NodeLogics.Where(n => n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec) != null))
                {
                    var inCon = node.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && c.In == oCon);
                    if (inCon != null)
                        inCon.Execute = true;
                }
            }
        }
    }

    public class TriggerOnAllKilled : NodeLogic
    {
        List<Actor> actors = new List<Actor>();

        public TriggerOnAllKilled(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            var inCon = InConnections.First(ic => ic.ConTyp == ConnectionType.Actor);

            if (inCon.In == null)
                throw new YamlException(NodeId + ": Actor not connected");

            if (inCon.In.Actor != null && !inCon.In.Actor.IsDead && inCon.In.Actor.IsInWorld)
                actors.Add(inCon.In.Actor);
        }

        public override void Tick(Actor self)
        {
            if (actors.Any())
            {
                var list2 = actors.ToArray();
                foreach (var actor in list2)
                {
                    if (actor.IsDead || !actor.IsInWorld)
                    {
                        ExecuteOnDeath(self.World);
                        actors.Remove(actor);
                    }
                }
            }
        }

        void ExecuteOnDeath(World world)
        {
            var oCon = OutConnections.FirstOrDefault(o => o.ConTyp == ConnectionType.Exec);
            if (oCon != null)
            {
                foreach (var node in Insc.NodeLogics.Where(n => n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec) != null))
                {
                    var inCon = node.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && c.In == oCon);
                    if (inCon != null)
                        inCon.Execute = true;
                }
            }
        }
    }
}