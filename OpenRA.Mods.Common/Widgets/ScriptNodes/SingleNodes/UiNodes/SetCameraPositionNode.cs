using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class SetCameraPositionNode : NodeLogic
    {
        WPos target;
        WPos source;
        int currentLength = 0;
        int maxLength;
        Player ply;
        IngameNodeScriptSystem insc;
        bool active;

        public SetCameraPositionNode(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
            this.insc = insc;
        }

        public override void Execute(World world)
        {
            if (insc.WorldRenderer == null || active)
                return;

            var numb = InConnections.First(c => c.ConTyp == ConnectionType.Integer);
            var inPly = InConnections.First(c => c.ConTyp == ConnectionType.Player);
            var inCon = InConnections.First(c => c.ConTyp == ConnectionType.Location);
            var inCon2 = InConnections.Last(c => c.ConTyp == ConnectionType.Location);

            if (numb.In == null)
                throw new YamlException(NodeId + "Time not connected");
            if (inPly.In == null)
                throw new YamlException(NodeId + "Player not connected");
            if (inCon.In == null)
                throw new YamlException(NodeId + "Target Location not connected");

            if (inPly.In.Player == null || world.LocalPlayer == null || numb.In.Number == null)
                return;

            if (inCon.In.Location == null)
                return;

            if (inCon2.In == null || inCon2.In.Logic == null)
                source = insc.WorldRenderer.Viewport.CenterPosition;

            ply = world.Players.First(p => p.InternalName == inPly.In.Player.Name);
            maxLength = numb.In.Number.Value;
            active = true;
        }

        public override void Tick(Actor self)
        {
            if (!active)
                return;

            if (maxLength < currentLength)
                currentLength++;
            else
            {
                active = false;
            }

            int x = 0;
            int y = 0;

            if (target.X >= source.X)
                x = source.X + (target.X - source.X / maxLength * currentLength);
            else if (target.X < source.X)
                x = source.X - (source.X - target.X / maxLength * currentLength);
            if (target.Y >= source.Y)
                x = source.Y + (target.Y - source.Y / maxLength * currentLength);
            else if (target.Y < source.Y)
                x = source.Y - (source.Y - target.Y / maxLength * currentLength);

            insc.WorldRenderer.Viewport.Center(new WPos(x, y, 0));
        }
    }
}