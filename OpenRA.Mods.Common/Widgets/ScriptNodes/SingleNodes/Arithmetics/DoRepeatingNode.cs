using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class DoRepeatingNodeLogic : NodeLogic
    {
        int repeat;
        int repeatwaiter;

        public DoRepeatingNodeLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            var incon = InConnections.First(c => c.ConTyp == ConnectionType.Integer);
            if (incon.In != null && incon.In.Number != null)
                repeat += incon.In.Number.Value;
        }

        public override void Tick(Actor self)
        {
            if (repeatwaiter > 0)
            {
                repeatwaiter--;
                return;
            }

            if (repeat <= 0)
                return;

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
            repeatwaiter = 5;
            repeat--;
        }
    }
}