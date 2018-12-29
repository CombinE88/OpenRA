using System;
using System.Linq;
using OpenRA.Effects;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.FunctionNodes
{
    public class TimedExecutionLogic : NodeLogic
    {
        public TimedExecutionLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Execute(World world)
        {
            Action delayedAction = () =>
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
            };

            var conInInt = InConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Integer)?.In;
            var delay = 0;
            if (conInInt == null || conInInt.Number == null)
                delay = conInInt.Number.Value;

            world.AddFrameEndTask(w => w.Add(new DelayedAction(delay, delayedAction)));
        }
    }
}