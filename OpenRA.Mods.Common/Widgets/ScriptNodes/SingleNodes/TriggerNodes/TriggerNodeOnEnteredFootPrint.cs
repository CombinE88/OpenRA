using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeOnEnteredFootPrint : NodeWidget
    {
        public static Dictionary<NodeType, BuildNodeConstructorInfo> NodeBuilder =
            new Dictionary<NodeType, BuildNodeConstructorInfo>()
            {
                {
                    NodeType.TriggerOnEnteredFootprint, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TriggerLogicEnteredFootPrint),

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

        public TriggerNodeOnEnteredFootPrint(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen,
            nodeInfo)
        {
        }
    }

    public class TriggerLogicEnteredFootPrint : NodeLogic
    {
        bool repeat;
        bool triggerOnEnter;
        bool enabled;

        public TriggerLogicEnteredFootPrint(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
            nodeInfo, ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            enabled = true;
            ForwardExec(this, 1);
        }

        public override void DoAfterConnections()
        {
            var boolean = GetLinkedConnectionFromInConnection(ConnectionType.Enabled, 0);
            repeat = boolean != null;
        }

        public override void Tick(Actor self)
        {
            if (!enabled || triggerOnEnter && !repeat)
                return;

            var cellArray = GetLinkedConnectionFromInConnection(ConnectionType.TimerConnection, 0);
            var playerGroup =
                GetLinkedConnectionFromInConnection(ConnectionType.TimerConnection, 0);

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
                ForwardExec(this, 0);

                triggerOnEnter = true;
            }
            else if (!actors.Any() && repeat)
            {
                triggerOnEnter = false;
            }
        }
    }
}