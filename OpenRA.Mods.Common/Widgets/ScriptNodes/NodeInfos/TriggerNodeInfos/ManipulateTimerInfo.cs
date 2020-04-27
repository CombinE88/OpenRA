using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.TriggerNodeInfos
{
    public class ManipulateTimerInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TimerReset", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Timer"},
                        Name = "Reset Timer",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.TimerConnection, "Reference to the timer"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Resets the timer")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after the timer got reset")
                        }
                    }
                },
                {
                    "TimerStart", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Timer"},
                        Name = "Start Timer",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.TimerConnection, "Reference to the timer"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Starts the timer")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after the timer starts")
                        }
                    }
                },
                {
                    "TimerStop", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Timer"},
                        Name = "Stop Timer",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.TimerConnection, "Reference to the timer"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Stops the timer")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after the timer got stopped")
                        }
                    }
                },
            };


        public ManipulateTimerInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }

        public override void LogicExecute(World world, NodeLogic logic)
        {
            var timerConnection = logic.GetLinkedConnectionFromInConnection(ConnectionType.TimerConnection, 0);

            var timer = timerConnection.Logic.NodeInfo as CreateTimerInfo;

            if (timer == null)
            {
                Debug.WriteLine(NodeId + "Timer not found");
                return;
            }

            switch (NodeType)
            {
                case "TimerReset":
                    timer.ResetTimer();
                    break;
                case "TimerStart":
                    timer.StartTimer();
                    break;
                case "TimerStop":
                    timer.StopTimer();
                    break;
            }
        }
    }
}