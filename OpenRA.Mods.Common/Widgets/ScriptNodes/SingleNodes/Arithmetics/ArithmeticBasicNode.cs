using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class ArithmeticBasicLogic : NodeLogic
    {
        bool repeating;
        bool started;

        public ArithmeticBasicLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void DoAfterConnections()
        {
            if (InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Repeatable) != null
                && InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Repeatable).In != null)
                repeating = true;
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

        public override void ExecuteTick(Actor self)
        {
            if (NodeInfo.NodeType == NodeType.ArithmeticsOr && (repeating || !started))
            {
                foreach (var conn in InConnections.Where(c => c.ConnectionTyp == ConnectionType.Exec))
                    if (conn.Execute)
                    {
                        Execute(self.World);
                        conn.Execute = false;
                        started = true;
                        break;
                    }
            }
            else if (NodeInfo.NodeType == NodeType.ArithmeticsAnd && (repeating || !started))
            {
                if (!InConnections.First(c => c.ConnectionTyp == ConnectionType.Exec).Execute)
                    return;

                if (!InConnections.Last(c => c.ConnectionTyp == ConnectionType.Exec).Execute)
                    return;

                Execute(self.World);
                started = true;
                InConnections.First(c => c.ConnectionTyp == ConnectionType.Exec).Execute = false;
                InConnections.Last(c => c.ConnectionTyp == ConnectionType.Exec).Execute = false;
            }
        }
    }
}