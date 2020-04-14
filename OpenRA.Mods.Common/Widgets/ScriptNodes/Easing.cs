using System;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public static class Easing
    {
        public static double InOutSine(double x)
        {
            return -(Math.Cos(Math.PI * x) - 1) / 2;
        }
    }
}