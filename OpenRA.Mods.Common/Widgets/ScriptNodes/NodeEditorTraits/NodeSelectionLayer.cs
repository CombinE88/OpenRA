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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Graphics;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits
{
    public enum CellPicking
    {
        Path,
        Array,
        Single,
        Range,
        Actor,
        none
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
        public CPos FixedCursorPosition;
        WDist yetCursorPosition;
        public CellPicking Mode;

        public List<EditorActorPreview> Actors = new List<EditorActorPreview>();

        public List<CPos> CellRegion;

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
            else if (Mode != CellPicking.Array)
                CellRegion.Add(add);
        }

        public void SetRange(WDist range)
        {
            yetCursorPosition = range;
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
            if (wr.World.Type != WorldType.Editor || Mode == CellPicking.Path)
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
                            wcr.DrawLine(
                                new float3(CellRegion[i - 1].X * self.World.Map.Grid.TileSize.Width + self.World.Map.Grid.TileSize.Width / 2,
                                    CellRegion[i - 1].Y * self.World.Map.Grid.TileSize.Height + self.World.Map.Grid.TileSize.Height / 2, 1),
                                new float3(CellRegion[i].X * self.World.Map.Grid.TileSize.Width + self.World.Map.Grid.TileSize.Width / 2,
                                    CellRegion[i].Y * self.World.Map.Grid.TileSize.Height + self.World.Map.Grid.TileSize.Height / 2, 1),
                                2 + (CellRegion.Count / i) / 2,
                                Color.Black);
                            wcr.DrawLine(
                                new float3(CellRegion[i - 1].X * self.World.Map.Grid.TileSize.Width + self.World.Map.Grid.TileSize.Width / 2,
                                    CellRegion[i - 1].Y * self.World.Map.Grid.TileSize.Height + self.World.Map.Grid.TileSize.Height / 2, 1),
                                new float3(CellRegion[i].X * self.World.Map.Grid.TileSize.Width + self.World.Map.Grid.TileSize.Width / 2,
                                    CellRegion[i].Y * self.World.Map.Grid.TileSize.Height + self.World.Map.Grid.TileSize.Height / 2, 1),
                                1 + (CellRegion.Count / i) / 2,
                                Color.DarkGreen, Color.GreenYellow);
                        }
                    }
                }
            }

            if (Mode == CellPicking.Actor)
                foreach (var preview in Actors)
                {
                    if (preview != null)
                        wcr.DrawRect(new float3(preview.Bounds.Left, preview.Bounds.Top, 1), new float3(preview.Bounds.Right, preview.Bounds.Bottom, 1), 1, Color.White);
                }

            if (Mode == CellPicking.Range && yetCursorPosition.Length > 0 && CellRegion != null && CellRegion.Any())
            {
                List<float3> floats = new List<float3>();
                for (int i = 0; i < 32; i++)
                {
                    floats.Add(new float3(
                        (float)Math.Cos(Math.PI / 180 * 360 / 32 * i) *
                        yetCursorPosition.Length / self.World.Map.Grid.TileSize.Width +
                        CellRegion.First().X * self.World.Map.Grid.TileSize.Width +
                        self.World.Map.Grid.TileSize.Width / 2,
                        (float)Math.Sin(Math.PI / 180 * 360 / 32 * i) *
                        yetCursorPosition.Length / self.World.Map.Grid.TileSize.Height +
                        CellRegion.First().Y * self.World.Map.Grid.TileSize.Height +
                        self.World.Map.Grid.TileSize.Height / 2,
                        1f));
                }

                wcr.DrawPolygon(floats.ToArray(), 2f, Color.White);
            }
        }
    }
}