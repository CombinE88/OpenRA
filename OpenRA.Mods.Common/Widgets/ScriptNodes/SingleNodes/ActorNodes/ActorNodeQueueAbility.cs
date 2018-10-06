using System.Linq;
using OpenRA.Mods.Common.Activities;
using OpenRA.Mods.Common.Traits;
using OpenRA.Primitives;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes
{
    public class ActorNodeQueueAbility : NodeWidget
    {
        public ActorNodeQueueAbility(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class ActorLogicQueueAbility : NodeLogic
    {
        public ActorLogicQueueAbility(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            if (InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Actor).In == null)
                throw new YamlException(NodeId + "Queue Activity Actor not connected");

            if (InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Actor).In.Actor == null)
                return;

            var actor = InConnections.First(c => c.ConTyp == ConnectionType.Actor).In.Actor;

            if (NodeInfo.NodeType == NodeType.ActorQueueMove)
            {
                if (InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Location).In == null)
                    throw new YamlException(NodeId + "Queue Activity Move Location not connected");

                if (InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Location).In.Location == null)
                    return;

                int i = 0;
                if (InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Integer).In != null &&
                    InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Integer).In.Number != null)
                    i = InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Integer).In.Number.Value;

                actor.QueueActivity(new Move(actor,
                    InConnections.First(c => c.ConTyp == ConnectionType.Location).In.Location.Value, WDist.FromCells(i)));
            }
            else if (NodeInfo.NodeType == NodeType.ActorQueueAttack)
            {
                if (InConnections.LastOrDefault(c => c.ConTyp == ConnectionType.Actor).In == null)
                    throw new YamlException(NodeId + "Queue Activity Attack Target Actor not connected");

                if (InConnections.LastOrDefault(c => c.ConTyp == ConnectionType.Actor).In.Actor == null)
                    return;

                if (InConnections.First(c => c.ConTyp == ConnectionType.Location).In.Location == null)
                    return;

                actor.QueueActivity(new Attack(actor,
                    Target.FromActor(InConnections.Last(c => c.ConTyp == ConnectionType.Location).In.Actor),
                    InConnections.First(c => c.ConTyp == ConnectionType.Boolean).In != null,
                    InConnections.Last(c => c.ConTyp == ConnectionType.Boolean).In != null,
                    InConnections.First(c => c.ConTyp == ConnectionType.Boolean).In != null ? InConnections.First(c => c.ConTyp == ConnectionType.Boolean).In.Number ?? 0 : 0));
            }
            else if (NodeInfo.NodeType == NodeType.ActorQueueHunt)
            {
                actor.QueueActivity(new Hunt(actor));
            }
            else if (NodeInfo.NodeType == NodeType.ActorQueueAttackMoveActivity)
            {
                if (InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Location).In == null)
                    throw new YamlException(NodeId + "Queue Activity AttackMove Location not connected");

                if (InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Location).In.Location == null)
                    return;

                actor.QueueActivity(new AttackMoveActivity(actor,
                    new Move(actor, InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Location).In.Location.Value, WDist.FromCells(2))));
            }
            else if (NodeInfo.NodeType == NodeType.ActorQueueSell)
            {
                if (actor.Trait<Sellable>() == null)
                    return;

                actor.Trait<Sellable>().Sell(actor);
            }
            else if (NodeInfo.NodeType == NodeType.ActorQueueFindResources)
            {
                if (actor.Trait<Harvester>() == null)
                    return;

                actor.QueueActivity(new FindResources(actor));
            }
            else if (NodeInfo.NodeType == NodeType.ActorKill)
            {
                actor.Kill(actor);
            }
            else if (NodeInfo.NodeType == NodeType.ActorRemove)
            {
                actor.Dispose();
            }
        }
    }
}