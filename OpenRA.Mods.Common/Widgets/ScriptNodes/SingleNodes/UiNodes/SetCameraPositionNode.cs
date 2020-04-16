using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class SetCameraPositionNode : NodeLogic
    {
        readonly IngameNodeScriptSystem ingameNodeScriptSystem;
        CPos loc;
        Player ply;

        public SetCameraPositionNode(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
            this.ingameNodeScriptSystem = ingameNodeScriptSystem;
        }

        public override void Execute(World world)
        {
            if (ingameNodeScriptSystem.WorldRenderer == null)
                return;

            var inPly = InConnections.First(c => c.ConnectionTyp == ConnectionType.Player);
            var inCon = InConnections.First(c => c.ConnectionTyp == ConnectionType.Location);

            if (inPly.In == null)
                throw new YamlException(NodeId + "Player not connected");
            if (inCon.In == null)
                throw new YamlException(NodeId + "Location not connected");

            if (inPly.In.Player == null || world.LocalPlayer == null)
                return;

            if (inCon.In.Location == null)
                return;

            ply = world.Players.First(p => p.InternalName == inPly.In.Player.Name);
            loc = inCon.In.Location.Value;

            if (loc == null || ingameNodeScriptSystem.WorldRenderer == null || world.LocalPlayer != ply)
                return;

            ingameNodeScriptSystem.WorldRenderer.Viewport.Center(world.Map.CenterOfCell(loc));
        }
    }
}