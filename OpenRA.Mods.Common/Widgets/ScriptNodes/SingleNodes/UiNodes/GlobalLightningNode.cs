using System;
using System.Collections.Generic;
using OpenRA.Mods.Common.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class GlobalLightningNodeLogic : NodeLogic
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "GlobalLightning", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(GlobalLightningNodeLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "Input Color: Red 0-256"),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "Input Color: Green 0-256"),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "Input Color: Blue 0-256"),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "Input Alpha: 0-256"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        readonly bool hasLighting;
        readonly GlobalLightingPaletteEffect lighting;

        public GlobalLightningNodeLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
            nodeInfo, ingameNodeScriptSystem)
        {
            lighting = ingameNodeScriptSystem.World.WorldActor.TraitOrDefault<GlobalLightingPaletteEffect>();
            hasLighting = lighting != null;
        }

        public override void Execute(World world)
        {
            var r = 1f;
            var g = 1f;
            var b = 1f;
            var a = 1f;
            if (InConnections[0].In != null && InConnections[0].In.Number != null)
                r = (float) 1 / 360 * InConnections[0].In.Number.Value;
            if (InConnections[1].In != null && InConnections[1].In.Number != null)
                r = (float) 1 / 360 * InConnections[1].In.Number.Value;
            if (InConnections[2].In != null && InConnections[2].In.Number != null)
                r = (float) 1 / 360 * InConnections[2].In.Number.Value;
            if (InConnections[3].In != null && InConnections[3].In.Number != null)
                r = (float) 1 / 360 * InConnections[3].In.Number.Value;

            if (hasLighting)
            {
                lighting.Red = r;
                lighting.Green = g;
                lighting.Blue = b;
                lighting.Ambient = a;
            }

            ForwardExec(this);
        }
    }
}