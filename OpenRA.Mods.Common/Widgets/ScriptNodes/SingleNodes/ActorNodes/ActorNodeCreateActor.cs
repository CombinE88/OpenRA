using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Effects;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Primitives;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes
{
    public class ActorNodeCreateActor : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "ActorCreateActor", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(ActorCreateActorLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfo, "Actor type information"),
                            new Tuple<ConnectionType, string>(ConnectionType.Player, "Owner of the actor"),
                            new Tuple<ConnectionType, string>(ConnectionType.Location, "Cell where the actor spawns"),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "Facing of the actor 0-255"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Run the node")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, "Actor that got created"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after the actor is created")
                        }
                    }
                }
            };

        public ActorNodeCreateActor(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen,
            nodeInfo)
        {
            IsIncorrectConnected =
                () => InConnections.Any(inCon => inCon.In == null && inCon.ConnectionTyp != ConnectionType.Integer);
        }
    }

    public class ActorCreateActorLogic : NodeLogic
    {
        public ActorCreateActorLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            var actorInfo = GetLinkedConnectionFromInConnection(ConnectionType.ActorInfo, 0);
            if (actorInfo == null || actorInfo.ActorInfo == null)
            {
                Debug.WriteLine(NodeId + "Actor Actor Info not connected");
                return;
            }

            var location = GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
            if (location == null || location.Location == null)
            {
                Debug.WriteLine(NodeId + "Actor Location Info not connected");
                return;
            }

            var player = GetLinkedConnectionFromInConnection(ConnectionType.Player, 0);
            if (player == null || player.Player == null)
            {
                Debug.WriteLine(NodeId + "Actor Player not connected");
                return;
            }

            var typeDict = new TypeDictionary
            {
                new OwnerInit(player.Player.Name)
            };

            typeDict.Add(new LocationInit(location.Location.Value));

            var rotation = GetLinkedConnectionFromInConnection(ConnectionType.ActorInfo, 0);
            if (rotation != null)
                typeDict.Add(new FacingInit(rotation.Number.Value));

            var newActor = world.CreateActor(false, actorInfo.String, typeDict);

            Action actorAction = () =>
            {
                world.Add(newActor);

                OutConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).Actor = newActor;

                ForwardExec(this);
            };

            world.AddFrameEndTask(w => w.Add(new DelayedAction(0, actorAction)));
        }
    }
}