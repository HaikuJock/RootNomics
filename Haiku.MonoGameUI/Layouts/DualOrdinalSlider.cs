using Haiku.MathExtensions;
using Microsoft.Xna.Framework;
using System;

namespace Haiku.MonoGameUI.Layouts
{
    public delegate void DualOrdinalSliderChangeHandler(int lower, int upper);

    public class DualOrdinalSlider : DualSlider
    {
        public new DualOrdinalSliderChangeHandler OnChanged;
        readonly int min;
        readonly int max;
        readonly float toPortion;
        readonly float fromPortion;
        int lowerValue;
        int upperValue;

        public DualOrdinalSlider(
            Rectangle frame, 
            int trackHeight, 
            Point indicatorSize,
            int min, 
            int max) 
            : base(frame, trackHeight, indicatorSize)
        {
            if (min > max)
            {
                var swap = max;
                max = min;
                min = swap;
            }

            this.min = min;
            this.max = max;
            lowerValue = min;
            upperValue = max;
            var range = max - min;

            if (range == 0)
            {
                toPortion = 1f;
            }
            else
            {
                toPortion = 1f / range;
            }
            fromPortion = 1 / toPortion;
        }

        public void SetValues(int lower, int upper)
        {
            var lowerPortion = lower * toPortion;
            var upperPortion = upper * toPortion;
            SetValues(lowerPortion, upperPortion);
            lowerValue = lower.Clamp(min, max);
            upperValue = upper.Clamp(min, max);
        }

        public override void ScrollLine(Layout scroller, int multiple)
        {
            if (scroller == indicatorLower)
            {
                ChangeLowerValue(multiple);
            }
            else
            {
                ChangeUpperValue(multiple);
            }
        }

        public override void Scroll(Layout scroller, int delta, bool animated = true)
        {
            base.Scroll(scroller, delta, animated);
            if (scroller == indicatorLower)
            {
                var newValue = (int)Math.Round(PortionLower * fromPortion);
                newValue = newValue.Clamp(min, max);
                if (lowerValue != newValue)
                {
                    lowerValue = newValue;
                    if (lowerValue > upperValue)
                    {
                        upperValue = lowerValue;
                    }
                    OnChanged?.Invoke(lowerValue, upperValue);
                }
            }
            else
            {
                var newValue = (int)Math.Round(PortionUpper * fromPortion);
                newValue = newValue.Clamp(min, max);
                if (upperValue != newValue)
                {
                    upperValue = newValue;
                    if (upperValue < lowerValue)
                    {
                        lowerValue = upperValue;
                    }
                    OnChanged?.Invoke(lowerValue, upperValue);
                }
            }
        }

        public override void OnBarDragEnded(Layout scroller)
        {
            if (scroller == indicatorLower)
            {
                var newValue = (int)Math.Round(PortionLower * fromPortion);
                var delta = lowerValue - newValue;
                ChangeLowerValue(delta);
            }
            else
            {
                var newValue = (int)Math.Round(PortionUpper * fromPortion);
                var delta = upperValue - newValue;
                ChangeUpperValue(delta);
            }
        }

        void ChangeLowerValue(int delta)
        {
            var oldValue = lowerValue;

            lowerValue -= delta;
            lowerValue = lowerValue.Clamp(min, max);
            if (lowerValue > upperValue)
            {
                upperValue = lowerValue;
            }
            var newPortion = (lowerValue - min) * toPortion;
            SetLowerPortion(newPortion);
            if (lowerValue != oldValue)
            {
                OnChanged?.Invoke(lowerValue, upperValue);
            }
        }

        void ChangeUpperValue(int delta)
        {
            var oldValue = upperValue;

            upperValue -= delta;
            upperValue = upperValue.Clamp(min, max);
            if (upperValue < lowerValue)
            {
                lowerValue = upperValue;
            }
            var newPortion = (upperValue - min) * toPortion;
            SetUpperPortion(newPortion);
            if (upperValue != oldValue)
            {
                OnChanged?.Invoke(lowerValue, upperValue);
            }
        }
    }
}
