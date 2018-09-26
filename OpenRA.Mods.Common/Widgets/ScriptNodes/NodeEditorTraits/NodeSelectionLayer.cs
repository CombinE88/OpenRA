#region Copyright & License Information

/*
 * Copyright 2007-2018 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */

#endregion

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits
{
    public enum CellPicking
    {
        Path,
        Array,
        Single
    }

    [Desc("Required for the map editor to work. Attach this to the world actor.")]
    public class NodeSelectionLayerInfo : ITraitInfo
    {
        [PaletteReference] [Desc("Palette to use for rendering the placement sprite.")]
        public readonly string Palette = TileSet.TerrainPaletteInternalName;

        [Desc("Sequence image where the selection overlay types are defined.")]
        public readonly string Image = "editor-overlay";

        [SequenceReference("Image")] [Desc("Sequence to use for the copy overlay.")]
        public readonly string CellSequence = "copy";

        public virtual object Create(ActorInitializer init)
        {
            return new NodeSelectionLayer(init.Self, this);
        }
    }

    public class NodeSelectionLayer : IWorldLoaded, IRenderAboveWorld, IRenderAboveShroud
    {
        readonly NodeSelectionLayerInfo info;
        readonly Map map;
        readonly Sprite cellSprite;
        PaletteReference palette;

        public CellPicking Mode;
        public List<CPos> CellRegion { get; private set; }

        public NodeSelectionLayer(Actor self, NodeSelectionLayerInfo info)
        {
            if (self.World.Type != WorldType.Editor)
                return;

            this.info = info;
            map = self.World.Map;
            cellSprite = map.Rules.Sequences.GetSequence(info.Image, info.CellSequence).GetSprite(0);
            CellRegion = new List<CPos>();
        }

        void IWorldLoaded.WorldLoaded(World w, WorldRenderer wr)
        {
            if (w.Type != WorldType.Editor)
                return;

            palette = wr.Palette(info.Palette);
        }

        public void AddCell(CPos add)
        {
            if (!CellRegion.Contains(add) && Mode == CellPicking.Array)
                CellRegion.Add(add);
            else
                CellRegion.Add(add);
        }

        public void RemoveCell(CPos add)
        {
            if (CellRegion.Contains(add) && Mode == CellPicking.Array)
                CellRegion.Remove(add);
            else if (CellRegion.Any() && Mode == CellPicking.Path)
                CellRegion.Remove(CellRegion.Last());
        }

        public void LoadInPath(List<CPos> path)
        {
            CellRegion = path;
        }

        public void Clear()
        {
            CellRegion = new List<CPos>();
        }

        void IRenderAboveWorld.RenderAboveWorld(Actor self, WorldRenderer wr)
        {
            if (wr.World.Type != WorldType.Editor)
                return;

            if (CellRegion != null && CellRegion.Any())
            {
                foreach (var c in CellRegion)
                    new SpriteRenderable(cellSprite, wr.World.Map.CenterOfCell(c),
                        WVec.Zero, -511, palette, 1f, true).PrepareRender(wr).Render(wr);
            }
        }

        public bool SpatiallyPartitionable { get; private set; }

        public IEnumerable<IRenderable> RenderAboveShroud(Actor self, WorldRenderer wr)
        {
            if (wr.World.Type != WorldType.Editor)
                yield break;

            var wcr = Game.Renderer.WorldRgbaColorRenderer;
            if (CellRegion != null && CellRegion.Any())
            {
                if (Mode == CellPicking.Path)
                {
                    for (int i = 0; i < CellRegion.Count; i++)
                    {
                        if (i >= 1)
                        {
                            wcr.DrawLine(new float3(CellRegion[i - 1].X * self.World.Map.Grid.TileSize.Width, CellRegion[i - 1].Y * self.World.Map.Grid.TileSize.Height, 1),
                                new float3(CellRegion[i].X * self.World.Map.Grid.TileSize.Width, CellRegion[i].Y * self.World.Map.Grid.TileSize.Height, 1),
                                2 + (CellRegion.Count / i) / 2,
                                Color.Black);
                            wcr.DrawLine(new float3(CellRegion[i - 1].X * self.World.Map.Grid.TileSize.Width, CellRegion[i - 1].Y * self.World.Map.Grid.TileSize.Height, 1),
                                new float3(CellRegion[i].X * self.World.Map.Grid.TileSize.Width, CellRegion[i].Y * self.World.Map.Grid.TileSize.Height, 1),
                                1 + (CellRegion.Count / i) / 2,
                                Color.DarkGreen, Color.GreenYellow);
                        }
                    }
                }
            }

            yield break;
        }
    }
}