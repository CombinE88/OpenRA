using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeWorldLoaded : NodeWidget
    {
        public TriggerNodeWorldLoaded(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class TriggerLogicWorldLoaded : NodeLogic
    {
        public TriggerLogicWorldLoaded(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            var exeNodes = Insc.NodeLogics.Where(n =>
                n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && OutConnections
                                                        .Where(t => t.ConTyp == ConnectionType.Exec).Contains(c.In)) != null);
            foreach (var node in exeNodes)
            {
                node.Execute(world);
            }
        }
    }
}