using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets
{
    public class EditorToolbarBrush : IEditorBrush
    {
        EditorBrushLayer ebl;

        //WorldRenderer worldRenderer;
        Widget editorWidget;

        public EditorToolbarBrush(EditorViewportControllerWidget editorWidget, WorldRenderer wr)
        {
            this.editorWidget = editorWidget;
            //worldRenderer = wr;
            ebl = wr.World.WorldActor.Trait<EditorBrushLayer>();
        }


        public void Dispose()
        {
            ebl.ClearSelection();
        }

        public bool HandleMouseInput(MouseInput mi)
        {
            if (mi.Button != MouseButton.Left && mi.Button != MouseButton.Right && !(mi.Event == MouseInputEvent.Up || mi.Event == MouseInputEvent.Down))
                return false;

            if (mi.Button == MouseButton.Right && mi.Event == MouseInputEvent.Down)
            {
                if (mi.Modifiers != Modifiers.Shift)
                {
                    ebl.OnLeftClick();
                }
                else
                {
                    ebl.OnShiftLeftClick();
                }

                return true;
            }

            if (mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down && ebl.Cells.Any())
            {
                ebl.OnRightClick();
                return true;
            }

            return false;
        }

        public void Tick()
        {
        }
    }
}