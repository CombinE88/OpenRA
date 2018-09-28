using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorArrayNodes
{
    public class FindActorsOnCells : SimpleNodeWidget
    {
        public FindActorsOnCells(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            OutConnections.Add(new OutConnection(ConnectionType.ActorList, this));
            InConnections.Add(new InConnection(ConnectionType.CellArray, this));
            WidgetName = "Group: Find Actors On Cells";
        }
    }
}