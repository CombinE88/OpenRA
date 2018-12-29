using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class SetCameraPositionNode : NodeLogic, IWorldLoaded
    {
        CPos loc;
        Player ply;
        WorldRenderer wr;

        public SetCameraPositionNode(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            if (wr == null)
                return;

            var inPly = InConnections.First(c => c.ConTyp == ConnectionType.Player);
            var inCon = InConnections.First(c => c.ConTyp == ConnectionType.Location);

            if (inPly.In == null)
                throw new YamlException(NodeId + "Player not connected");
            if (inCon.In == null)
                throw new YamlException(NodeId + "Location not connected");

            if (inPly.In.Player == null)
                return;

            if (inCon.In.Location == null)
                return;

            ply = world.Players.First(p => p.InternalName == inPly.In.Player.Name);
            loc = inCon.In.Location.Value;

            if (world.LocalPlayer == null || world.LocalPlayer != ply || loc == null)
                return;

            wr.Viewport.Center(world.Map.CenterOfCell(loc));
        }

        public void WorldLoaded(World w, WorldRenderer wr)
        {
            this.wr = wr;
        }
    }
}