using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Effects;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Primitives;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.FunctionNodeInfos
{
    public class CreatActorInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "ActorCreateActor", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Functions"},
                        Name = "Create Actor",

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

        public CreatActorInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override bool WidgetIsIncorrectConnected(NodeWidget widget)
        {
            return widget.InConnections.Any(inCon => inCon.In == null && inCon.ConnectionTyp != ConnectionType.Integer);
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            var actorInfo = logic.GetLinkedConnectionFromInConnection(ConnectionType.ActorInfo, 0);
            if (actorInfo == null || actorInfo.ActorInfo == null)
            {
                Debug.WriteLine(NodeId + "Actor Actor Info not connected");
                return;
            }

            var location = logic.GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
            if (location == null || location.Location == null)
            {
                Debug.WriteLine(NodeId + "Actor Location Info not connected");
                return;
            }

            var player = logic.GetLinkedConnectionFromInConnection(ConnectionType.Player, 0);
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

            var rotation = logic.GetLinkedConnectionFromInConnection(ConnectionType.ActorInfo, 0);
            if (rotation != null && rotation.Number != null)
                typeDict.Add(new FacingInit(rotation.Number.Value));

            var newActor = world.CreateActor(false, actorInfo.String, typeDict);

            Action actorAction = () =>
            {
                world.Add(newActor);

                logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).Actor = newActor;

                NodeLogic.ForwardExec(logic);
            };

            world.AddFrameEndTask(w => w.Add(new DelayedAction(0, actorAction)));
        }
    }
}