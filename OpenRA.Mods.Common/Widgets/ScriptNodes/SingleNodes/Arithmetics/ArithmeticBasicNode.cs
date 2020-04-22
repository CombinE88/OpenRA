using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class ArithmeticBasicLogic : NodeLogic
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "ArithmeticsOr", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(ArithmeticBasicLogic),
                        Nesting = new[] {"Arithmetic's"},
                        Name = "Forward Or",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Enabled, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    "ArithmeticsAnd", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(ArithmeticBasicLogic),
                        Nesting = new[] {"Arithmetic's"},
                        Name = "Forward And",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Enabled, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };
        
        bool repeating;
        bool started;

        public ArithmeticBasicLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void DoAfterConnections()
        {
            var repeatable = GetLinkedConnectionFromInConnection(ConnectionType.Enabled, 0);
            if (repeatable != null)
                repeating = true;
        }

        public override void ExecuteTick(Actor self)
        {
            if (NodeInfo.NodeType == "ArithmeticsOr" && (repeating || !started))
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
            else if (NodeInfo.NodeType == "ArithmeticsAnd" && (repeating || !started))
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