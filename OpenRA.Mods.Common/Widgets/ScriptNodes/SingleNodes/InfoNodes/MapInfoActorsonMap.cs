using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.EditorNodeBrushes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes
{
    public class MapInfoActorsonMap : NodeWidget
    {
        ButtonWidget button;

        public MapInfoActorsonMap(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            button = new ButtonWidget(screen.Snw.ModData);
            AddChild(button);
            button.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 34, FreeWidgetEntries.Width, 25);
            button.Text = "Add Actor";
            button.OnClick = () =>
            {
                Editor.SetBrush(new EditorNodeBrushBrush(
                    CellPicking.Actor,
                    OutConnections.First(c => c.ConTyp == ConnectionType.ActorList),
                    Editor,
                    Screen.Snw.WorldRenderer,
                    () =>
                    {
                        button.Text = OutConnections.First(c => c.ConTyp == ConnectionType.ActorList).ActorPrevs != null
                            ? "Group: " + OutConnections.First(c => c.ConTyp == ConnectionType.ActorList).ActorPrevs.Count()
                            : "None";
                    }));
            };
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            if (connection.ActorPrevs != null && connection.ActorPrevs.Any())
                button.Text = "Group: " + connection.ActorPrevs.Length;
        }


        public override void Tick()
        {
            base.Tick();

            var first = OutConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.ActorList);
            var second = OutConnections.FirstOrDefault(c => c.ConTyp == ConnectionType.Actor);

            if (first != null && second != null)
            {
                second.ActorPrev = first.ActorPrevs != null && first.ActorPrevs.Any() ? first.ActorPrevs.First() : null;
            }
        }
    }
}