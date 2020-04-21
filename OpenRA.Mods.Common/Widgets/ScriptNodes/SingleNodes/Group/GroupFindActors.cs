using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Group
{
    public class GroupFindActorsLogic : NodeLogic
    {
        public static Dictionary<NodeType, BuildNodeConstructorInfo> NodeBuilder =
            new Dictionary<NodeType, BuildNodeConstructorInfo>()
            {
                {
                    NodeType.FindActorsOnFootprint, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(GroupFindActorsLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.CellArray, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
                {
                    NodeType.FinActorsInCircle, new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(GroupFindActorsLogic),

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.LocationRange, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.ActorList, ""),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "")
                        }
                    }
                },
            };

        public GroupFindActorsLogic(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            var outCon = OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList);

            if (NodeType == NodeType.FinActorsInCircle)
            {
                var integ = GetLinkedConnectionFromInConnection(ConnectionType.LocationRange, 0);

                if (integ == null)
                {
                    Debug.WriteLine(NodeId + "FindActorsInCircle Location and Range not connected");
                    return;
                }

                outCon.ActorGroup = world.FindActorsInCircle(world.Map.CenterOfCell(integ.Location.Value),
                        WDist.FromCells(integ.Number.Value))
                    .Where(a => !a.IsDead && a.IsInWorld).ToArray();
            }
            else if (NodeType == NodeType.FindActorsOnFootprint)
            {
                var integ = GetLinkedConnectionFromInConnection(ConnectionType.CellArray, 0);
                ;

                if (integ == null)
                {
                    Debug.WriteLine(NodeId + "Cell Array not connected");
                    return;
                }

                outCon.ActorGroup = world.Actors
                    .Where(a => !a.IsDead && a.IsInWorld && integ.CellArray.Contains(a.Location)).ToArray();
            }

            ForwardExec(this);
        }
    }
}