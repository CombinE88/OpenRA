using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class DoRepeatingNodeLogic : NodeLogic
    {
        int repeat;
        int repeatwaiter;

        public DoRepeatingNodeLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            var incon = GetLinkedConnectionFromInConnection(ConnectionType.Integer, 0);
            if (incon != null && incon.Number != null)
                repeat += incon.Number.Value;
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

            repeatwaiter = 5;
            repeat--;
        }
    }
}