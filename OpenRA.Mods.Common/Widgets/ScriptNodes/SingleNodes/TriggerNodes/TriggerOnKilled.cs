using System.Collections.Generic;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerOnKilled : NodeLogic
    {
        readonly List<Actor> actors = new List<Actor>();
        bool enabled;

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

            enabled = true;

            ForwardExec(this, 1);
        }

        public override void Tick(Actor self)
        {
            if (!enabled || !actors.Any()) 
                return;
            
            var idles = actors.ToList();
            foreach (var actor in idles.Where(actor => actor.IsDead && actors.Contains(actor)))
            {
                ForwardExec(this, 0);
                actors.Remove(actor);
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
            
            ForwardExec(this, 1);
        }

        public override void Tick(Actor self)
        {
            if (!actors.Any())
                return;

            if (actors.Any(a => !a.IsDead))
                return;

            actors = new List<Actor>();
            ForwardExec(this, 0);
        }
    }
}