using Haiku.MathExtensions;
using Microsoft.Xna.Framework;
using System;

namespace Haiku.MonoGameUI.Layouts
{
    public delegate void OrdinalSliderChangeHandler(int value);

    public class OrdinalSlider : Slider
    {
        public new OrdinalSliderChangeHandler OnChanged;
        public int Value => value;
        readonly int min;
        readonly int max;
        readonly float toPortion;
        readonly float fromPortion;
        int value;

        public OrdinalSlider(
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
            value = min;
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

        public void SetValue(int value)
        {
            var portion = value * toPortion;
            SetValue(portion);
            this.value = value.Clamp(min, max);
        }

        public override void ScrollLine(Layout scroller, int multiple)
        {
            ChangeValue(multiple);
        }

        public override void Scroll(Layout scroller, int delta, bool animated = true)
        {
            base.Scroll(scroller, delta, animated);
            var newValue = (int)Math.Round(Portion * fromPortion);
            newValue = newValue.Clamp(min, max);
            if (value != newValue)
            {
                value = newValue;
                OnChanged?.Invoke(newValue);
            }
        }

        public override void OnBarDragEnded(Layout scroller)
        {
            var newValue = (int)Math.Round(Portion * fromPortion);
            var delta = value - newValue;
            ChangeValue(delta);
        }

        void ChangeValue(int delta)
        {
            var oldValue = value;
            value -= delta;
            value = value.Clamp(min, max);
            var newPortion = (value - min) * toPortion;
            SetPortion(newPortion);
            if (value != oldValue)
            {
                OnChanged?.Invoke(value);
            }
        }
    }
}
