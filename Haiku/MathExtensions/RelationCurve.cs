using System;
using RelationD = System.Func<double, double>;

namespace Haiku.MathExtensions
{
    public enum RCurve : byte
    {
        Linear,
        Inverse,
        QuadEaseInOut,
        BezierEaseInOut,
        ParametricEaseInOut,
        QuadEaseIn,
        QuadEaseOut,
        QuartEaseIn,
        QuartEaseOut,
        EaseInElasticBounce,
        EaseOutElasticBounce,
        EaseInOutElasticBounce,
        EaseOutQuint,
        NaturalPower,
        SimplePower,
    }

    public static class RelationCurve
    {
        public static readonly RelationD[] Fn = new RelationD[]
        {
            (x) => x,
            (x) => 1 / x,
            (x) => (x < 0.5) ? 2 * x * x : (4 - 2 * x) * x - 1,
            (x) => Math.Sqrt(x) * (3 - 2 * x),
            (x) =>
            {
                var sqt = Math.Sqrt(x);
                return sqt / (2 * (sqt - x) + 1);
            },
            (x) => x * x,
            (x) => x * (2 - x),
            (x) => x * x * x * x,
            (x) =>
            {
                var oneMinusPortion = 1 - x;
                return 1 - oneMinusPortion * oneMinusPortion * oneMinusPortion * oneMinusPortion;
            },
            // elastic bounce effect at the beginning - easeInElastic
            (x) => (0.04 - 0.04 / x) * Math.Sin(25 * x) + 1,
            // elastic bounce effect at the end - easeOutElastic
            (x) => 0.04 * x / (--x) * Math.Sin(25 * x),
            // elastic bounce effect at the beginning and end - easeInOutElastic
            (t) => (t -= .5) < 0 ? (.02 + .01 / t) * Math.Sin(50 * t) : (.02 - .01 / t) * Math.Sin(50 * t) + 1,
            // Quint: close approximation to a power-curve, upside down
            (t) => 1 + (--t)*t*t*t*t,
            // y=a\cdot\ln\left(x+1\right)^{b}
            // y = a * Math.Pow(ln(x + 1), b) where a = 0.01 and b = -1
            // Approximation of a power curve on 0...1
            (t) => 0.01 / Math.Log(t + 1),
            (t) => 0.01 / t,
        };
    }
}
