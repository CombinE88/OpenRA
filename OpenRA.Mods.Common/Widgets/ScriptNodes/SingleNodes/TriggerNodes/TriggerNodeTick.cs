using System;
using System.Collections.Generic;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeTick : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TriggerTick", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TriggerLogicTick),
                        Nesting = new[] {"Trigger"},
                        Name = "Every Tick",

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