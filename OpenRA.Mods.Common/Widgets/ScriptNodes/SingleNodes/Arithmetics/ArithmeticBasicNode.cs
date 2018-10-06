using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class ArithmeticBasicLogic : NodeLogic
    {
        public ArithmeticBasicLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            if (NodeInfo.NodeType == NodeType.ArithmeticsOr)
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
}