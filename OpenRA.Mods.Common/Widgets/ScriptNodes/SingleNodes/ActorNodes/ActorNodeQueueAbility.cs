using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Activities;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
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
        readonly List<Actor> idleHunter = new List<Actor>();

        public ActorLogicQueueAbility(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Tick(Actor self)
        {
            var idles = idleHunter.ToArray();

            foreach (var idler in idles)
                if (idler.IsDead || !idler.IsInWorld)
                    idleHunter.Remove(idler);
                else if (idler.IsIdle) idler.QueueActivity(new Hunt(idler));
        }

        public override void Execute(World world)
        {
            if (InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Actor).In == null &&
                InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.ActorList).In == null)
                throw new YamlException(NodeId + "Queue Activity needs either a single actor or group");

            var actor = InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).In != null
                ? InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).In.Actor
                : null;

            switch (NodeInfo.NodeType)
            {
                case NodeType.ActorQueueMove:
                {
                    if (InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Location).In == null)
                        throw new YamlException(NodeId + "Queue Activity Move Location not connected");

                    if (InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Location).In.Location ==
                        null)
                        return;

                    var i = 0;
                    if (InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Integer).In != null &&
                        InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Integer).In.Number != null)
                        i = InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Integer).In.Number
                            .Value;

                    if (actor != null)
                        actor.QueueActivity(new Move(actor,
                            InConnections.First(c => c.ConnectionTyp == ConnectionType.Location).In.Location.Value,
                            WDist.FromCells(i)));

                    if (InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup.Any())
                        foreach (var actors in InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In
                            .ActorGroup)
                            if (!actors.IsDead && actors.IsInWorld)
                                actors.QueueActivity(new Move(actors,
                                    InConnections.First(c => c.ConnectionTyp == ConnectionType.Location).In.Location
                                        .Value,
                                    WDist.FromCells(i)));
                    break;
                }
                case NodeType.ActorQueueAttack:
                {
                    if (InConnections.LastOrDefault(c => c.ConnectionTyp == ConnectionType.Actor).In == null)
                        throw new YamlException(NodeId + "Queue Activity Attack Target Actor not connected");

                    var first = InConnections.First(c => c.ConnectionTyp == ConnectionType.Repeatable);
                    var last = InConnections.Last(c => c.ConnectionTyp == ConnectionType.Repeatable);

                    if (actor != null && !actor.IsDead && actor.IsInWorld)
                        actor.QueueActivity(new Attack(actor,
                            Target.FromActor(InConnections.Last(c => c.ConnectionTyp == ConnectionType.Actor).In.Actor),
                            first.In != null, last.In != null, first.In != null ? first.In.Number ?? 0 : 0));

                    if (InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup.Any())
                        foreach (var actors in InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In
                            .ActorGroup)
                            if (!actors.IsDead && actors.IsInWorld)
                                actors.QueueActivity(new Attack(actors,
                                    Target.FromActor(InConnections.Last(c => c.ConnectionTyp == ConnectionType.Actor).In
                                        .Actor),
                                    first.In != null, last.In != null, first.In != null ? first.In.Number ?? 0 : 0));
                    break;
                }
                case NodeType.ActorQueueHunt:
                {
                    var idleHunting = InConnections.First(c => c.ConnectionTyp == ConnectionType.Repeatable).In != null;

                    if (actor != null && !actor.IsDead && actor.IsInWorld)
                    {
                        actor.QueueActivity(new Hunt(actor));

                        if (idleHunting)
                            idleHunter.Add(actor);
                    }

                    if (InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup.Any())
                        foreach (var actors in InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In
                            .ActorGroup)
                        {
                            if (!actors.IsDead && actors.IsInWorld)
                                actors.QueueActivity(new Hunt(actors));

                            if (idleHunting)
                                idleHunter.Add(actors);
                        }

                    break;
                }
                case NodeType.ActorQueueAttackMoveActivity:
                {
                    var conn = InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Location);
                    if (conn.In == null)
                        throw new YamlException(NodeId + "Queue Activity AttackMove Location not connected");

                    if (conn.In.Location == null)
                        return;

                    if (actor != null && !actor.IsDead && actor.IsInWorld)
                        actor.QueueActivity(new AttackMoveActivity(actor,
                            new Move(actor, conn.In.Location.Value, WDist.FromCells(2))));

                    if (InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup.Any())
                        foreach (var actors in InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In
                            .ActorGroup)
                            if (!actors.IsDead && actors.IsInWorld)
                                actors.QueueActivity(new AttackMoveActivity(actors,
                                    new Move(actors, conn.In.Location.Value, WDist.FromCells(2))));
                    break;
                }
                case NodeType.ActorQueueSell:
                {
                    if (actor != null && actor.Trait<Sellable>() != null)
                        actor.Trait<Sellable>().Sell(actor);

                    if (InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup.Any())
                        foreach (var actors in InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In
                            .ActorGroup)
                        {
                            if (actors.IsDead || !actors.IsInWorld)
                                continue;

                            if (actors.Trait<Sellable>() == null)
                                continue;

                            actors.Trait<Sellable>().Sell(actors);
                        }

                    break;
                }
                case NodeType.ActorQueueFindResources:
                {
                    if (actor != null && !actor.IsDead && actor.IsInWorld && actor.Trait<Harvester>() != null)
                        actor.QueueActivity(new FindResources(actor));

                    if (InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup.Any())
                        foreach (var actors in InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In
                            .ActorGroup)
                            if (!actors.IsDead && actors.IsInWorld)
                                actor.QueueActivity(new FindResources(actors));
                    break;
                }
                case NodeType.ActorKill:
                {
                    if (actor != null && !actor.IsDead && actor.IsInWorld)
                        actor.Kill(actor);

                    if (InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup.Any())
                        foreach (var actors in InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In
                            .ActorGroup)
                            if (!actors.IsDead && actors.IsInWorld)
                                actors.Kill(actors);
                    break;
                }
                case NodeType.ActorRemove:
                {
                    if (actor != null && !actor.IsDead && actor.IsInWorld)
                        actor.Dispose();

                    if (InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup.Any())
                        foreach (var actors in InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In
                            .ActorGroup)
                            if (!actors.IsDead && actors.IsInWorld)
                                actors.Dispose();
                    break;
                }
                case NodeType.ActorChangeOwner:
                {
                    var newPlayer = InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Player);
                    if (newPlayer.In == null)
                        throw new YamlException(NodeId + "ChangeOwner Player not connected");

                    if (actor != null && !actor.IsDead && actor.IsInWorld)
                        actor.ChangeOwner(world.Players.First(p => p.InternalName == newPlayer.In.Player.Name));

                    if (InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup != null
                        && InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In.ActorGroup.Any())
                        foreach (var actors in InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).In
                            .ActorGroup)
                            actors.ChangeOwner(world.Players.First(p => p.InternalName == newPlayer.In.Player.Name));
                    break;
                }
            }

            ForwardExec(this);
        }
    }
}