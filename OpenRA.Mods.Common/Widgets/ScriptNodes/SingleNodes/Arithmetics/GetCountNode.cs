using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.Arithmetics
{
    public class GetCountNodeLogic : NodeLogic
    {
        public GetCountNodeLogic(NodeInfo nodeinfo, IngameNodeScriptSystem insc) : base(nodeinfo, insc)
        {
        }

        public override void Tick(Actor self)
        {
            var outcon = OutConnections.First(c => c.ConTyp == ConnectionType.Integer);
            var incon = InConnections.First(c => c.ConTyp == ConnectionType.Universal);

            if (incon.In == null)
                return;

            if (incon.In.ConTyp == ConnectionType.Integer)
                outcon.Number = incon.In.Number.Value;
            else if (incon.In.ConTyp == ConnectionType.ActorList)
                outcon.Number = incon.In.ActorGroup.Length;
            else if (incon.In.ConTyp == ConnectionType.PlayerGroup)
                outcon.Number = incon.In.PlayerGroup.Length;
            else if (incon.In.ConTyp == ConnectionType.ActorInfoArray)
                outcon.Number = incon.In.ActorInfos.Length;
            else if (incon.In.ConTyp == ConnectionType.CellPath)
                outcon.Number = incon.In.CellArray.Count;
            else if (incon.In.ConTyp == ConnectionType.CellArray)
                outcon.Number = incon.In.CellArray.Count;
            else if (incon.In.ConTyp == ConnectionType.LocationRange)
                outcon.Number = incon.In.Number.Value;
            else
                outcon.Number = incon.In.Number.Value;
        }
    }
}