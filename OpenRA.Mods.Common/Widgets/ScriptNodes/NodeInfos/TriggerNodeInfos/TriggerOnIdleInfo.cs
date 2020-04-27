using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.TriggerNodeInfos
{
    public class TriggerOnIdleInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TriggerOnIdle", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TriggerOnIdleInfo),
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

        public TriggerOnIdleInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            var inCon = logic.GetLinkedConnectionFromInConnection(ConnectionType.Actor, 0);

            if (inCon == null)
            {
                Debug.WriteLine(NodeId + ": Actor not connected");
                return;
            }

            if (!inCon.Actor.IsDead && inCon.Actor.IsInWorld)
                idleActors.Add(inCon.Actor);

            enabled = true;

            NodeLogic.ForwardExec(logic, 1);
        }

        public override void LogicTick(Actor self, NodeLogic logic)
        {
            if (!enabled || idleActors.All(a => a.IsDead))
                return;

            var newList = idleActors.ToList();
            foreach (var act in newList.Where(act => act.IsDead && !act.IsInWorld))
                idleActors.Remove(act);

            var idles = idleActors.ToList();
            foreach (var actor in idles.Where(actor => actor.IsIdle))
            {
                logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).Actor = actor;
                NodeLogic.ForwardExec(logic, 0);

                if (logic.GetLinkedConnectionFromInConnection(ConnectionType.Enabled, 0) != null)
                    idleActors.Remove(actor);
            }
        }
    }
}