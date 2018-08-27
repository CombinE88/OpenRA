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
using OpenRA.Graphics;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits
{
    class Sculpt
    {
        public int Corner;
        public int Radius;
        public CPos Pos;
        public int Rot;
        public Color Color;
        public Size Size;

        public int Sliceradius;
        public int Slice;
        public int Slicerot;

        public Sculpt(int corner, int radius, CPos pos, int rot, Color color, Size size, int slice, int sliceradius, int slicerot)
        {
            Corner = corner;
            Radius = radius;
            Pos = pos;
            Rot = rot;
            Color = color;
            Size = size;
            Sliceradius = sliceradius;
            Slice = slice;
            Slicerot = slicerot;
        }

        public void Render()
        {
            for (var c = 0; c < Slice; c++)
            {
                var sclicerotation = 360 / Slice * c + Slicerot;

                var slicespos = new float2(
                    (float)(Math.Cos(Math.PI / 180 * sclicerotation) * Sliceradius + Pos.X) * Size.Width,
                    (float)(Math.Sin(Math.PI / 180 * sclicerotation) * Sliceradius + Pos.Y) * Size.Height);

                var verts = new List<float3>();

                for (var i = 0; i < Corner; i++)
                {
                    var singlerotation = 360 / Corner * i + Rot;

                    verts.Add(new float3(
                        (float)Math.Cos(Math.PI / 180 * (singlerotation + sclicerotation)) * Radius * Size.Width + slicespos.X,
                        (float)Math.Sin(Math.PI / 180 * (singlerotation + sclicerotation)) * Radius * Size.Height + slicespos.Y,
                        1));
                }

                var wcr = Game.Renderer.WorldRgbaColorRenderer;
                wcr.DrawPolygon(verts.ToArray(), 4, Color.FromArgb(Color.A, 0, 0, 0));
                wcr.DrawPolygon(verts.ToArray(), 2, Color);
            }
        }
    }

    class SculptlayerInfo : ITraitInfo
    {
        public object Create(ActorInitializer init)
        {
            return new Sculptlayer();
        }
    }

    class Sculptlayer : IRenderAboveShroud
    {
        public Dictionary<string, Sculpt> Sculpts = new Dictionary<string, Sculpt>();
        public bool SpatiallyPartitionable { get; private set; }

        public IEnumerable<IRenderable> RenderAboveShroud(Actor self, WorldRenderer wr)
        {
            foreach (var sculpt in Sculpts.Values)
                sculpt.Render();

            yield break;
        }
    }
}