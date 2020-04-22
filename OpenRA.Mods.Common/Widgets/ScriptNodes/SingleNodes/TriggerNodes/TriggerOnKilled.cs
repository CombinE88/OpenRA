using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerOnKilled : NodeLogic
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TriggerOnKilled", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TriggerOnKilled),
                        Nesting = new[] {"Trigger"},
                        Name = "On Actor Killed",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, "Actor that fires the trigger"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Setup the trigger")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec,
                                "Runs when the trigger condition is met"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after the trigger has set up")
                        }
                    }
                },
                {
                    "TriggerOnAllKilled", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TriggerOnAllKilled),
                        Nesting = new[] {"Trigger"},
                        Name = "On all Actors Killed",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, "Actor Group that fires the trigger"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Setup the trigger")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec,
                                "Runs when the trigger condition is met"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after the trigger has set up")
                        }
                    }
                },
            };
        

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
            var inCon = GetLinkedConnectionFromInConnection(ConnectionType.ActorList, 0);

            if (inCon == null)
            {
                Debug.WriteLine(NodeId + ": Actor list not connected");
                return;
            }

            if (inCon.ActorGroup != null && inCon.ActorGroup.Any(a => !a.IsDead) &&
                inCon.ActorGroup.Any(a => !a.IsDead))
                foreach (var actor in inCon.ActorGroup)
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