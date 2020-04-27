using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.NodeInfos.TriggerNodeInfos
{
    public class CreateTimerInfo : NodeInfo
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TriggerCreateTimer", new BuildNodeConstructorInfo
                    {
                        Nesting = new[] {"Timer"},
                        Name = "Create Timer",

                        InConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.Integer, "Timer duration"),
                            new Tuple<ConnectionType, string>(ConnectionType.Enabled,
                                "Determent whether or not the timer repeats periodically"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Setup and start the timer")
                        },
                        OutConnections = new List<Tuple<ConnectionType, string>>
                        {
                            new Tuple<ConnectionType, string>(ConnectionType.TimerConnection, "Reference to the timer"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs when the timer has run out"),
                            new Tuple<ConnectionType, string>(ConnectionType.Exec, "Runs after setting up the timer")
                        }
                    }
                },
            };

        public CreateTimerInfo(string nodeType, string nodeId, string nodeName) : base(nodeType, nodeId, nodeName)
        {
        }
        
        bool repeating;

        int timer;
        
        bool timerDone;
        int timerMax;
        bool timerStarted;

        public override void LogicExecute(World world, NodeLogic logic)
        {
            timerStarted = true;
            NodeLogic.ForwardExec(logic, 1);
        }

        public void StopTimer()
        {
            timerStarted = false;
        }

        public void ResetTimer()
        {
            timer = 0;
            timerDone = false;
        }

        public void StartTimer()
        {
            timerStarted = true;
        }

        public override void LogicTick(Actor self, NodeLogic logic)
        {
            if (logic.InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Enabled) != null
                && logic.InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Enabled).In != null)
                repeating = true;
            else if (repeating)
                repeating = false;

            if (!timerStarted || timerDone)
                return;

            if (timer < timerMax)
            {
                timer++;
            }
            else if (!timerDone)
            {
                timer = 0;
                if (!repeating)
                    timerDone = true;
                NodeLogic.ForwardExec(logic, 0);
            }
        }

        public override void LogicDoAfterConnections(NodeLogic logic)
        {
            var conInInt = logic.InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Integer);
            if (conInInt == null || conInInt.In.Number == null)
                throw new YamlException(NodeId + "Timer time not connected");

            timerMax = conInInt.In.Number.Value * 25;
        }
    }
}