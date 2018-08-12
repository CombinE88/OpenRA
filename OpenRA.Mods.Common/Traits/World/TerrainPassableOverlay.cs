using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits
{
    public class Cross

    {
        public Size Size;

        public Cross(Size size)
        {
            Size = size;
        }

        public void Render(HashSet<CPos> cells, WorldRenderer wr, Sprite sprite, PaletteReference palette)
        {
            foreach (var cell in cells)
            {
                new SpriteRenderable(sprite, wr.World.Map.CenterOfCell(cell),
                    WVec.Zero, -511, palette, 1f, true).PrepareRender(wr).Render(wr);
            }
        }
    }

    class TerrainPassableOverlayInfo : ITraitInfo
    {
        public readonly string CheckOnBuilding = "";

        [PaletteReference] [Desc("Palette to use for rendering the dissabled sprite.")]
        public readonly string Palette = TileSet.TerrainPaletteInternalName;

        [Desc("Sequence image where the toggle buildable overlay types are defined.")]
        public readonly string Image = "editor-overlay";

        [SequenceReference("Image")] [Desc("Sequence to use for toggle buildable area overlay.")]
        public readonly string PasteSequence = "paste";

        public object Create(ActorInitializer init)
        {
            return new TerrainPassableOverlay(init, this);
        }
    }

    class TerrainPassableOverlay : IRenderAboveShroud, IWorldLoaded
    {
        readonly Cross cross;
        readonly Sprite disabledSprite;

        TerrainPassableOverlayInfo info;
        public bool Enabled = false;

        PaletteReference palette;

        public TerrainPassableOverlay(ActorInitializer init, TerrainPassableOverlayInfo info)
        {
            if (init.Self.World.Type != WorldType.Editor)
                return;

            cross = new Cross(init.World.Map.Grid.TileSize);
            this.info = info;

            disabledSprite = init.Self.World.Map.Rules.Sequences.GetSequence(info.Image, info.PasteSequence).GetSprite(0);
        }

        void IWorldLoaded.WorldLoaded(World w, WorldRenderer wr)
        {
            if (w.Type != WorldType.Editor)
                return;

            palette = wr.Palette(info.Palette);
        }

        public IEnumerable<IRenderable> RenderAboveShroud(Actor self, WorldRenderer wr)
        {
            if (info.CheckOnBuilding == "" || !self.World.Map.Rules.Actors.ContainsKey(info.CheckOnBuilding) || !Enabled)
                yield break;

            var allowedTerrain = self.World.Map.Rules.Actors[info.CheckOnBuilding.ToLowerInvariant()].TraitInfo<BuildingInfo>().TerrainTypes;

            HashSet<CPos> cells = self.World.Map.AllCells
                .Where(c =>
                    self.World.Map.Contains(c)
                    && !allowedTerrain.Contains(self.World.Map.GetTerrainInfo(c).Type.ToString())
                    && wr.Viewport.VisibleCellsInsideBounds.Contains(new PPos(c.X, c.Y)))
                .ToHashSet();

            cross.Render(cells, wr, disabledSprite, palette);
        }

        public bool SpatiallyPartitionable { get; private set; }
    }
}