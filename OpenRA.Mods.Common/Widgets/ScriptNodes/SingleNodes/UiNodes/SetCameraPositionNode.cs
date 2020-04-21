using System.Diagnostics;
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

            var inPly = GetLinkedConnectionFromInConnection(ConnectionType.Player, 0);
            var inCon = GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);

            if (inPly == null)
            {
                Debug.WriteLine(NodeId + "Player not connected");
                return;
            }

            if (inCon == null)
            {
                Debug.WriteLine(NodeId + "Location not connected");
                return;
            }

            if (inPly.Player == null || world.LocalPlayer == null)
                return;

            if (inCon.Location == null)
                return;

            ply = world.Players.First(p => p.InternalName == inPly.Player.Name);
            loc = inCon.Location.Value;

            if (loc == null || ingameNodeScriptSystem.WorldRenderer == null || world.LocalPlayer != ply)
                return;

            ingameNodeScriptSystem.WorldRenderer.Viewport.Center(world.Map.CenterOfCell(loc));

            ForwardExec(this);
        }
    }
}