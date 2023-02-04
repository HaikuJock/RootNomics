using System;

namespace Haiku.MathExtensions
{
    public static class FloatExtensions
    {
        public static float Clamp(this float value, float min, float max)
        {
            return (value < min) ? min : (value > max ? max : value);
        }
    }
}
