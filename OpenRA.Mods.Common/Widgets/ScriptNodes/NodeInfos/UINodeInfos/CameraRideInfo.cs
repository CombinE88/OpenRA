using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.UINodeInfos
{
    public class CameraRideNodeLogic : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "CameraRide", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(CameraRideNodeLogic),
                        Nesting = new[] {"User Interface", "General UI"},
                        Name = "Camera Ride",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        bool active;
        int currentLength;
        int maxLength;
        WPos source;
        WPos target;

        public CameraRideNodeLogic(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            if (logic.IngameNodeScriptSystem.WorldRenderer == null || active)
                return;
            var numb = logic.GetLinkedConnectionFromInConnection(ConnectionType.Integer, 0);
            var inPly = logic.GetLinkedConnectionFromInConnection(ConnectionType.Player, 0);
            var inCon = logic.GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
            var inCon2 = logic.GetLinkedConnectionFromInConnection(ConnectionType.Location, 1);
            if (numb == null)
            {
                Debug.WriteLine(NodeId + "Time not connected");
                return;
            }

            if (inPly == null)
            {
                Debug.WriteLine(NodeId + "Player not connected");
                return;
            }

            if (inCon == null)
            {
                Debug.WriteLine(NodeId + "Target Location not connected");
                return;
            }

            if (inPly.Player == null || world.LocalPlayer == null || numb.Number == null)
                return;
            if (inCon.Location == null)
                return;

            if (inCon2 == null || inCon2.Location == null)
                source = logic.IngameNodeScriptSystem.WorldRenderer.Viewport.CenterPosition;
            else
                source = world.Map.CenterOfCell(inCon2.Location.Value);

            target = world.Map.CenterOfCell(inCon.Location.Value);
            maxLength = numb.Number.Value;
            active = true;

            NodeLogic.ForwardExec(logic, 0);
        }

        public override void LogicTick(Actor self, NodeLogic logic)
        {
            if (!active)
                return;

            if (maxLength > currentLength)
            {
                currentLength++;
            }
            else if (active)
            {
                NodeLogic.ForwardExec(logic, 1);
                active = false;
            }

            var pos = source + (target - source) / maxLength * currentLength;

            logic.IngameNodeScriptSystem.WorldRenderer.Viewport.Center(pos);
        }
    }
}