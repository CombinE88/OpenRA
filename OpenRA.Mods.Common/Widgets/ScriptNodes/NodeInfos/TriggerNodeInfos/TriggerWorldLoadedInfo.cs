using System;
using System.Collections.Generic;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.TriggerNodeInfos
{
    public class TriggerWorldLoadedInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TriggerWorldLoaded", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Trigger"},
                        Name = "World Loaded",

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after the game starts")
                        }
                    }
                },
            };

        public TriggerWorldLoadedInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId,
            nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            NodeLogic.ForwardExec(logic);
        }
    }
}