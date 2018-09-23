using System.Drawing;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class AddNewNodeWidget : Widget
    {
        NodeEditorNodeScreenWidget screen;
        string text;
        object textsize;
        ScriptNodeWidget snw;
        NodeEditorBackgroundWidget NEBack;

        public AddNewNodeWidget(ScriptNodeWidget snw, NodeEditorNodeScreenWidget screen, NodeEditorBackgroundWidget NEBack)
        {
            this.screen = screen;
            this.snw = snw;
            this.NEBack = NEBack;

            text = "Add Test Node";
            textsize = snw.FontRegular.Measure(text);
        }

        public override void Tick()
        {
            Bounds = new Rectangle(NEBack.RenderBounds.X + 5, NEBack.RenderBounds.Y + 5, 110, 25);
        }

        public override bool HandleMouseInput(MouseInput mi)
        {
            if (!EventBounds.Contains(mi.Location))
            {
                return false;
            }

            if (mi.Button == MouseButton.Left && mi.Event == MouseInputEvent.Down)
                screen.AddNode();

            return true;
        }

        public override void Draw()
        {
            WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X, RenderBounds.Y, RenderBounds.Width, RenderBounds.Height), Color.Black);
            WidgetUtils.FillRectWithColor(new Rectangle(RenderBounds.X + 1, RenderBounds.Y + 1, RenderBounds.Width - 2, RenderBounds.Height - 2), Color.Silver);

            snw.FontRegular.DrawTextWithShadow(text, new float2(RenderBounds.X + 2, RenderBounds.Y + 1),
                Color.White, Color.Black, 1);
        }
    }
}