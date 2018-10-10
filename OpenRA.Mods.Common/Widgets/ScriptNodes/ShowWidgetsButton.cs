using System.Drawing;
using OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class ShowWidgetsButtonWidget : Widget
    {
        public ScriptNodeWidget Snw;
        string text;
        int2 textsize;
        ButtonWidget butt;

        public ShowWidgetsButtonWidget(ScriptNodeWidget snw)
        {
            Snw = snw;

            text = "Open Node Editor";
            textsize = Snw.FontRegular.Measure(text);

            butt = new ButtonWidget(snw.ModData);
            butt.Bounds = new Rectangle(Game.Settings.Graphics.WindowedSize.X / 2, 5, textsize.X + 20, 25);
            butt.Text = text;
            butt.OnClick = () => { Snw.NodeWidget.Visible = !Snw.NodeWidget.Visible; };
            AddChild(butt);
        }
    }
}