using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeTick : NodeWidget
    {
        public TriggerNodeTick(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class TriggerLogicTick : NodeLogic
    {
        public TriggerLogicTick(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            var exeNodes = Insc.NodeLogics.Where(n => n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && c.In == OutConnections.First()) != null);
            foreach (var node in exeNodes)
            {
                node.Execute(world);
            }
        }

        public override void Tick(Actor self)
        {
            Execute(self.World);
        }
    }
}