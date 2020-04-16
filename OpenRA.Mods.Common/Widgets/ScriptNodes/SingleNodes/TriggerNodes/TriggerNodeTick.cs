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
            var oCon = OutConnections.FirstOrDefault(o => o.ConnectionTyp == ConnectionType.Exec);
            if (oCon != null)
                foreach (var node in IngameNodeScriptSystem.NodeLogics.Where(n =>
                    n.InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Exec) != null))
                {
                    var inCon = node.InConnections.FirstOrDefault(c =>
                        c.ConnectionTyp == ConnectionType.Exec && c.In == oCon);
                    if (inCon != null)
                        inCon.Execute = true;
                }
        }

        public override void Tick(Actor self)
        {
            Execute(self.World);
        }
    }
}