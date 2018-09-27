using System;
using System.Drawing;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.EditorNodeBrushes;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes.OutPuts
{
    public class CellArrayWidget : SimpleNodeWidget
    {
        ButtonWidget startPath;
        OutConnection outconnection;

        public CellArrayWidget(NodeEditorNodeScreenWidget screen) : base(screen)
        {
            WidgetName = "Output: Cell Array";
            AddChild(startPath = new ButtonWidget(screen.Snw.ModData));
            startPath.Text = "Select Cells";
            startPath.OnClick = () => { Editor.SetBrush(new EditorCellPickerBrush(CellPicking.Array, this, Editor, screen.Snw.WorldRenderer)); };

            var inRecangle = new Rectangle(0, 0, 0, 0);
            outconnection = new OutConnection(ConnectionType.CellArray, this);
            OutConnections.Add(outconnection);
            outconnection.InWidgetPosition = inRecangle;
        }

        public override void Tick()
        {
            startPath.Bounds = new Rectangle(FreeWidgetEntries.X, FreeWidgetEntries.Y + 25, FreeWidgetEntries.Width, 25);
            if(SelectedCells != null && SelectedCells.Any())
            outconnection.CellArray = SelectedCells;
            base.Tick();
        }

        public override void Draw()
        {
            base.Draw();

            string text = SelectedCells != null ? "Cells: " + SelectedCells.Count : "-- No cells defined --";
            Screen.Snw.FontRegular.DrawTextWithShadow(text, new float2(RenderBounds.X + FreeWidgetEntries.X + 2,RenderBounds.Y + FreeWidgetEntries.Y + 50),
                Color.White, Color.Black, 1);
        }
    }
}