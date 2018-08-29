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

using System.Drawing;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits.Render
{
    public class WithMoveAnimationInfo : ConditionalTraitInfo, Requires<WithSpriteBodyInfo>, Requires<IMoveInfo>
    {
        [Desc("Displayed while moving.")] [SequenceReference]
        public readonly string MoveSequence = "move";

        [Desc("Which sprite body to modify.")] public readonly string Body = "body";

        public override object Create(ActorInitializer init)
        {
            return new WithMoveAnimation(init, this);
        }

        public override void RulesetLoaded(Ruleset rules, ActorInfo ai)
        {
            var matches = ai.TraitInfos<WithSpriteBodyInfo>().Count(w => w.Name == Body);
            if (matches != 1)
                throw new YamlException("WithMoveAnimation needs exactly one sprite body with matching name.");

            base.RulesetLoaded(rules, ai);
        }
    }

    public class WithMoveAnimation : ConditionalTrait<WithMoveAnimationInfo>, ITick
    {
        readonly IMove movement;
        readonly WithSpriteBody wsb;
        string moveanimation;
        private WithHarvestAnimation harvinfo;

        public WithMoveAnimation(ActorInitializer init, WithMoveAnimationInfo info)
            : base(info)
        {
            movement = init.Self.Trait<IMove>();
            wsb = init.Self.TraitsImplementing<WithSpriteBody>().Single(w => w.Info.Name == Info.Body);
            moveanimation = info.MoveSequence;
            harvinfo = init.Self.TraitOrDefault<WithHarvestAnimation>();
        }

        void ITick.Tick(Actor self)
        {
            if (IsTraitDisabled || wsb.IsTraitDisabled)
                return;

            var isMoving = movement.IsMoving && !self.IsDead;

            if (!isMoving)
                return;

            if (wsb.DefaultAnimation.CurrentSequence.Name == moveanimation)
                return;

            moveanimation = self.Info.HasTraitInfo<WithHarvestAnimationInfo>() ? NormalizeMoveSequence(self, Info.MoveSequence) : Info.MoveSequence;

            wsb.DefaultAnimation.ReplaceAnim(moveanimation);
        }

        string NormalizeMoveSequence(Actor self, string baseSequence)
        {
            if (harvinfo != null)
            {
                var desiredState = harvinfo.Harv.Fullness * (harvinfo.Info.PrefixByFullness.Length - 1) / 100;
                var desiredPrefix = harvinfo.Info.PrefixByFullness[desiredState];

                if (wsb.DefaultAnimation.HasSequence(desiredPrefix + baseSequence))
                    return desiredPrefix + baseSequence;
            }

            return baseSequence;
        }
    }
}