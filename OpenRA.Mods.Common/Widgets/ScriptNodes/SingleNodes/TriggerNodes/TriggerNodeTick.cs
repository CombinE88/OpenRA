using System;
using System.Collections.Generic;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeTick : NodeWidget
    {
        public static Dictionary<NodeType, BuildNodeConstructorInfo> NodeBuilder =
            new Dictionary<NodeType, BuildNodeConstructorInfo>()
            {
                {
                    NodeType.TriggerTick, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TriggerLogicTick),

                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs every game tick")
                        }
                    }
                },
            };

        public TriggerNodeTick(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class TriggerLogicTick : NodeLogic
    {
        public TriggerLogicTick(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            ForwardExec(this);
        }

        public override void Tick(Actor self)
        {
            Execute(self.World);
        }
    }
}