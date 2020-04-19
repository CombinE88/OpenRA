using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeCreateTimer : NodeWidget
    {
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
            if (InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Repeatable) != null
                && InConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Repeatable).In != null)
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
            var timerConnection = GetLinkedConnectionFromInConnection(this, InConnections, ConnectionType.TimerConnection, 0);

            var timer = timerConnection.Logic as TriggerLogicCreateTimer;

            if (timer == null)
                throw new YamlException(NodeId + "Timer not found");

            switch (NodeType)
            {
                case NodeType.TimerReset:
                    timer.ResetTimer();
                    break;
                case NodeType.TimerStart:
                    timer.StartTimer();
                    break;
                case NodeType.TimerStop:
                    timer.StopTimer();
                    break;
            }
        }
    }
}