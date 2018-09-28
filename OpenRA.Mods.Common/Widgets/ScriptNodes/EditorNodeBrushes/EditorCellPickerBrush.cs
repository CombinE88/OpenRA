using System.Collections.Generic;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.EditorNodeBrushes
{
    public class EditorCellPickerBrush : IEditorBrush
    {
        readonly Map map;
        readonly EditorViewportControllerWidget editorWidget;
        public List<CPos> Cells = new List<CPos>();

        SimpleNodeWidget node;
        NodeSelectionLayer nodeSelectionLayer;
        WorldRenderer worldRenderer;
        int range;

        public EditorCellPickerBrush(CellPicking mode, SimpleNodeWidget node, EditorViewportControllerWidget editorWidget, WorldRenderer wr)
        {
            worldRenderer = wr;
            this.node = node;
            this.editorWidget = editorWidget;

            map = wr.World.Map;

            nodeSelectionLayer = wr.World.WorldActor.Trait<NodeSelectionLayer>();

            Cells = node.SelectedCells;
            nodeSelectionLayer.Mode = mode;
            node.Screen.Bgw.Visible = false;

            if (node.SelectedCells != null && node.SelectedCells.Any())
                nodeSelectionLayer.LoadInPath(node.SelectedCells);

            if (node.Range != 0 && mode == CellPicking.Range)
                nodeSelectionLayer.SetRange(new WDist(range * 1024));
            else if (mode == CellPicking.Range)
            {
                nodeSelectionLayer.SetRange(new WDist(0));
            }
        }

        public void Dispose()
        {
            nodeSelectionLayer.Clear();
        }


        public bool HandleMouseInput(MouseInput mi)
        {
            if (mi.Button != MouseButton.Left && mi.Button != MouseButton.Right)
                return false;

            if (mi.Button == MouseButton.Right)
            {
                if (mi.Event == MouseInputEvent.Up)
                {
                    node.SelectedCells = nodeSelectionLayer.CellRegion;
                    node.Screen.Bgw.Visible = true;
                    node.Range = range;
                    editorWidget.ClearBrush();
                    return true;
                }

                return false;
            }

            if (mi.Button == MouseButton.Left && mi.Modifiers == Modifiers.Shift && nodeSelectionLayer.Mode == CellPicking.Array)
            {
                nodeSelectionLayer.RemoveCell(worldRenderer.Viewport.ViewToWorld(mi.Location));
                return true;
            }

            if (mi.Button == MouseButton.Left && mi.Modifiers == Modifiers.Shift && nodeSelectionLayer.Mode == CellPicking.Path && mi.Event == MouseInputEvent.Down)
            {
                nodeSelectionLayer.RemoveCell(worldRenderer.Viewport.ViewToWorld(mi.Location));
                return true;
            }

            if (mi.Button == MouseButton.Left && nodeSelectionLayer.Mode == CellPicking.Path && mi.Event == MouseInputEvent.Down)
            {
                nodeSelectionLayer.AddCell(worldRenderer.Viewport.ViewToWorld(mi.Location));
                return true;
            }

            if (mi.Button == MouseButton.Left && nodeSelectionLayer.Mode == CellPicking.Array)
            {
                nodeSelectionLayer.AddCell(worldRenderer.Viewport.ViewToWorld(mi.Location));
                return true;
            }

            if (mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down && nodeSelectionLayer.Mode == CellPicking.Range)
            {
                nodeSelectionLayer.Clear();
                nodeSelectionLayer.AddCell(worldRenderer.Viewport.ViewToWorld(mi.Location));
                nodeSelectionLayer.FixedCursorPosition = worldRenderer.Viewport.ViewToWorld(mi.Location);
                return true;
            }

            if (mi.Button == MouseButton.Left && nodeSelectionLayer.Mode == CellPicking.Range)
            {
                range = (nodeSelectionLayer.FixedCursorPosition - worldRenderer.Viewport.ViewToWorld(mi.Location)).Length;
                nodeSelectionLayer.SetRange(new WDist(range * 1024));
                return true;
            }

            if (mi.Button == MouseButton.Left && nodeSelectionLayer.Mode == CellPicking.Single && mi.Event == MouseInputEvent.Down)
            {
                nodeSelectionLayer.Clear();
                nodeSelectionLayer.AddCell(worldRenderer.Viewport.ViewToWorld(mi.Location));
                return true;
            }

            return false;
        }

        public void Tick()
        {
        }
    }
}