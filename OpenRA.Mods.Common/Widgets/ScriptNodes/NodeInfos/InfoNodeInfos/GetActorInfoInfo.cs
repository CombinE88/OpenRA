using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.ActorNodeInfos;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.InfoNodeInfos
{
    public class GetActorInfoInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "ActorGetInformations", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Actor Activity"},
                        Name = "Actor Information of Actor",

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

        public GetActorInfoInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }
        
        public override void LogicDoAfterConnections(NodeLogic logic)
        {
            if (logic.InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).In == null)
                Debug.WriteLine(NodeId + "Actor not connected");
        }
        

        public override void LogicTick(Actor self, NodeLogic logic)
        {
            var actor = logic.GetLinkedConnectionFromInConnection(ConnectionType.Actor, 0);
            if (actor.Actor == null)
                return;

            logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfo).ActorInfo = actor.ActorInfo;
            logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.Location).Location = actor.Location;
            logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.Player).Player =
                actor.Actor.Owner.PlayerReference;
        }
    }
}