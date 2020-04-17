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
        
        public override void ExecuteTick(Actor self)
        {
            if (NodeInfo.NodeType == NodeType.ArithmeticsOr && (repeating || !started))
            {
                foreach (var conn in InConnections.Where(c => c.ConnectionTyp == ConnectionType.Exec))
                    if (conn.Execute)
                    {
                        ForwardExec(this);
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

                ForwardExec(this);
                started = true;
                InConnections.First(c => c.ConnectionTyp == ConnectionType.Exec).Execute = false;
                InConnections.Last(c => c.ConnectionTyp == ConnectionType.Exec).Execute = false;
            }
        }
    }
}