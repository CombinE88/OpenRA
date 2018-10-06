using System.Linq;
using OpenRA.Mods.Common.Effects;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes
{
    public class FunctionCreateEffectLogic : NodeLogic
    {
        public FunctionCreateEffectLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            if (InConnections.First(ic => ic.ConTyp == ConnectionType.Location).In == null
                || InConnections.First(ic => ic.ConTyp == ConnectionType.Location).In.Location == null)
                throw new YamlException(NodeId + "Location not connected");

            if (InConnections.First(ic => ic.ConTyp == ConnectionType.String).In == null
                || InConnections.First(ic => ic.ConTyp == ConnectionType.String).In.String == null)
                throw new YamlException(NodeId + "String Image not connected");

            if (InConnections.Last(ic => ic.ConTyp == ConnectionType.String).In == null
                || InConnections.Last(ic => ic.ConTyp == ConnectionType.String).In.String == null)
                throw new YamlException(NodeId + "String Sequence not connected");

            world.AddFrameEndTask(w =>
            {
                w.Add(new SpriteEffect(
                    w.Map.CenterOfCell(InConnections.First(ic => ic.ConTyp == ConnectionType.Location).In.Location.Value),
                    w,
                    InConnections.First(ic => ic.ConTyp == ConnectionType.String).In.String,
                    InConnections.Last(ic => ic.ConTyp == ConnectionType.String).In.String,
                    "terrain"));
            });
        }
    }
}