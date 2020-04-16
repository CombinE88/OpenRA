using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes
{
    public class GetActorInformationsLogic : NodeLogic
    {
        public GetActorInformationsLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
            nodeInfo, ingameNodeScriptSystem)
        {
        }

        public override void DoAfterConnections()
        {
            if (InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).In == null)
                throw new YamlException(NodeId + "Actor not connected");
        }

        public override void Tick(Actor self)
        {
            var actor = InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).In.Actor;
            if (InConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).In.Actor == null)
                return;

            OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfo).ActorInfo = actor.Info;
            OutConnections.First(c => c.ConnectionTyp == ConnectionType.Location).Location = actor.Location;
            OutConnections.First(c => c.ConnectionTyp == ConnectionType.Player).Player = actor.Owner.PlayerReference;
        }
    }
}