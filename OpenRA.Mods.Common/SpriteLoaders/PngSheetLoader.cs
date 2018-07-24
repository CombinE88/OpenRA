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
using System.Drawing;
using System.IO;
using OpenRA.FileFormats;
using OpenRA.Graphics;
using OpenRA.Mods.Common.FileFormats;

namespace OpenRA.Mods.Common.SpriteLoaders
{
	public class PngSheetLoader : ISpriteLoader
	{
		class PngSheetFrame : ISpriteFrame
		{
			public Size Size { get; set; }
			public Size FrameSize { get; set; }
			public float2 Offset { get; set; }
			public byte[] Data { get; set; }
			public bool DisableExportPadding { get { return false; } }
		}

		public bool TryParseSprite(Stream s, out ISpriteFrame[] frames)
		{
			if (!Png.Verify(s))
			{
				frames = null;
				return false;
			}

			var png = new Png(s);

			var frameAmount = 0;
			Rectangle[] frameRegions;
			float2[] frameOffsets;

			while (png.Meta.ContainsKey("Frame[" + frameAmount + "]"))
				frameAmount++;

			if (frameAmount > 0)
			{
				frameRegions = new Rectangle[frameAmount];
				frameOffsets = new float2[frameAmount];

				for (var i = 0; i < frameAmount; i++)
				{
					var coords = png.Meta["Frame[" + i + "]"].Split(',');

					frameRegions[i] = new Rectangle(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]), int.Parse(coords[3]));
					frameOffsets[i] = new float2(int.Parse(coords[4]), int.Parse(coords[5]));
				}
			}
			else
			{
				Size frameSize;

				if (png.Meta.ContainsKey("FrameSize"))
				{
					var dimensions = png.Meta["FrameSize"].Split(',');
					frameSize = new Size(int.Parse(dimensions[0]), int.Parse(dimensions[1]));

					if (png.Meta.ContainsKey("FrameAmount"))
						frameAmount = int.Parse(png.Meta["FrameAmount"]);
					else
						frameAmount = png.Width / frameSize.Width * png.Height / frameSize.Height;
				}
				else
				{
					if (png.Meta.ContainsKey("FrameAmount"))
					{
						frameAmount = int.Parse(png.Meta["FrameAmount"]);
						frameSize = new Size(png.Width / frameAmount, png.Height);
					}
					else
					{
						frameAmount = 1;
						frameSize = new Size(png.Width, png.Height);
					}
				}

				float2 offset;

				if (png.Meta.ContainsKey("Offset"))
				{
					var coords = png.Meta["Offset"].Split(',');
					offset = new float2(int.Parse(coords[0]), int.Parse(coords[1]));
				}
				else
					offset = new float2(0, 0);

				frameRegions = new Rectangle[frameAmount];
				frameOffsets = new float2[frameAmount];

				var framesPerRow = png.Width / frameSize.Width;

				for (var i = 0; i < frameAmount; i++)
				{
					var x = i % framesPerRow * frameSize.Width;
					var y = i / framesPerRow * frameSize.Height;

					frameRegions[i] = new Rectangle(x, y, frameSize.Width, frameSize.Height);
					frameOffsets[i] = offset;
				}
			}

			frames = new ISpriteFrame[frameAmount];

			for (var i = 0; i < frameAmount; i++)
			{
				var frame = new PngSheetFrame();
				frame.FrameSize = frame.Size = new Size(frameRegions[i].Width, frameRegions[i].Height);
				frame.Offset = frameOffsets[i];
				frame.Data = new byte[frame.Size.Width * frame.Size.Height];

				var frameStart = frameRegions[i].X + frameRegions[i].Y * png.Width;

				for (var y = 0; y < frame.Size.Height; y++)
					Array.Copy(png.Data, frameStart + y * png.Width, frame.Data, y * frame.Size.Width, frame.Size.Width);

				frames[i] = frame;
			}

			return true;
		}
	}
}
