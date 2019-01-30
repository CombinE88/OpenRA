using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes
{
    public class GetActorInformationsLogic : NodeLogic
    {
        public GetActorInformationsLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void DoAfterConnections()
        {
            if (InConnections.First(c => c.ConTyp == ConnectionType.Actor).In == null)
                throw new YamlException(NodeId + "Actor not connected");
        }

        public override void Tick(Actor self)
        {
            var actor = InConnections.First(c => c.ConTyp == ConnectionType.Actor).In.Actor;
            if (InConnections.First(c => c.ConTyp == ConnectionType.Actor).In.Actor == null)
                return;

            OutConnections.First(c => c.ConTyp == ConnectionType.ActorInfo).ActorInfo = actor.Info;
            OutConnections.First(c => c.ConTyp == ConnectionType.Location).Location = actor.Location;
            OutConnections.First(c => c.ConTyp == ConnectionType.Player).Player = actor.Owner.PlayerReference;
        }
    }
}