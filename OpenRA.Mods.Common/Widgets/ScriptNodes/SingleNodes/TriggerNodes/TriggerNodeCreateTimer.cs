using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.TriggerNodes
{
    public class TriggerNodeCreateTimer : NodeWidget
    {
        public TriggerNodeCreateTimer(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            InConTexts.Add("Seconds");
            InConTexts.Add("Trigger");
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

        public override void Tick(Actor self)
        {

            if (InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Boolean) != null && InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Boolean).In != null)
                repeating = true;
            else if (repeating)
                repeating = false;

            if (!timerStarted || (!repeating && timerDone))
                return;

            if (timer < timerMax)
                timer++;
            else if (!timerDone && !repeating)
            {
                timer = 0;
                timerDone = true;
                ExecuteTimer(self.World);
            }
        }

        public override void DoAfterConnections()
        {
            if (InConnections.First(c => c.ConTyp == ConnectionType.Integer).In == null || InConnections.First(c => c.ConTyp == ConnectionType.Integer).In.Number == null)
                throw new YamlException(NodeId + "Timer time not connected");

            timerMax = InConnections.First(c => c.ConTyp == ConnectionType.Integer).In.Number.Value * 25;
        }

        void ExecuteTimer(World world)
        {
            var exeNodes = Insc.NodeLogics.Where(n => n.InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Exec && c.In == OutConnections.First()) != null);
            foreach (var node in exeNodes)
            {
                node.Execute(world);
            }
        }
    }
}