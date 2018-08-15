using System.Collections.Generic;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits
{
    class EditorBrushLayerInfo : ITraitInfo
    {
        public readonly string CheckOnBuilding = "";

        [PaletteReference] [Desc("Palette to use for rendering the dissabled sprite.")]
        public readonly string Palette = TileSet.TerrainPaletteInternalName;

        public readonly string Image = "editor-overlay";

        public readonly string PasteSequence = "paste";

        public readonly string CopySequence = "copy";

        public object Create(ActorInitializer init)
        {
            return new EditorBrushLayer(init, this);
        }
    }

    class EditorBrushLayer : IRenderAboveShroud, IWorldLoaded
    {
        public int Brushsize = 5;
        public string Methode = "none";

        public Sprite EnableOverlay { get; private set; }
        public Sprite DissableOverlay { get; private set; }
        public Sprite CurrentOverlay;

        public HashSet<CPos> Cells = new HashSet<CPos>();
        public HashSet<CPos> SelectedCells { get; private set; }

        EditorBrushLayerInfo info;
        PaletteReference palette;

        public EditorBrushLayer(ActorInitializer init, EditorBrushLayerInfo info)
        {
            if (init.Self.World.Type != WorldType.Editor)
                return;
            this.info = info;

            EnableOverlay = init.Self.World.Map.Rules.Sequences.GetSequence(info.Image, info.CopySequence).GetSprite(0);
            DissableOverlay = init.Self.World.Map.Rules.Sequences.GetSequence(info.Image, info.PasteSequence).GetSprite(0);
            CurrentOverlay = EnableOverlay;
        }

        void IWorldLoaded.WorldLoaded(World w, WorldRenderer wr)
        {
            if (w.Type != WorldType.Editor)
                return;

            SelectedCells = new HashSet<CPos>();
            palette = wr.Palette(info.Palette);
        }

        public void Render(HashSet<CPos> cells, WorldRenderer wr, Sprite sprite, PaletteReference palette)
        {
            foreach (var cell in cells)
            {
                new SpriteRenderable(sprite, wr.World.Map.CenterOfCell(cell),
                    WVec.Zero, -511, palette, 1f, true).PrepareRender(wr).Render(wr);
            }
        }

        public IEnumerable<IRenderable> RenderAboveShroud(Actor self, WorldRenderer wr)
        {
            if (Methode != "square" && Methode != "circle")
                yield break;

            var center = new CPos
            (
                wr.Viewport.ViewToWorld(Viewport.LastMousePos).X,
                wr.Viewport.ViewToWorld(Viewport.LastMousePos).Y
            );

            if (Methode == "circle")
            {
                Cells = self.World.Map.FindTilesInCircle(center, Brushsize).ToHashSet();
            }

            if (Methode == "square")
            {
                Cells = self.World.Map.AllCells
                    .Where(c =>
                        c.X <= center.X + Brushsize
                        && c.X >= center.X - Brushsize
                        && c.Y <= center.Y + Brushsize
                        && c.Y >= center.Y - Brushsize
                    )
                    .ToHashSet();
            }

            HashSet<CPos> viewcells = Cells.Any() ? Cells : new HashSet<CPos>();
            HashSet<CPos> viewselectedcells = SelectedCells.Any() ? SelectedCells : new HashSet<CPos>();

            if (viewcells.Any() && viewselectedcells.Any())
            {
                foreach (var cell in viewcells)
                {
                    if (!viewselectedcells.Contains(cell))
                        viewselectedcells.Add(cell);
                }
            }

            if (viewcells.Any())
                Render(viewcells, wr, CurrentOverlay, palette);
            if (viewselectedcells.Any())
                Render(viewselectedcells, wr, DissableOverlay, palette);
        }

        public bool SpatiallyPartitionable { get; private set; }

        public void OnLeftClick()
        {
            Methode = "none";
            Cells = new HashSet<CPos>();
        }

        public void OnRightClick()
        {
            foreach (var cell in Cells)
            {
                if (!SelectedCells.Contains(cell))
                    SelectedCells.Add(cell);
            }
        }

        public void OnShiftLeftClick()
        {
            foreach (var cell in Cells)
            {
                if (SelectedCells.Contains(cell))
                    SelectedCells.Remove(cell);
            }
        }

        public void ClearSelection()
        {
            Cells = new HashSet<CPos>();
            SelectedCells = new HashSet<CPos>();
        }
    }
}