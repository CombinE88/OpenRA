using System.Drawing;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class NodeEditorBackgroundWidget : Widget
    {
        public ScriptNodeWidget Snw;
        NodeEditorNodeScreenWidget screenWidget;
        AddNewNodeWidget testAdd;

        [ObjectCreator.UseCtor]
        public NodeEditorBackgroundWidget(ScriptNodeWidget snw)
        {
            Snw = snw;

            Children.Add(screenWidget = new NodeEditorNodeScreenWidget(Snw, this));
            Children.Add(testAdd = new AddNewNodeWidget(Snw, screenWidget, this));
        }

        public override void Tick()
        {
            Bounds = new Rectangle(100, 100, Snw.RenderBounds.Width - 200, Snw.RenderBounds.Height - 200);
        }

        public override bool HandleMouseInput(MouseInput mi)
        {
            if (!EventBounds.Contains(mi.Location))
            {
                return false;
            }

            return true;
        }

        public override void Draw()
        {
            WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X - 3, RenderBounds.Y - 3, RenderBounds.Width + 6, RenderBounds.Height + 6), Color.Black);
            WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height), Color.DarkGray);
        }
    }
}