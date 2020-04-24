using System;
using System.Collections.Generic;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeWorldLoaded : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TriggerWorldLoaded", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TriggerLogicWorldLoaded),
                        Nesting = new[] {"Trigger"},
                        Name = "World Loaded",

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