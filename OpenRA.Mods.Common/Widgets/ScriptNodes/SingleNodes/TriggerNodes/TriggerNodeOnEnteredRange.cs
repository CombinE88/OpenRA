using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeOnEnteredRange : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "ActorCreateActor", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TriggerLogicOnEnteredRange),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.PlayerGroup,
                                "Only actor of this player group can trigger"),
                            new Tuple<ConnectionType, string>(ConnectionType.LocationRange, "Cells"),
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

        public TriggerNodeOnEnteredRange(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class TriggerLogicOnEnteredRange : NodeLogic
    {
        bool repeat;
        bool triggerOnEnter;
        bool enabled;

        public TriggerLogicOnEnteredRange(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
            nodeInfo, ingameNodeScriptSystem)
        {
        }

        public override void DoAfterConnections()
        {
            var boolean = GetLinkedConnectionFromInConnection(ConnectionType.Enabled, 0);
            repeat = boolean != null;
        }

        public override void Execute(World world)
        {
            enabled = true;
            ForwardExec(this, 1);
        }

        public override void Tick(Actor self)
        {
            if (!enabled || triggerOnEnter && !repeat)
                return;

            var locationRange = GetLinkedConnectionFromInConnection(ConnectionType.LocationRange, 0);

            if (locationRange == null || locationRange.Location == null || locationRange.Number == null)
            {
                Debug.WriteLine(NodeId + "Location and Range not connected");
                return;
            }

            var playerGroup = GetLinkedConnectionFromInConnection(ConnectionType.PlayerGroup, 0);

            if (playerGroup == null || !playerGroup.PlayerGroup.Any())
            {
                Debug.WriteLine(NodeId + "player Group not connected");
                return;
            }

            var actors = self.World
                .FindActorsInCircle(
                    self.World.Map.CenterOfCell(locationRange.Location.Value),
                    WDist.FromCells(locationRange.Number.Value))
                .Where(a => !a.IsDead
                            && a.IsInWorld
                            && a.TraitOrDefault<Mobile>() != null
                            && playerGroup.PlayerGroup
                                .Contains(a.Owner.PlayerReference))
                .ToArray();

            if (!triggerOnEnter && actors.Any())
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