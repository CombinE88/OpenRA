using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeCreateTimer : NodeWidget
    {
        public new static Dictionary<string, BuildNodeConstructorInfo> NodeConstructorInformation =
            new Dictionary<string, BuildNodeConstructorInfo>()
            {
                {
                    "TriggerCreateTimer", new BuildNodeConstructorInfo
                    {
                        LogicClass = typeof(TriggerLogicCreateTimer),
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
        public TriggerNodeCreateTimer(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
        }
    }

    public class TriggerLogicCreateTimer : NodeLogic
    {
        bool repeating;

        int timer;
        bool timerDone;
        int timerMax;
        bool timerStarted;

        public TriggerLogicCreateTimer(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(
            nodeInfo, ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            timerStarted = true;
            ForwardExec(this, 1);
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

        public override void Tick(Actor self)
        {
            if (InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Enabled) != null
                && InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Enabled).In != null)
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
                ForwardExec(this, 0);
            }
        }

        public override void DoAfterConnections()
        {
            var conInInt = InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Integer);
            if (conInInt == null || conInInt.In.Number == null)
                throw new YamlException(NodeId + "Timer time not connected");

            timerMax = conInInt.In.Number.Value * 25;
        }
    }

    internal class TimerLogics : NodeLogic
    {
        public TimerLogics(NodeInfo nodeInfo, IngameNodeScriptSystem ingameNodeScriptSystem) : base(nodeInfo,
            ingameNodeScriptSystem)
        {
        }

        public override void Execute(World world)
        {
            var timerConnection = GetLinkedConnectionFromInConnection(ConnectionType.TimerConnection, 0);

            var timer = timerConnection.Logic as TriggerLogicCreateTimer;

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