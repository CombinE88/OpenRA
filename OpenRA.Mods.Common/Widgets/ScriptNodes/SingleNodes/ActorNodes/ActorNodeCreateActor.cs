using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;
using OpenRA.Primitives;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorNodes
{
    public class ActorNodeCreateActor : NodeWidget
    {
        public ActorNodeCreateActor(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            InConTexts.Add("Actor Info");
            InConTexts.Add("Player");
            InConTexts.Add("Cell Location");
            InConTexts.Add("Integer Facing");
            InConTexts.Add("Trigger");
        }
    }

    public class ActorCreateActorLogic : NodeLogic
    {
        public ActorCreateActorLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            if (InConnections.First(c => c.ConTyp == ConnectionType.ActorInfo).In == null || InConnections.First(c => c.ConTyp == ConnectionType.ActorInfo).In.ActorInfo == null)
                throw new YamlException(NodeId + "Actor Actor Info not connected");

            if (InConnections.First(c => c.ConTyp == ConnectionType.Location).In == null || InConnections.First(c => c.ConTyp == ConnectionType.Location).In.Location == null)
                throw new YamlException(NodeId + "Actor Location Info not connected");

            if (InConnections.First(c => c.ConTyp == ConnectionType.Player).In == null || InConnections.First(c => c.ConTyp == ConnectionType.Player).In.Player == null)
                throw new YamlException(NodeId + "Actor Player not connected");

            var typeDict = new TypeDictionary()
            {
                new OwnerInit(InConnections.First(c => c.ConTyp == ConnectionType.Player).In.Player.Name)
            };

            typeDict.Add(new LocationInit(InConnections.First(c => c.ConTyp == ConnectionType.Location).In.Location.Value));

            if (InConnections.First(c => c.ConTyp == ConnectionType.Integer).In != null)
                typeDict.Add(new FacingInit(InConnections.First(c => c.ConTyp == ConnectionType.Integer).In.Number.Value));

            var newActor = world.CreateActor(InConnections.First(c => c.ConTyp == ConnectionType.ActorInfo).In.ActorInfo.Name, typeDict);

            OutConnections.First(c => c.ConTyp == ConnectionType.Actor).Actor = newActor;

            var exeNodes = Insc.NodeLogics.Where(n =>
                n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && c.In == OutConnections.First(o => o.ConTyp == ConnectionType.Exec)) != null);
            foreach (var node in exeNodes)
            {
                node.Execute(world);
            }
        }
    }
}