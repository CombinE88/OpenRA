using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Eluant;
using OpenRA.Activities;
using OpenRA.Effects;
using OpenRA.Mods.Common.Activities;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Primitives;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes
{
    public class FunctionNodeReinforcements : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "Reinforcements", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(FunctionLogicReinforcements),
                        Nesting = new []{"Functions"},
                         Name = "Reinforcements",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfoArray, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.CellPath, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    "ReinforcementsWithTransport", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(FunctionLogicReinforcements),
                        Nesting = new []{"Functions"},
                        Name = "Reinforcements (Transport)",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfo, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfoArray, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.CellPath, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.CellPath, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        public FunctionNodeReinforcements(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class FunctionLogicReinforcements : NodeLogic
    {
        DomainIndex domainIndex;
        Dictionary<uint, MovementClassDomainIndex> domainIndexes;
        List<Actor> reinforce = new List<Actor>();
        TileSet tileSet;

        public FunctionLogicReinforcements(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
            nodeInfo, ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            
            domainIndex = world.WorldActor.Trait<DomainIndex>();
            domainIndexes = new Dictionary<uint, MovementClassDomainIndex>();
            tileSet = world.Map.Rules.TileSet;
            var locomotors = world.WorldActor.TraitsImplementing<Locomotor>()
                .Where(l => !string.IsNullOrEmpty(l.Info.Name));
            var movementClasses = locomotors.Select(t => t.Info.GetMovementClass(tileSet)).Distinct();

            foreach (var mc in movementClasses)
                domainIndexes[mc] = new MovementClassDomainIndex(world, mc);

            var player = GetLinkedConnectionFromInConnection(ConnectionType.Player, 0);
            if (player == null || player.Player == null)
            {
                Debug.WriteLine(NodeId + "Reinforcement Player not connected");
                return;
            }

            var actorInfoArray = GetLinkedConnectionFromInConnection(ConnectionType.ActorInfoArray, 0);
            if (actorInfoArray == null || actorInfoArray.ActorInfos == null || !actorInfoArray.ActorInfos.Any())
            {
                Debug.WriteLine(NodeId + "Reinforcement ActorGroup not connected or empty");
                return;
            }

            var cellPath = GetLinkedConnectionFromInConnection(ConnectionType.CellPath, 0);
            if (cellPath == null || cellPath.CellArray == null || !cellPath.CellArray.Any())
            {
                Debug.WriteLine(NodeId + "Reinforcement Entry Path not connected or empty");
                return;
            }

            if (NodeType == "Reinforcements")
            {
                var actors = new List<string>();
                foreach (var act in actorInfoArray
                    .ActorInfos) actors.Add(act.Name);

                var inNumber = GetLinkedConnectionFromInConnection(ConnectionType.Integer, 0);

                Reinforce(
                    world,
                    world.Players.First(p => p.InternalName == player.Player.Name),
                    actors.ToArray(),
                    cellPath.CellArray.ToArray(),
                    inNumber != null ? inNumber.Number.Value : 25);
            }

            if (NodeType == "ReinforcementsWithTransport")
            {
                var exitpath = GetLinkedConnectionFromInConnection(ConnectionType.CellPath, 1);
                if (exitpath == null || exitpath.CellArray == null || !exitpath.CellArray.Any())
                {
                    Debug.WriteLine(NodeId + "Reinforcement exit path not connected or empty");
                    return;
                }

                var transport = GetLinkedConnectionFromInConnection(ConnectionType.ActorInfo, 0);
                if (transport == null || transport.ActorInfo == null)
                {
                    Debug.WriteLine(NodeId + "Transport actor not connected");
                    return;
                }

                var actors = new List<string>();
                foreach (var act in InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfoArray).In
                    .ActorInfos) actors.Add(act.Name);

                ReinforceWithTransport(
                    world,
                    world.Players.First(p =>
                        p.InternalName == player.Player
                            .Name),
                    transport.ActorInfo.Name,
                    actors.ToArray(),
                    cellPath.CellArray.ToArray(),
                    exitpath.CellArray.ToArray());
            }

            ForwardExec(this, 1);
        }

        Actor CreateActor(World world, Player owner, string actorType, bool addToWorld, CPos? entryLocation = null,
            CPos? nextLocation = null)
        {
            ActorInfo ai;
            if (!world.Map.Rules.Actors.TryGetValue(actorType, out ai))
            {
                throw new LuaException("Unknown actor type '{0}'".F(actorType));
                return null;
            }

            var initDict = new TypeDictionary();

            initDict.Add(new OwnerInit(owner));

            if (entryLocation.HasValue)
            {
                var pi = ai.TraitInfoOrDefault<AircraftInfo>();
                initDict.Add(new CenterPositionInit(owner.World.Map.CenterOfCell(entryLocation.Value) +
                                                    new WVec(0, 0, pi != null ? pi.CruiseAltitude.Length : 0)));
                initDict.Add(new LocationInit(entryLocation.Value));
            }

            if (entryLocation.HasValue && nextLocation.HasValue)
                initDict.Add(new FacingInit(world.Map.FacingBetween(CPos.Zero,
                    CPos.Zero + (nextLocation.Value - entryLocation.Value), 0)));

            var actor = world.CreateActor(addToWorld, actorType, initDict);

            return actor;
        }

        void Move(Actor actor, CPos dest)
        {
            var move = actor.TraitOrDefault<IMove>();
            if (move == null)
                return;

            actor.QueueActivity(move.MoveTo(dest, 2));
        }

        public Actor[] Reinforce(World world, Player owner, string[] actorTypes, CPos[] entryPath, int interval = 25)
        {
            var actors = new List<Actor>();
            var leng = 0;
            for (var i = 0; i < actorTypes.Length; i++)
            {
                leng += interval;
                var actor = CreateActor(world, owner, actorTypes[i], false, entryPath[0],
                    entryPath.Length > 1 ? entryPath[1] : (CPos?) null);
                reinforce.Add(actor);
                actors.Add(actor);

                var actionDelay = i * interval;
                Action actorAction = () =>
                {
                    world.Add(actor);
                    for (var j = 1; j < entryPath.Length; j++)
                        Move(actor, entryPath[j]);
                };

                world.AddFrameEndTask(w => w.Add(new DelayedAction(actionDelay, actorAction)));
            }

            Action worldendAction = () =>
            {
                OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).ActorGroup = actors.ToArray();
                ForwardExec(this, 1);
            };
            world.AddFrameEndTask(w => w.Add(new DelayedAction(leng, worldendAction)));
            return actors.ToArray();
        }

        public void ReinforceWithTransport(World world, Player owner, string actorType, string[] cargoTypes,
            CPos[] entryPath, CPos[] exitPath = null, int dropRange = 3)
        {
            var transport = CreateActor(world, owner, actorType, true, entryPath[0],
                entryPath.Length > 1 ? entryPath[1] : (CPos?) null);
            var cargo = transport.TraitOrDefault<Cargo>();

            var passengers = new List<Actor>();
            reinforce = new List<Actor>();
            if (cargo != null && cargoTypes != null)
                foreach (var cargoType in cargoTypes)
                {
                    var passenger = CreateActor(world, owner, cargoType, false, entryPath[0]);
                    passengers.Add(passenger);
                    cargo.Load(transport, passenger);
                    reinforce.Add(passenger);
                }

            for (var i = 1; i < entryPath.Length; i++)
                Move(transport, entryPath[i]);

            var aircraft = transport.TraitOrDefault<Aircraft>();
            if (aircraft != null)
            {
                var destination = entryPath.Last();

                // Try to find an alternative landing spot if we can't land at the current destination
                if (!aircraft.CanLand(destination) && dropRange > 0)
                {
                    var locomotors = cargo.Passengers
                        .Select(a => a.Info.TraitInfoOrDefault<MobileInfo>())
                        .Where(m => m != null)
                        .Distinct()
                        .Select(m => m.LocomotorInfo)
                        .ToList();

                    foreach (var c in transport.World.Map.FindTilesInCircle(destination, dropRange))
                    {
                        if (!aircraft.CanLand(c))
                            continue;

                        if (!locomotors.All(m => domainIndex.IsPassable(destination, c, m)))
                            continue;

                        destination = c;
                        break;
                    }
                }

                if (aircraft.Info.VTOL)
                {
                    if (destination != entryPath.Last())
                        Move(transport, destination);

                    transport.QueueActivity(new Turn(transport, aircraft.Info.InitialFacing));
                    transport.QueueActivity(new HeliLand(transport, true));
                }
                else
                {
                    transport.QueueActivity(new Land(transport, Target.FromCell(transport.World, destination)));
                }

                transport.QueueActivity(new Wait(15));
            }

            if (cargo != null)
            {
                transport.QueueActivity(new UnloadCargo(transport, true));
                transport.QueueActivity(new WaitFor(() => cargo.IsEmpty(transport)));
            }

            transport.QueueActivity(new CallFunc(() => { ForwardExec(this, 0); }));
            transport.QueueActivity(new Wait(aircraft != null ? 50 : 25));

            if (exitPath != null)
            {
                foreach (var wpt in exitPath)
                    Move(transport, wpt);

                transport.QueueActivity(new RemoveSelf());
            }
        }
    }
}