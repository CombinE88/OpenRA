using System;
using System.Collections.Generic;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeWorldLoaded : NodeWidget
    {
        public static Dictionary<NodeType, BuildNodeConstructorInfo> NodeBuilder =
            new Dictionary<NodeType, BuildNodeConstructorInfo>()
            {
                {
                    NodeType.TriggerWorldLoaded, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TriggerLogicWorldLoaded),

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after the game starts")
                        }
                    }
                },
            };

        public TriggerNodeWorldLoaded(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class TriggerLogicWorldLoaded : NodeLogic
    {
        public TriggerLogicWorldLoaded(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
            nodeInfo, ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            ForwardExec(this);
        }
    }
}