using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.UiNodes
{
    public class CameraRideNodeLogic : NodeLogic
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "CameraRide", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(CameraRideNodeLogic),

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
        readonly IngameNodeScriptSystem ingameNodeScriptSystem;
        int maxLength;
        Player ply;
        WPos source;
        WPos target;

        public CameraRideNodeLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
            this.ingameNodeScriptSystem = ingameNodeScriptSystem;
        }

        public override void Execute(World world)
        {
            if (ingameNodeScriptSystem.WorldRenderer == null || active)
                return;
            var numb = GetLinkedConnectionFromInConnection(ConnectionType.Integer, 0);
            var inPly = GetLinkedConnectionFromInConnection(ConnectionType.Player, 0);
            var inCon = GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);
            var inCon2 = GetLinkedConnectionFromInConnection(ConnectionType.Location, 1);
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
                source = ingameNodeScriptSystem.WorldRenderer.Viewport.CenterPosition;
            else
                source = world.Map.CenterOfCell(inCon2.Location.Value);

            target = world.Map.CenterOfCell(inCon.Location.Value);
            ply = world.Players.First(p => p.InternalName == inPly.Player.Name);
            maxLength = numb.Number.Value;
            active = true;

            ForwardExec(this, 0);
        }

        public override void Tick(Actor self)
        {
            if (!active)
                return;

            if (maxLength > currentLength)
            {
                currentLength++;
            }
            else if (active)
            {
                ForwardExec(this, 1);
                active = false;
            }

            var pos = source + (target - source) / maxLength * currentLength;

            ingameNodeScriptSystem.WorldRenderer.Viewport.Center(pos);
        }
    }
}