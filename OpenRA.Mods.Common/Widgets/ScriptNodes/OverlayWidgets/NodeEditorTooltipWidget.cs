using System.Drawing;
using OpenRA.Graphics;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.OverlayWidgets
{
    public class NodeEditorTooltipWidget : Widget
    {
        readonly string background = "panel-black";

        LabelWidget tooltipText;
        SpriteFont font;

        public NodeEditorTooltipWidget(SpriteFont fontRegular)
        {
            font = fontRegular;

            tooltipText = new LabelWidget();
            AddChild(tooltipText);
            Visible = false;
        }

        public void ShowToolTip(string text, int2 location)
        {
            Bounds = new Rectangle(location.X, location.Y, font.Measure(text).X + 12, font.Measure(text).Y + 12);
            tooltipText.Bounds = new Rectangle(2, 2, Bounds.Width - 4, Bounds.Height - 4);
            tooltipText.Text = text;
            Visible = true;
        }

        public override void Draw()
        {
            WidgetUtils.DrawPanel(background, RenderBounds);
        }
    }
}