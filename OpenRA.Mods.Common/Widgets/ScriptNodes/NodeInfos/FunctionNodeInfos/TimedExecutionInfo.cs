using System;
using System.Collections.Generic;
using OpenRA.Effects;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.FunctionNodeInfos
{
    public class TimedExecutionInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TimedExecution", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Functions"},
                        Name = "Timed Execution",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                }
            };


        public TimedExecutionInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId,
            nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            Action delayedAction = () => { NodeLogic.ForwardExec(logic, 0); };

            var conInInt = logic.GetLinkedConnectionFromInConnection(ConnectionType.Integer, 0);
            ;

            var delay = 0;
            if (!(conInInt == null || conInInt == null || conInInt.Number == null))
                delay = conInInt.Number.Value;
            world.AddFrameEndTask(w => w.Add(new DelayedAction(delay, delayedAction)));
            NodeLogic.ForwardExec(logic, 1);
        }
    }
}