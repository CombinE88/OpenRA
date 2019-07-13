#region Copyright & License Information
/*
 * Copyright 2007-2019 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.GameRules;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits
{
	[Desc("This actor blocks bullets and missiles with 'Blockable' property.")]
	public class BlocksProjectilesInfo : ConditionalTraitInfo, IBlocksProjectilesInfo
	{
		public readonly WDist Height = WDist.FromCells(1);

		[Desc("Types of block-types this actor is blocking")]
		public readonly string[] BlockTypes = { "wall" };

		[Desc("probability this actor is blocking 0-100%")]
		public readonly int BlockChance = 100;

		public override object Create(ActorInitializer init) { return new BlocksProjectiles(init.Self, this); }
	}

	public class BlocksProjectiles : ConditionalTrait<BlocksProjectilesInfo>, IBlocksProjectiles
	{
		public BlocksProjectiles(Actor self, BlocksProjectilesInfo info)
			: base(info) { }

		WDist IBlocksProjectiles.BlockingHeight { get { return Info.Height; } }

		public static bool AnyBlockingActorAt(World world, WPos pos)
		{
			var dat = world.Map.DistanceAboveTerrain(pos);

			return world.ActorMap.GetActorsAt(world.Map.CellContaining(pos))
				.Any(a => a.TraitsImplementing<IBlocksProjectiles>()
					.Where(t => t.BlockingHeight > dat)
					.Any(Exts.IsTraitEnabled));
		}

		public static bool AnyBlockingActorsBetween(World world, WPos start, WPos end, WDist width, out WPos hit)
		{
			var actors = world.FindBlockingActorsOnLine(start, end, width);

			var length = (end - start).Length;

			foreach (var a in actors)
			{
				var blockers = a.TraitsImplementing<IBlocksProjectiles>()
					.Where(Exts.IsTraitEnabled).ToList();

				if (!blockers.Any())
					continue;

				var hitPos = WorldExtensions.MinimumPointLineProjection(start, end, a.CenterPosition);
				var dat = world.Map.DistanceAboveTerrain(hitPos);
				if ((hitPos - start).Length < length && blockers.Any(t => t.BlockingHeight > dat))
				{
					hit = hitPos;
					return true;
				}
			}

			hit = WPos.Zero;
			return false;
		}

		public static bool AnyBlockingActorsBetween(
			World world,
			List<Actor> ignore,
			List<Actor> alreadyChecked,
			ProjectileArgs args,
			Stance[] stances,
			int chance,
			bool adaptive,
			WDist minBlockRange,
			string[] blockType,
			WPos start,
			WPos end,
			WDist width,
			out WPos hit,
			out List<Actor> ignoreList,
			out List<Actor> checkedList)
		{
			var actors = world.FindBlockingActorsOnLine(start, end, width)
				.Where(a =>
					new WDist((args.Source - start).Length) < minBlockRange
					&& a != args.SourceActor
					&& !ignore.Contains(a)
					&& !stances.Contains(a.Owner.Stances[args.SourceActor.Owner]));

			var length = (end - start).Length;

			foreach (var a in actors)
			{
				var blockers = a.TraitsImplementing<IBlocksProjectiles>()
					.Where(Exts.IsTraitEnabled).ToList();

				if (!blockers.Any())
					continue;

				if (!alreadyChecked.Contains(a) && !ignore.Contains(a) && world.SharedRandom.Next(0, 100) >= chance)
					continue;

				var blocks = a.TraitsImplementing<BlocksProjectiles>();

				var blocking = false;

				foreach (var block in blocks)
				{
					if (!alreadyChecked.Contains(a) && !ignore.Contains(a) && world.SharedRandom.Next(0, 100) >= block.Info.BlockChance)
					{
						ignore.Add(a);
						continue;
					}

					foreach (var type in block.Info.BlockTypes)
						if (blockType.Contains(type))
							blocking = true;
				}

				// Check Adaptive distance, the longer the projectile travels the more likely it is to hit a blocking actor
				if (adaptive && !alreadyChecked.Contains(a) && !ignore.Contains(a))
				{
					var adaptiveChance = 100 / ((args.PassiveTarget - args.Source).Length - minBlockRange.Length) * ((start - args.Source).Length - minBlockRange.Length);

					if (world.SharedRandom.Next(0, 100) >= adaptiveChance)
					{
						ignore.Add(a);
						continue;
					}
				}

				if (!blocking)
					continue;

				alreadyChecked.Add(a);

				var hitPos = WorldExtensions.MinimumPointLineProjection(start, end, a.CenterPosition);
				var dat = world.Map.DistanceAboveTerrain(hitPos);
				if ((hitPos - start).Length < length && blockers.Any(t => t.BlockingHeight > dat))
				{
					hit = hitPos;
					ignoreList = ignore;
					checkedList = alreadyChecked;
					return true;
				}
			}

			hit = WPos.Zero;
			ignoreList = ignore;
			checkedList = alreadyChecked;
			return false;
		}
	}
}
