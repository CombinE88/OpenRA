using System.Collections.Generic;
using System.Diagnostics;
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
            var actorlist = GetLinkedConnectionFromInConnection(ConnectionType.ActorList, 0);
            var actorCon = GetLinkedConnectionFromInConnection(ConnectionType.Actor, 0);
            if (actorCon == null && actorlist == null)
            {
                Debug.WriteLine(NodeId + "Queue Activity needs either a single actor or group");
                return;
            }

            var actor = actorCon.Actor;

            switch (NodeInfo.NodeType)
            {
                case NodeType.ActorQueueMove:
                {
                    var location = GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
                    if (location == null || location.Location == null)
                    {
                        Debug.WriteLine(NodeId + "Queue Activity Move Location not connected");
                        return;
                    }

                    var integer = GetLinkedConnectionFromInConnection(ConnectionType.Integer, 0);
                    var i = 0;
                    if (integer != null && integer.Number != null)
                        i = integer.Number.Value;

                    if (actor != null)
                        actor.QueueActivity(new Move(actor, location.Location.Value, WDist.FromCells(i)));

                    if (actorlist != null && actorlist.ActorGroup != null && actorlist.ActorGroup.Any())
                        foreach (var actors in actorlist.ActorGroup)
                            if (!actors.IsDead && actors.IsInWorld)
                                actors.QueueActivity(new Move(actors,
                                    location.Location.Value,
                                    WDist.FromCells(i)));
                    break;
                }
                case NodeType.ActorQueueAttack:
                {
                    var targetActor = GetLinkedConnectionFromInConnection(ConnectionType.Actor, 1);
                    if (targetActor == null || targetActor.Actor == null)
                    {
                        Debug.WriteLine(NodeId + "Queue Activity Attack Target Actor not connected");
                        return;
                    }

                    var first = GetLinkedConnectionFromInConnection(ConnectionType.Enabled, 0);
                    var last = GetLinkedConnectionFromInConnection(ConnectionType.Enabled, 1);

                    if (actor != null && !actor.IsDead && actor.IsInWorld)
                        actor.QueueActivity(new Attack(actor,
                            Target.FromActor(InConnections.Last(c => c.ConnectionTyp == ConnectionType.Actor).In
                                .Actor),
                            first != null, last != null, first != null ? first.Number ?? 0 : 0));

                    if (actorlist != null
                        && actorlist.ActorGroup != null && actorlist.ActorGroup.Any())
                        foreach (var actors in actorlist.ActorGroup)
                            if (!actors.IsDead && actors.IsInWorld)
                                actors.QueueActivity(new Attack(actors,
                                    Target.FromActor(InConnections
                                        .Last(c => c.ConnectionTyp == ConnectionType.Actor).In
                                        .Actor),
                                    first != null, last != null,
                                    first != null ? first.Number ?? 0 : 0));
                    break;
                }
                case NodeType.ActorQueueHunt:
                {
                    var idleHunting = GetLinkedConnectionFromInConnection(ConnectionType.Enabled, 0) != null;

                    if (actor != null && !actor.IsDead && actor.IsInWorld)
                    {
                        actor.QueueActivity(new Hunt(actor));

                        if (idleHunting)
                            idleHunter.Add(actor);
                    }

                    if (actorlist != null
                        && actorlist.ActorGroup != null && actorlist.ActorGroup.Any())
                        foreach (var actors in actorlist.ActorGroup)
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
                    var attackLocation = GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
                    if (attackLocation == null)
                    {
                        Debug.WriteLine(NodeId + "Queue Activity AttackMove Location not connected");
                        return;
                    }

                    if (attackLocation.Location == null)
                        return;

                    if (actor != null && !actor.IsDead && actor.IsInWorld)
                        actor.QueueActivity(new AttackMoveActivity(actor,
                            new Move(actor, attackLocation.Location.Value, WDist.FromCells(2))));

                    if (actorlist != null && actorlist.ActorGroup != null && actorlist.ActorGroup.Any())
                        foreach (var actors in actorlist.ActorGroup)
                            if (!actors.IsDead && actors.IsInWorld)
                                actors.QueueActivity(new AttackMoveActivity(actors,
                                    new Move(actors, attackLocation.Location.Value, WDist.FromCells(2))));
                    break;
                }
                case NodeType.ActorQueueSell:
                {
                    if (actor != null && actor.Trait<Sellable>() != null)
                        actor.Trait<Sellable>().Sell(actor);

                    if (actorlist != null
                        && actorlist.ActorGroup != null && actorlist.ActorGroup.Any())
                        foreach (var actors in actorlist.ActorGroup)
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

                    if (actorlist != null && actorlist.ActorGroup != null && actorlist.ActorGroup.Any())
                        foreach (var actors in actorlist.ActorGroup)
                            if (!actors.IsDead && actors.IsInWorld)
                                actor.QueueActivity(new FindResources(actors));
                    break;
                }
                case NodeType.ActorKill:
                {
                    if (actor != null && !actor.IsDead && actor.IsInWorld)
                        actor.Kill(actor);

                    if (actorlist != null && actorlist.ActorGroup != null && actorlist.ActorGroup.Any())
                        foreach (var actors in actorlist.ActorGroup)
                            if (!actors.IsDead && actors.IsInWorld)
                                actors.Kill(actors);
                    break;
                }
                case NodeType.ActorRemove:
                {
                    if (actor != null && !actor.IsDead && actor.IsInWorld)
                        actor.Dispose();

                    if (actorlist != null && actorlist.ActorGroup != null && actorlist.ActorGroup.Any())
                        foreach (var actors in actorlist.ActorGroup)
                            if (!actors.IsDead && actors.IsInWorld)
                                actors.Dispose();
                    break;
                }
                case NodeType.ActorChangeOwner:
                {
                    var newPlayer = GetLinkedConnectionFromInConnection(ConnectionType.Player, 0);
                    if (newPlayer == null)
                    {
                        Debug.WriteLine(NodeId + "ChangeOwner Player not connected");
                        return;
                    }

                    if (actor != null && !actor.IsDead && actor.IsInWorld)
                        actor.ChangeOwner(world.Players.First(p => p.InternalName == newPlayer.Player.Name));

                    if (actorlist != null
                        && actorlist.ActorGroup != null && actorlist.ActorGroup.Any())
                        foreach (var actors in actorlist.ActorGroup)
                            actors.ChangeOwner(world.Players.First(p =>
                                p.InternalName == newPlayer.Player.Name));
                    break;
                }
            }

            ForwardExec(this);
        }
    }
}