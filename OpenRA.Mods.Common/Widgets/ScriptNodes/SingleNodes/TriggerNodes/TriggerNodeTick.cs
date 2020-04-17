using System.Linq;

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
        public TriggerLogicTick(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            ForwardExec(this);
        }

        public override void Tick(Actor self)
        {
            Execute(self.World);
        }
    }
}