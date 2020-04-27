using System;
using System.Collections.Generic;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.TriggerNodeInfos
{
    public class TriggerTickInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TriggerTick", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Trigger"},
                        Name = "Every Tick",

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs every game tick")
                        }
                    }
                },
            };

        public TriggerTickInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            NodeLogic.ForwardExec(logic);
        }

        public override void LogicTick(Actor self, NodeLogic logic)
        {
            logic.Execute(self.World);
        }
    }
}