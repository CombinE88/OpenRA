using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class DoRepeatingNodeLogic : NodeLogic
    {
        public DoRepeatingNodeLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            if (InConnections.First(c => c.ConTyp == ConnectionType.Integer).In != null)
                for (int i = 0; i < InConnections.First(c => c.ConTyp == ConnectionType.Integer).In.Number.Value; i++)
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
                }
        }
    }
}