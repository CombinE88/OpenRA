using OpenRA.Mods.Common.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class GlobalLightningNodeLogic : NodeLogic
    {
        GlobalLightingPaletteEffect lighting;
        bool hasLighting;

        public GlobalLightningNodeLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
            lighting = insc.World.WorldActor.TraitOrDefault<GlobalLightingPaletteEffect>();
            hasLighting = lighting != null;
        }

        public override void Execute(World world)
        {
            float r = 1f;
            float g = 1f;
            float b = 1f;
            float a = 1f;
            if (InConnections[0].In != null && InConnections[0].In.Number != null)
                r = (float)1 / 360 * InConnections[0].In.Number.Value;
            if (InConnections[1].In != null && InConnections[1].In.Number != null)
                r = (float)1 / 360 * InConnections[1].In.Number.Value;
            if (InConnections[2].In != null && InConnections[2].In.Number != null)
                r = (float)1 / 360 * InConnections[2].In.Number.Value;
            if (InConnections[3].In != null && InConnections[3].In.Number != null)
                r = (float)1 / 360 * InConnections[3].In.Number.Value;

            if (hasLighting)
            {
                lighting.Red = r;
                lighting.Green = g;
                lighting.Blue = b;
                lighting.Ambient = a;
            }
        }
    }
}