using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorArrayNodes
{
    public class FindActorsInCircle : SimpleNodeWidget
    {
        public FindActorsInCircle(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            OutConnections.Add(new OutConnection(ConnectionType.ActorList, this));
            InConnections.Add(new InConnection(ConnectionType.Location, this));
            InConnections.Add(new InConnection(ConnectionType.Integer, this));
            WidgetName = "Group: Find Actors in circle";
        }
    }
}