using System;
using System.Collections.Generic;
using OpenRA.Effects;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes
{
    public class TimedExecutionLogic : NodeLogic
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TimedExecution", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TimedExecutionLogic),

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
                },
            };

        public TimedExecutionLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            Action delayedAction = () => { ForwardExec(this, 0); };

            var conInInt = GetLinkedConnectionFromInConnection(ConnectionType.Integer, 0);
            ;
            var delay = 0;
            if (!(conInInt == null || conInInt == null || conInInt.Number == null))
                delay = conInInt.Number.Value;

            world.AddFrameEndTask(w => w.Add(new DelayedAction(delay, delayedAction)));

            ForwardExec(this, 1);
        }
    }
}