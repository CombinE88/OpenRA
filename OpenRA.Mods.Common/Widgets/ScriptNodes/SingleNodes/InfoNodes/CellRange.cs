using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.EditorNodeBrushes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.InfoNodes
{
    public class CelLRangeWidget : SimpleNodeWidget
    {
        ButtonWidget startPath;
        OutConnection outconnection;

        public CelLRangeWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Info: Location + Range";
            AddChild(startPath = new ButtonWidget(screen.Snw.ModData));
            startPath.Text = "Select Location";
            startPath.OnClick = () => { Editor.SetBrush(new EditorCellPickerBrush(CellPicking.Range, this, Editor, screen.Snw.WorldRenderer)); };

            var inRecangle = new Rectangle(0, 0, 0, 0);
            outconnection = new OutConnection(ConnectionType.Location, this);
            OutConnections.Add(outconnection);
            outconnection = new OutConnection(ConnectionType.Integer, this);
            OutConnections.Add(outconnection);
        }

        public override void Tick()
        {
            startPath.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);
            if (SelectedCells != null && SelectedCells.Any())
                outconnection.Location = SelectedCells.FirstOrDefault();
            base.Tick();
        }

        public override void Draw()
        {
            base.Draw();

            string text = SelectedCells != null ? "Cell: " + SelectedCells.FirstOrDefault().X + " " + SelectedCells.FirstOrDefault().Y : "-- No cell picked --";
            Screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(RenderBounds.X + FreeWidgetEntries.X + 2,RenderBounds.Y +  FreeWidgetEntries.Y + 50),
                Color.White, Color.Black, 1);
            text = Range != 0 ? "Range: " + Range : "-- No Range defined --";
            Screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(RenderBounds.X + FreeWidgetEntries.X + 2,RenderBounds.Y +  FreeWidgetEntries.Y + 75),
                Color.White, Color.Black, 1);
        }
    }
}