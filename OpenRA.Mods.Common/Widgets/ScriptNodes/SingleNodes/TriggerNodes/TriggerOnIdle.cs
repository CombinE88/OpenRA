using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerOnIdle : NodeLogic
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TriggerOnIdle", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TriggerOnIdle),
                        Nesting = new[] {"Trigger"},
                        Name = "On Actor Idle",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, "Actor that fires the trigger"),
                            new Tuple<ConnectionType, string>(ConnectionType.Enabled,
                                "Trigger can repeat more than once"),
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