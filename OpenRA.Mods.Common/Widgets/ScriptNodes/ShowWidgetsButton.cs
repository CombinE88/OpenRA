using System.Drawing;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public sealed class ShowWidgetsButtonWidget : Widget
    {
        public ShowWidgetsButtonWidget(NodeScriptContainerWidget snw)
        {
            var text = "Open Node Editor";
            var button = new ButtonWidget(snw.ModData);

            button.Bounds = new Rectangle(5, Game.Renderer.Resolution.Height - 30, snw.FontRegular.Measure(text).X + 20,
                25);
            button.Text = text;
            button.OnClick = () => { snw.Toggle(); };
            AddChild(button);
        }
    }
}