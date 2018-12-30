using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class CameraRideNodeLogic : NodeLogic
    {
        CPos loc;
        Player ply;
        IngameNodeScriptSystem insc;

        public CameraRideNodeLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
            this.insc = insc;
        }

        public override void Execute(World world)
        {
            if (insc.WorldRenderer == null)
                return;

            var inPly = InConnections.First(c => c.ConTyp == ConnectionType.Player);
            var inCon = InConnections.First(c => c.ConTyp == ConnectionType.Location);

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

            if (loc == null || insc.WorldRenderer == null || world.LocalPlayer != ply)
                return;

            insc.WorldRenderer.Viewport.Center(world.Map.CenterOfCell(loc));
        }
    }
}