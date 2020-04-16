using System;
using System.Linq;
using OpenRA.Effects;
using OpenRA.Primitives;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes
{
    public class ActorNodeCreateActor : NodeWidget
    {
        public ActorNodeCreateActor(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
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
            if (InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfo).In == null ||
                InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfo).In.ActorInfo == null)
                throw new YamlException(NodeId + "Actor Actor Info not connected");

            if (InConnections.First(c => c.ConnectionTyp == ConnectionType.Location).In == null ||
                InConnections.First(c => c.ConnectionTyp == ConnectionType.Location).In.Location == null)
                throw new YamlException(NodeId + "Actor Location Info not connected");

            if (InConnections.First(c => c.ConnectionTyp == ConnectionType.Player).In == null ||
                InConnections.First(c => c.ConnectionTyp == ConnectionType.Player).In.Player == null)
                throw new YamlException(NodeId + "Actor Player not connected");

            var typeDict = new TypeDictionary
            {
                new OwnerInit(InConnections.First(c => c.ConnectionTyp == ConnectionType.Player).In.Player.Name)
            };

            typeDict.Add(new LocationInit(InConnections.First(c => c.ConnectionTyp == ConnectionType.Location).In
                .Location.Value));

            if (InConnections.First(c => c.ConnectionTyp == ConnectionType.Integer).In != null)
                typeDict.Add(new FacingInit(InConnections.First(c => c.ConnectionTyp == ConnectionType.Integer).In
                    .Number.Value));

            var newActor = world.CreateActor(false,
                InConnections.First(c => c.ConnectionTyp == ConnectionType.ActorInfo).In.ActorInfo.Name, typeDict);

            Action actorAction = () =>
            {
                world.Add(newActor);

                OutConnections.First(c => c.ConnectionTyp == ConnectionType.Actor).Actor = newActor;

                var oCon = OutConnections.FirstOrDefault(o => o.ConnectionTyp == ConnectionType.Exec);
                if (oCon != null)
                    foreach (var node in IngameNodeScriptSystem.NodeLogics.Where(n =>
                        n.InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Exec) != null))
                    {
                        var inCon = node.InConnections.FirstOrDefault(c =>
                            c.ConnectionTyp == ConnectionType.Exec && c.In == oCon);
                        if (inCon != null)
                            inCon.Execute = true;
                    }
            };

            world.AddFrameEndTask(w => w.Add(new DelayedAction(0, actorAction)));
        }
    }
}