using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.TriggerNodeInfos
{
    public class TriggerOnEnteredFootPrintInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TriggerOnEnteredFootprint", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Trigger"},
                        Name = "On Entered Footprint",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.PlayerGroup,
                                "Only actor of this player group can trigger"),
                            new Tuple<ConnectionType, string>(ConnectionType.CellArray, "Cells"),
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

        public TriggerOnEnteredFootPrintInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId,
            nodeName)
        {
        }

        bool repeat;
        bool triggerOnEnter;
        bool enabled;

        public override void LogicExecute(World world, NodeLogic logic)
        {
            enabled = true;
            NodeLogic.ForwardExec(logic, 1);
        }

        public override void LogicDoAfterConnections(NodeLogic logic)
        {
            var boolean = logic.GetLinkedConnectionFromInConnection(ConnectionType.Enabled, 0);
            repeat = boolean != null;
        }

        public override void LogicTick(Actor self, NodeLogic logic)
        {
            if (!enabled || triggerOnEnter && !repeat)
                return;

            var cellArray = logic.GetLinkedConnectionFromInConnection(ConnectionType.TimerConnection, 0);
            var playerGroup =
                logic.GetLinkedConnectionFromInConnection(ConnectionType.TimerConnection, 0);

            if (!cellArray.CellArray.Any() || !playerGroup.PlayerGroup.Any())
                return;

            var actors = self.World.Actors
                .Where(a => !a.IsDead
                            && a.IsInWorld
                            && a.TraitOrDefault<Mobile>() != null
                            && playerGroup.PlayerGroup
                                .Contains(a.Owner.PlayerReference)
                            && cellArray.CellArray
                                .Contains(a.Location))
                .ToArray();

            if (actors.Any() && !triggerOnEnter)
            {
                NodeLogic.ForwardExec(logic, 0);

                triggerOnEnter = true;
            }
            else if (!actors.Any() && repeat)
            {
                triggerOnEnter = false;
            }
        }
    }
}