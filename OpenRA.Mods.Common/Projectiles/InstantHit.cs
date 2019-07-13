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

using System.Collections.Generic;
using System.Linq;
using OpenRA.GameRules;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Traits;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Projectiles
{
	[Desc("Simple, invisible, usually direct-on-target projectile.")]
	public class InstantHitInfo : IProjectileInfo
	{
		[Desc("Maximum offset at the maximum range.")]
		public readonly WDist Inaccuracy = WDist.Zero;

		[Desc("Projectile can be blocked.")]
		public readonly bool Blockable = false;

		[Desc("The closer the blocking actor is to the source, the less likely it will block.")]
		public readonly bool AdaptiveBlockable = false;

		[Desc("Types this projectile can be blocked by.")]
		public readonly string[] BlockTypes = { "wall" };

		[Desc("Actors in this range to the source will never block the projectile.")]
		public readonly WDist MinBLockRange = new WDist(512);

		[Desc("probability this projectile will be blocked 0-100%")]
		public readonly int BlockChance = 100;

		[Desc("Actors with these stances will be ignored and not blocking this projectile")]
		public readonly Stance[] IgnoreStance = { };

		[Desc("The width of the projectile.")]
		public readonly WDist Width = new WDist(1);

		[Desc("Scan radius for actors with projectile-blocking trait. If set to a negative value (default), it will automatically scale",
			"to the blocker with the largest health shape. Only set custom values if you know what you're doing.")]
		public WDist BlockerScanRadius = new WDist(-1);

		public IProjectile Create(ProjectileArgs args) { return new InstantHit(this, args); }
	}

	public class InstantHit : IProjectile
	{
		readonly ProjectileArgs args;
		readonly InstantHitInfo info;

		List<Actor> ignoredActors = new List<Actor>();
		List<Actor> checkedActors = new List<Actor>();

		Target target;

		public InstantHit(InstantHitInfo info, ProjectileArgs args)
		{
			this.args = args;
			this.info = info;

			if (args.Weapon.TargetActorCenter)
				target = args.GuidedTarget;
			else if (info.Inaccuracy.Length > 0)
			{
				var inaccuracy = Util.ApplyPercentageModifiers(info.Inaccuracy.Length, args.InaccuracyModifiers);
				var maxOffset = inaccuracy * (args.PassiveTarget - args.Source).Length / args.Weapon.Range.Length;
				target = Target.FromPos(args.PassiveTarget + WVec.FromPDF(args.SourceActor.World.SharedRandom, 2) * maxOffset / 1024);
			}
			else
				target = Target.FromPos(args.PassiveTarget);
		}

		public void Tick(World world)
		{
			// Check for blocking actors
			WPos blockedPos;
			if (info.Blockable && BlocksProjectiles.AnyBlockingActorsBetween(
				    world,
				    checkedActors,
				    ignoredActors,
				    args,
				    info.IgnoreStance,
				    info.BlockChance,
				    info.AdaptiveBlockable,
				    info.MinBLockRange,
				    info.BlockTypes,
				    args.Source,
				    target.CenterPosition,
				    info.Width,
				    out blockedPos,
				    out ignoredActors,
				    out checkedActors))
			{
				target = Target.FromPos(blockedPos);
			}

			args.Weapon.Impact(target, args.SourceActor, args.DamageModifiers);
			world.AddFrameEndTask(w => w.Remove(this));
		}

		public IEnumerable<IRenderable> Render(WorldRenderer wr)
		{
			return Enumerable.Empty<IRenderable>();
		}
	}
}
