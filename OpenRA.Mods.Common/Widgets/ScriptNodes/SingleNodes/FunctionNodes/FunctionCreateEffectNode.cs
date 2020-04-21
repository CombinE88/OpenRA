using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Effects;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes
{
    public class FunctionCreateEffectLogic : NodeLogic
    {
        public FunctionCreateEffectLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
            nodeInfo, ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            var location = GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
            if (location == null || location.Location == null)
            {
                Debug.WriteLine(NodeId + "Location not connected");
                return;
            }

            var image = GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
            if (image == null || image.String == null)
            {
                Debug.WriteLine(NodeId + "String Image not connected");
                return;
            }

            var sequence = GetLinkedConnectionFromInConnection(ConnectionType.Location, 1);
            if (sequence == null || sequence.String == null)
            {
                Debug.WriteLine(NodeId + "String Sequence not connected");
                return;
            }

            world.AddFrameEndTask(w =>
            {
                w.Add(new SpriteEffect(
                    w.Map.CenterOfCell(location.Location.Value),
                    w,
                    image.String,
                    sequence.String,
                    "terrain"));
            });

            ForwardExec(this);
        }
    }
}