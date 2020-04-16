using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Widgets.ScriptNodes.EditorNodeBrushes;
using OpenRA.Mods.Common.Widgets.ScriptNodes.NodeEditorTraits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes
{
    public class MapInfoActorsonMap : NodeWidget
    {
        readonly ButtonWidget button;

        public MapInfoActorsonMap(NodeEditorNodeScreenWidget screen, NodeInfo nodeInfo) : base(screen, nodeInfo)
        {
            button = new ButtonWidget(screen.NodeScriptContainerWidget.ModData);
            AddChild(button);
            button.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 34, FreeWidgetEntries.Width, 25);
            button.Text = "Add Actor";
            button.OnClick = () =>
            {
                Editor.SetBrush(new EditorNodeBrushBrush(
                    CellPicking.Actor,
                    OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList),
                    Editor,
                    Screen.NodeScriptContainerWidget.WorldRenderer,
                    () =>
                    {
                        button.Text =
                            OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList).ActorPreviews != null
                                ? "Group: " + OutConnections.First(c => c.ConnectionTyp == ConnectionType.ActorList)
                                      .ActorPreviews.Count()
                                : "None";
                    }));
            };
        }

        public override void AddOutConConstructor(OutConnection connection)
        {
            base.AddOutConConstructor(connection);

            if (connection.ActorPreviews != null && connection.ActorPreviews.Any())
                button.Text = "Group: " + connection.ActorPreviews.Length;
        }

        public override void Tick()
        {
            base.Tick();

            var first = OutConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.ActorList);
            var second = OutConnections.FirstOrDefault(c => c.ConnectionTyp == ConnectionType.Actor);

            if (first != null && second != null)
                second.ActorPrev = first.ActorPreviews != null && first.ActorPreviews.Any()
                    ? first.ActorPreviews.First()
                    : null;
        }
    }
}