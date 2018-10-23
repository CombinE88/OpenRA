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

        [ObjectCreator.UseCtor]
        public ScriptNodeWidget(World world, WorldRenderer worldRenderer, ModData modData)
        {
            World = world;
            WorldRenderer = worldRenderer;
            ModData = modData;
            Game.Renderer.Fonts.TryGetValue("Regular", out FontRegular);
            Game.Renderer.Fonts.TryGetValue("Small", out FontSmall);

            Children.Add(new ShowWidgetsButtonWidget(this));
            Children.Add(NodeWidget = new NodeEditorBackgroundWidget(this, worldRenderer, world) { Visible = false });
        }

        public override void Tick()
        {
            Bounds = new Rectangle(0, 0, Game.Renderer.Resolution.Width, Game.Renderer.Resolution.Height);
        }
    }
}