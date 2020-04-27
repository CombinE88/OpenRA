using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.GroupInfos
{
    public class GroupFindActorsInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "FindActorsOnFootprint", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(GroupFindActorsInfo),
                        Nesting = new[] {"Actor/Player Group"},
                        Name = "Find Actors on Footprint",

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
                    "FinActorsInCircle", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(GroupFindActorsInfo),
                        Nesting = new[] {"Actor/Player Group"},
                        Name = "Find Actors in Range",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "Range in cells"),
                            new Tuple<ConnectionType, string>(ConnectionType.Location, ""),
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

        public GroupFindActorsInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            var outCon = logic.OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList);

            if (NodeType == "FinActorsInCircle")
            {
                var integ = logic.GetLinkedConnectionFromInConnection(ConnectionType.Integer, 0);
                var cell = logic.GetLinkedConnectionFromInConnection(ConnectionType.Location, 0);

                if (integ == null)
                {
                    Debug.WriteLine(NodeId + "FindActorsInCircle Integer (Range) not connected");
                    return;
                }

                if (cell == null)
                {
                    Debug.WriteLine(NodeId + "FindActorsInCircle Location not connected");
                    return;
                }

                outCon.ActorGroup = world.FindActorsInCircle(world.Map.CenterOfCell(integ.Location.Value),
                        WDist.FromCells(integ.Number.Value))
                    .Where(a => !a.IsDead && a.IsInWorld).ToArray();
            }
            else if (NodeType == "FindActorsOnFootprint")
            {
                var integ = logic.GetLinkedConnectionFromInConnection(ConnectionType.CellArray, 0);
                ;

                if (integ == null)
                {
                    Debug.WriteLine(NodeId + "Cell Array not connected");
                    return;
                }

                outCon.ActorGroup = world.Actors
                    .Where(a => !a.IsDead && a.IsInWorld && integ.CellArray.Contains(a.Location)).ToArray();
            }

            NodeLogic.ForwardExec(logic);
        }
    }
}