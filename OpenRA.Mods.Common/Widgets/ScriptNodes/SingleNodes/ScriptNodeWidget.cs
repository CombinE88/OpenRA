using System.Drawing;
using OpenRA.Graphics;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes.SingleNodes
{
    public class ScriptNodeWidget : Widget
    {
        public SpriteFont FontRegular;
        public SpriteFont FontSmall;
        public World World;
        public WorldRenderer WorldRenderer;
        public NodeEditorBackgroundWidget NodeWidget;
        public ModData ModData;
        ShowWidgetsButtonWidget showWidget;

        [ObjectCreator.UseCtor]
        public ScriptNodeWidget(World world, WorldRenderer worldRenderer, ModData modData)
        {
            World = world;
            WorldRenderer = worldRenderer;
            ModData = modData;
            Game.Renderer.Fonts.TryGetValue("Regular", out FontRegular);
            Game.Renderer.Fonts.TryGetValue("Small", out FontSmall);

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