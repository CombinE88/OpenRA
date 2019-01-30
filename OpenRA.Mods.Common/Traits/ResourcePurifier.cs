using System;
using OpenRA.Mods.Common.Effects;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits
{
    [Desc("A actor has to enter the building before the unit spawns.")]
    public class ResourcePurifierInfo : ITraitInfo
    {
        public readonly int Percentage = 10;
        public readonly bool ShowTicks = true;
        public readonly int TickLifetime = 30;

        public object Create(ActorInitializer init)
        {
            return new ResourcePurifier();
        }
    }

    class ResourcePurifier
    {
        public ResourcePurifier()
        {
        }
    }
}