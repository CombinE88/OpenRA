using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.UINodeInfos
{
    public class SetCameraPositionInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "SetCameraPosition", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(SetCameraPositionInfo),
                        Nesting = new[] {"User Interface", "General UI"},
                        Name = "Set Camera Location",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Player, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        IngameNodeScriptSystem ingameNodeScriptSystem;
        CPos loc;
        Player ply;

        public SetCameraPositionInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            if (ingameNodeScriptSystem.WorldRenderer == null)
                return;

            var inPly = logic.GetLinkedConnectionFromInConnection(ConnectionType.Player, 0);
            var inCon = logic.GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);

            if (inPly == null)
            {
                Debug.WriteLine(NodeId + "Player not connected");
                return;
            }

            if (inCon == null)
            {
                Debug.WriteLine(NodeId + "Location not connected");
                return;
            }

            if (inPly.Player == null || world.LocalPlayer == null)
                return;

            if (inCon.Location == null)
                return;

            ply = world.Players.First(p => p.InternalName == inPly.Player.Name);
            loc = inCon.Location.Value;

            if (loc == null || ingameNodeScriptSystem.WorldRenderer == null || world.LocalPlayer != ply)
                return;

            ingameNodeScriptSystem.WorldRenderer.Viewport.Center(world.Map.CenterOfCell(loc));

            NodeLogic.ForwardExec(logic);
        }
    }
}