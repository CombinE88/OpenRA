using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class CameraRideNodeLogic : NodeLogic
    {
        WPos target;
        WPos source;
        int currentLength = 0;
        int maxLength;
        Player ply;
        IngameNodeScriptSystem insc;
        bool active;

        public CameraRideNodeLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
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

            if (inCon2.In == null || inCon2.In.Location == null)
                source = insc.WorldRenderer.Viewport.CenterPosition;
            else
            {
                source = world.Map.CenterOfCell(inCon2.In.Location.Value);
            }

            target = world.Map.CenterOfCell(inCon.In.Location.Value);
            ply = world.Players.First(p => p.InternalName == inPly.In.Player.Name);
            maxLength = numb.In.Number.Value;
            active = true;
        }

        public override void Tick(Actor self)
        {
            if (insc.WorldRenderer == null || self.World.LocalPlayer != ply)
                return;

            if (!active)
                return;

            if (maxLength > currentLength)
                currentLength++;

            else if (active)
            {
                var oCon = OutConnections.FirstOrDefault(o => o.ConTyp == ConnectionType.Exec);
                if (oCon != null)
                {
                    foreach (var node in Insc.NodeLogics.Where(n => n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec) != null))
                    {
                        var inCon = node.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && c.In == oCon);
                        if (inCon != null)
                            inCon.Execute = true;
                    }
                }

                active = false;
            }

            var pos = source + (target - source) / maxLength * currentLength;

            insc.WorldRenderer.Viewport.Center(pos);
        }
    }
}