using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class CameraRideNodeLogic : NodeLogic
    {
        bool active;
        int currentLength;
        readonly IngameNodeScriptSystem ingameNodeScriptSystem;
        int maxLength;
        Player ply;
        WPos source;
        WPos target;

        public CameraRideNodeLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
            this.ingameNodeScriptSystem = ingameNodeScriptSystem;
        }

        public override void Execute(World world)
        {
            if (ingameNodeScriptSystem.WorldRenderer == null || active)
                return;
            var numb = InConnections.First(c => c.ConnectionTyp == ConnectionType.Integer);
            var inPly = InConnections.First(c => c.ConnectionTyp == ConnectionType.Player);
            var inCon = InConnections.First(c => c.ConnectionTyp == ConnectionType.Location);
            var inCon2 = InConnections.Last(c => c.ConnectionTyp == ConnectionType.Location);
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
                source = ingameNodeScriptSystem.WorldRenderer.Viewport.CenterPosition;
            else
                source = world.Map.CenterOfCell(inCon2.In.Location.Value);

            target = world.Map.CenterOfCell(inCon.In.Location.Value);
            ply = world.Players.First(p => p.InternalName == inPly.In.Player.Name);
            maxLength = numb.In.Number.Value;
            active = true;

            ForwardExec(this, 0);
        }

        public override void Tick(Actor self)
        {
            if (!active)
                return;
            
            if (maxLength > currentLength)
            {
                currentLength++;
            }
            else if (active)
            {
                ForwardExec(this, 1);
                active = false;
            }

            var pos = source + (target - source) / maxLength * currentLength;

            ingameNodeScriptSystem.WorldRenderer.Viewport.Center(pos);
        }
    }
}