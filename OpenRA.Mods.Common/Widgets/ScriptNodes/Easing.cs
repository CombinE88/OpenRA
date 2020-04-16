using System;

namespace OpenRA.Mods.Common.Widgets.ScriptNodes
{
    public static class Easing
    {
        public static double InOutSine(double x)
        {
            return -(Math.Cos(Math.PI * x) - 1) / 2;
        }

        public static double InOutQuart(double x)
        {
            return x < 0.5 ? 8 * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 4) / 2;
        }

        public static double InOutCubic(double x)
        {
            return x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2;
        }
    }
}