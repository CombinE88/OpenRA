using System.Linq;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.ActorArrayNodes
{
    public class ActorGroupWidget : SimpleNodeWidget
    {
        public ActorGroupWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            OutConnections.Add(new OutConnection(ConnectionType.ActorList, this));
            WidgetName = "Group: Create Actorgroup";
        }

        public override void Tick()
        {
            var none = InConnections.Where(c => c.In == null).ToArray();
            var with = InConnections.Where(c => c.In != null).ToArray();
            var listsize = with.Length + 1;

            if (none.Length < 1)
                InConnections.Add(new InConnection(ConnectionType.Actor, this));
            else if (none.Length > 1)
                foreach (var con in none)
                {
                    if (con != none.First())
                        InConnections.Remove(con);
                }

            base.Tick();
        }
    }
}