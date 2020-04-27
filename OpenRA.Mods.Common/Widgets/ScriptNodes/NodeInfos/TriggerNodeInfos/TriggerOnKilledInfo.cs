using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.TriggerNodeInfos
{
    public class TriggerOnKilledInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TriggerOnKilled", new BuildNodeConstructorInfo
                    {
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
                }
            };


        readonly List<Actor> actors = new List<Actor>();
        bool enabled;

        public TriggerOnKilledInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            var inCon = logic.InConnections.First(ic => ic.ConnectionTyp == ConnectionType.Actor);

            if (inCon.In == null)
                throw new YamlException(NodeId + ": Actor not connected");

            if (!inCon.In.Actor.IsDead && inCon.In.Actor.IsInWorld && !actors.Contains(inCon.In.Actor))
                actors.Add(inCon.In.Actor);

            enabled = true;

            NodeLogic.ForwardExec(logic, 1);
        }

        public override void LogicTick(Actor self, NodeLogic logic)
        {
            if (!enabled || !actors.Any())
                return;

            var idles = actors.ToList();
            foreach (var actor in idles.Where(actor => actor.IsDead && actors.Contains(actor)))
            {
                NodeLogic.ForwardExec(logic, 0);
                actors.Remove(actor);
            }
        }
    }

    public class TriggerOnAllKilledInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TriggerOnAllKilled", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Trigger"},
                        Name = "On all Actors Killed",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList,
                                "Actor Group that fires the trigger"),
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

        List<Actor> actors = new List<Actor>();

        public TriggerOnAllKilledInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId,
            nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            var inCon = logic.GetLinkedConnectionFromInConnection(ConnectionType.ActorList, 0);

            if (inCon == null)
            {
                Debug.WriteLine(NodeId + ": Actor list not connected");
                return;
            }

            if (inCon.ActorGroup != null && inCon.ActorGroup.Any(a => !a.IsDead) &&
                inCon.ActorGroup.Any(a => !a.IsDead))
                foreach (var actor in inCon.ActorGroup)
                    actors.Add(actor);

            NodeLogic.ForwardExec(logic, 1);
        }

        public override void LogicTick(Actor self, NodeLogic logic)
        {
            if (!actors.Any())
                return;

            if (actors.Any(a => !a.IsDead))
                return;

            actors = new List<Actor>();
            NodeLogic.ForwardExec(logic, 0);
        }
    }
}