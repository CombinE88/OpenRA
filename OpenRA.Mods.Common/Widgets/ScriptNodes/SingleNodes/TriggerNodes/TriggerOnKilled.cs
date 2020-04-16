using System.Collections.Generic;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerOnKilled : NodeLogic
    {
        readonly List<Actor> actors = new List<Actor>();

        public TriggerOnKilled(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            var inCon = InConnections.First(ic => ic.ConnectionTyp == ConnectionType.Actor);

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
                    if (actor.IsDead && actors.Contains(actor))
                    {
                        ExecuteOnDeath(self.World);
                        actors.Remove(actor);
                    }
            }
        }

        void ExecuteOnDeath(World world)
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

    public class TriggerOnAllKilled : NodeLogic
    {
        List<Actor> actors = new List<Actor>();

        public TriggerOnAllKilled(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            var inCon = InConnections.First(ic => ic.ConnectionTyp == ConnectionType.ActorList);

            if (inCon.In == null)
                throw new YamlException(NodeId + ": Actorlist not connected");

            if (inCon.In.ActorGroup != null && inCon.In.ActorGroup.Any(a => !a.IsDead) &&
                inCon.In.ActorGroup.Any(a => !a.IsDead))
                foreach (var actor in inCon.In.ActorGroup)
                    actors.Add(actor);
        }

        public override void Tick(Actor self)
        {
            if (actors.Any())
            {
                if (actors.Any(a => !a.IsDead))
                    return;

                actors = new List<Actor>();
                ExecuteOnDeath(self.World);
            }
        }

        void ExecuteOnDeath(World world)
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