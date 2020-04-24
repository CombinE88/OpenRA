using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenRA.Mods.Common.Effects;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.FunctionNodeInfos
{
    public class CreateEffectInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "CreateEffect", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Functions"},
                        Name = "Create Effect",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.String, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        public CreateEffectInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            var location = logic.GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
            if (location == null || location.Location == null)
            {
                Debug.WriteLine(NodeId + "Location not connected");
                return;
            }

            var image = logic.GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
            if (image == null || image.String == null)
            {
                Debug.WriteLine(NodeId + "String Image not connected");
                return;
            }

            var sequence = logic.GetLinkedConnectionFromInConnection(ConnectionType.Location, 1);
            if (sequence == null || sequence.String == null)
            {
                Debug.WriteLine(NodeId + "String Sequence not connected");
                return;
            }

            world.AddFrameEndTask(w =>
            {
                w.Add(new SpriteEffect(
                    w.Map.CenterOfCell(location.Location.Value),
                    w,
                    image.String,
                    sequence.String,
                    "terrain"));
            });

            NodeLogic.ForwardExec(logic);
        }
    }
}