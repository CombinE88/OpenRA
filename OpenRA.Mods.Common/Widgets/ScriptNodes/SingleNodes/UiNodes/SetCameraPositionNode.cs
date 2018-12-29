using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class SetCameraPositionNode : NodeLogic, IFinalizedRenderable
    {
        bool set;
        CPos loc;

        public SetCameraPositionNode(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
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

            var ply = world.Players.First(p => p.InternalName == inPly.In.Player.Name);

            if (world.LocalPlayer != ply)
                return;

            loc = inCon.In.Location.Value;
            set = true;
        }

        public void Render(WorldRenderer wr)
        {
            if (!set)
                return;

            wr.Viewport.Center(wr.World.Map.CenterOfCell(loc));
            set = false;
        }

        public void RenderDebugGeometry(WorldRenderer wr) { }

        public Rectangle ScreenBounds(WorldRenderer wr) { return Rectangle.Empty; }
    }
}