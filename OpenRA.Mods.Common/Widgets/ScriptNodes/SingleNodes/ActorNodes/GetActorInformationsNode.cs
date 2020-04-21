using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes
{
    public class GetActorInformationsLogic : NodeLogic
    {
        public static Dictionary<NodeType, BuildNodeConstructorInfo> NodeBuilder =
            new Dictionary<NodeType, BuildNodeConstructorInfo>()
            {
                {
                    NodeType.ActorGetInformations, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(GetActorInformationsLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Actor, "Target actor")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorInfo, "Actor type"),
                            new Tuple<ConnectionType, string>(ConnectionType.Location, "Cell position of the actor"),
                            new Tuple<ConnectionType, string>(ConnectionType.Player, "Owner of the actor")
                        }
                    }
                }
            };
        public GetActorInformationsLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
            nodeInfo, ingameNodeScriptSystem)
        {
        }

        public override void DoAfterConnections()
        {
            if (InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).In == null)
               Debug.WriteLine(NodeId + "Actor not connected");
        }

        public override void Tick(Actor self)
        {
            var actor = GetLinkedConnectionFromInConnection(ConnectionType.Actor, 0);
            if (actor.Actor == null)
                return;

            OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfo).ActorInfo = actor.ActorInfo;
            OutConnections.First(c => c.ConnectionTyp == ConnectionType.Location).Location = actor.Location;
            OutConnections.First(c => c.ConnectionTyp == ConnectionType.Player).Player =
                actor.Actor.Owner.PlayerReference;
        }
    }
}