using System.Drawing;
using OpenRA.Graphics;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public class NodeScriptContainerWidget : Widget
    {
        public readonly SpriteFont FontBold;
        public readonly SpriteFont FontRegular;
        public readonly SpriteFont FontSmall;
        public readonly ModData ModData;
        public readonly NodeEditorBackgroundWidget NodeWidget;
        readonly ShowWidgetsButtonWidget showWidget;
        public readonly World World;
        public readonly WorldRenderer WorldRenderer;

        [ObjectCreator.UseCtorAttribute]
        public NodeScriptContainerWidget(World world, WorldRenderer worldRenderer, ModData modData)
        {
            World = world;
            WorldRenderer = worldRenderer;
            ModData = modData;
            Game.Renderer.Fonts.TryGetValue("Regular", out FontRegular);
            Game.Renderer.Fonts.TryGetValue("Small", out FontSmall);
            Game.Renderer.Fonts.TryGetValue("BigBold", out FontBold);

            Children.Add(showWidget = new ShowWidgetsButtonWidget(this));
            Children.Add(NodeWidget = new NodeEditorBackgroundWidget(this, worldRenderer, world) {Visible = false});
            Bounds = new Rectangle(5, Game.Renderer.Resolution.Height - 30,
                showWidget.Bounds.Width, 25);
        }

        public override bool HandleTextInputOuter(string text)
        {
            return false;
        }

        public void Toggle()
        {
            if (NodeWidget.Visible)
            {
                showWidget.Visible = true;
                Bounds = new Rectangle(5, Game.Renderer.Resolution.Height - 30,
                    showWidget.Bounds.Width, 25);
                NodeWidget.Visible = !NodeWidget.Visible;
            }
            else
            {
                Bounds = new Rectangle(0, 0, Game.Renderer.Resolution.Width, Game.Renderer.Resolution.Height);
                showWidget.Visible = false;
                NodeWidget.Visible = !NodeWidget.Visible;
            }
        }
    }
}