using System.Drawing;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class ShowWidgetsButtonWidget : Widget
    {
        public ScriptNodeWidget Snw;
        string text;
        int2 textsize;

        public ShowWidgetsButtonWidget(ScriptNodeWidget snw)
        {
            Snw = snw;

            text = "Open Node Editor";
            textsize = Snw.FontRegular.Measure(text);

            Bounds = new Rectangle(5, 40, textsize.X + 5, 25);
        }

        public override void Tick()
        {
            Bounds = new Rectangle(5, 40, textsize.X + 5, 25);
        }

        public override bool HandleMouseInput(MouseInput mi)
        {
            if (!EventBounds.Contains(mi.Location))
            {
                return false;
            }

            if (mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down)
                Snw.NodeWidget.Visible = !Snw.NodeWidget.Visible;

            return true;
        }

        public override void Draw()
        {
            WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height), Color.Black);
            WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X + 1, RenderBounds.Y + 1, RenderBounds.Width - 2, RenderBounds.Height - 2), Color.Silver);

            Snw.FontRegular.DrawTextWithShadow(text, new float2(RenderBounds.X + 2, RenderBounds.Y + 1),
                Color.White, Color.Black, 1);
        }
    }
}