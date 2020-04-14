using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.Library;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;

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
        bool timerStarted;
        bool repeating;
        bool timerDone;

        int timer;
        int timerMax;

        public TriggerLogicCreateTimer(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            timerStarted = true;
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
            if (InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Repeatable) != null
                && InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Repeatable).In != null)
                repeating = true;
            else if (repeating)
                repeating = false;

            if (!timerStarted || timerDone)
                return;

            if (timer < timerMax)
                timer++;
            else if (!timerDone)
            {
                timer = 0;
                if (!repeating)
                    timerDone = true;
                ExecuteTimer(self.World);
            }
        }

        public override void DoAfterConnections()
        {
            var conInInt = InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Integer);
            if (conInInt == null || conInInt.In.Number == null)
                throw new YamlException(NodeId + "Timer time not connected");

            timerMax = conInInt.In.Number.Value * 25;
        }

        void ExecuteTimer(World world)
        {
            var oCon = OutConnections.FirstOrDefault(o => o.ConTyp == ConnectionType.Exec);
            if (oCon != null)
            {
                foreach (var node in Insc.NodeLogics.Where(n => n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec) != null))
                {
                    var inCon = node.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && c.In == oCon);
                    if (inCon != null)
                        inCon.Execute = true;
                }
            }
        }
    }

    class TimerLogics : NodeLogic
    {
        public TimerLogics(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            var timercon = InConnections.First(c => c.ConTyp == ConnectionType.TimerConnection);

            if (timercon.In == null)
                throw new YamlException(NodeId + "Timer not connected");

            var timer = timercon.In.Logic as TriggerLogicCreateTimer;

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