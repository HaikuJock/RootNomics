using Microsoft.Xna.Framework;
using System;

namespace Haiku.MonoGameUI
{
    public class Hsv
    {
        public const float UndefinedHue = -1.0f;
        const float Inverse180 = 1.0f / 180.0f;

        public readonly float H;    // [0, 360]. Exception: H is returned UndefinedHue if S==0.
        public readonly float S;    // [0, 1]
        public readonly float V;    // [0, 1]
        public readonly float A;    // [0, 1]

        public static Hsv FromColor(Color rgb)
        {
            float red = rgb.R / 255.0f;
            float green = rgb.G / 255.0f;
            float blue = rgb.B / 255.0f;
            float alpha = rgb.A / 255.0f;
            float Maximum;
            float Minimum;
            float dominantDifference;

            Minimum = Math.Min(Math.Min(red, green), blue);
            Maximum = Math.Max(Math.Max(red, green), blue);

            if (Maximum == Minimum)
            {
                return new Hsv(UndefinedHue, 0, Maximum, alpha);   // greyscale
            }
            if (Maximum == 0.0f)
            {
                return new Hsv(UndefinedHue, 0, Maximum, alpha);   // (black) if V = 0, then S is undefined.
            }

            dominantDifference = (red == Maximum) ? green - blue : ((green == Maximum) ? blue - red : red - green);

            // order is important here
            float hueRotation = red == Maximum && green >= blue
                ? 0.0f
                : red == Maximum ? 360.0f : green == Maximum ? 120.0f : 240.0f;
            return new Hsv(
                (dominantDifference / (Maximum - Minimum)) * 60.0f + hueRotation, 
                (Maximum - Minimum) / Maximum, 
                Maximum, 
                alpha);
        }

        public static Color LerpColors(Color rgbFrom, Color rgbTo, float t)
        {
            Hsv hsv1 = FromColor(rgbFrom);
            Hsv hsv2 = FromColor(rgbTo);
            Hsv hsvResult = hsv1.Lerp(hsv2, t);

            return hsvResult.ToColor();
        }

        public Hsv() : this(UndefinedHue, 0.0f, 0.0f)
        { }

        public Hsv(float h, float s, float v) : this(h, s, v, 1.0f)
        { }

        public Hsv(float h, float s, float v, float a)
        {
            if (h != UndefinedHue)
            {
                while (h < 0.0f)
                {
                    h += 360.0f;
                }
                while (h > 360.0f)
                {
                    h -= 360.0f;
                }
            }
            H = h;
            S = MathHelper.Clamp(s, 0.0f, 1.0f);
            V = MathHelper.Clamp(v, 0.0f, 1.0f);
            A = MathHelper.Clamp(a, 0.0f, 1.0f);
        }


        public Color ToColor()
        {
            int hi = Convert.ToInt32(Math.Floor(H / 60.0f)) % 6;
            float f = H / 60.0f - (float)Math.Floor(H / 60.0f);
            float s = S;
            float v = V;
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);

            if (hi == 0)
                return new Color(v, t, p, A);
            else if (hi == 1)
                return new Color(q, v, p, A);
            else if (hi == 2)
                return new Color(p, v, t, A);
            else if (hi == 3)
                return new Color(p, q, v, A);
            else if (hi == 4)
                return new Color(t, p, v, A);
            else
                return new Color(v, p, q, A);
        }

        // H on [0,360], S on [0,1], V on [0,1]
        public float EuclidDistanceSqrd(Hsv rhs)
        {
            float sDiff = S - rhs.S;
            float vDiff = V - rhs.V;
            float hDiff = HueDistance(rhs);

            // scale hue to be on 0-1 so it is as important as the other channels
            hDiff *= Inverse180;

            return hDiff * hDiff + sDiff * sDiff + vDiff * vDiff;
        }

        public float ManhattanDistance(Hsv rhs)
        {
            float sDiff = Math.Abs(S - rhs.S);
            float vDiff = Math.Abs(V - rhs.V);
            float hDiff = HueDistance(rhs);

            // scale hue to be on 0-1 so it is as important as the other channels
            hDiff *= Inverse180;

            return hDiff + sDiff + vDiff;
        }

        public float WeightedManhattanDistance(Hsv rhs, float fHueWeight, float fSatWeight, float fValWeight)
        {
            float sDiff = Math.Abs(S - rhs.S);
            float vDiff = Math.Abs(V - rhs.V);
            float hDiff = HueDistance(rhs);

            // scale hue to be on 0-1 so it is as important as the other channels
            hDiff *= Inverse180;

            return hDiff * fHueWeight + sDiff * fSatWeight + vDiff * fValWeight;
        }

        public float HueDistance(Hsv rhs)
        {
            if (H == UndefinedHue && rhs.H == UndefinedHue)
            {
                return 0.0f;
            }
            else if (H == UndefinedHue || rhs.H == UndefinedHue)
            {
                // rather than some arbitrary value, 
                // return the average of the intensity and saturation difference
                return (Math.Abs(S - rhs.S) + Math.Abs(V - rhs.V)) * 90.0f; // scaled on 0-180
            }
            else
            {
                float hDiff = Math.Abs(H - rhs.H);
                if (hDiff > 180.0f)
                    hDiff = 360.0f - hDiff;
                return hDiff;
            }
        }

        public Hsv Complementary()
        {
            float complementaryHue = H + 180.0f;
            while (complementaryHue > 360.0f)
            {
                complementaryHue -= 360.0f;
            }
            return new Hsv(complementaryHue, S, V);
        }

        public Hsv SplitComplementary(float complementarySpace)
        {
            float splitComplementaryHue = H + 180.0f + complementarySpace * 0.5f;
            while (splitComplementaryHue > 360.0f)
            {
                splitComplementaryHue -= 360.0f;
            }
            while (splitComplementaryHue < 0.0f)
            {
                splitComplementaryHue += 360.0f;
            }
            return new Hsv(splitComplementaryHue, S, V);
        }

        public Hsv NegativeTriad()
        {
            float negativeTriadHue = H - 120.0f;
            while (negativeTriadHue < 0.0f)
            {
                negativeTriadHue += 360.0f;
            }
            return new Hsv(negativeTriadHue, S, V);
        }

        public Hsv PositiveTriad()
        {
            float positiveTriadHue = H + 120.0f;
            while (positiveTriadHue > 360.0f)
            {
                positiveTriadHue -= 360.0f;
            }
            return new Hsv(positiveTriadHue, S, V);
        }

        public Hsv Lerp(Hsv toHSV, float t)
        {
            if (H == UndefinedHue && toHSV.H == UndefinedHue)
            {
                return new Hsv(UndefinedHue, S + (toHSV.S - S) * t, V + (toHSV.V - V) * t);
            }
            else if (H == UndefinedHue)
            {
                return new Hsv(toHSV.H, S + (toHSV.S - S) * t, V + (toHSV.V - V) * t);
            }
            else if (toHSV.H == UndefinedHue)
            {
                return new Hsv(H, S + (toHSV.S - S) * t, V + (toHSV.V - V) * t);
            }
            else
            {
                float hDiff = toHSV.H - H;
                if (hDiff > 180.0f)
                {
                    hDiff = -(360.0f - hDiff);
                }
                else if (hDiff < -180.0f)
                {
                    hDiff = 360.0f + hDiff;
                }

                float h = H + hDiff * t;
                while (h > 360.0f)
                {
                    h -= 360.0f;
                }
                while (h < 0.0f)
                {
                    h += 360.0f;
                }
                h = MathHelper.Clamp(h, 0.0f, 360.0f);
                return new Hsv(h, S + (toHSV.S - S) * t, V + (toHSV.V - V) * t);
            }
        }
    }
}
