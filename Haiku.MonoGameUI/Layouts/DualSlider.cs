using Haiku.MathExtensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public delegate void DualSliderChangeHandler(float lower, float upper);

    public class DualSlider : Control, Scrollable
    {
        public static SpriteFrame TrackTextureLeft;
        public static SpriteFrame TrackTextureMiddle;
        public Point ContentSize => track.Frame.Size;
        public Point ContentOffset => Point.Zero;
        public Point FrameSize => Point.Zero;
        public Orientation Orientation => Orientation.Horizontal;
        public override bool IsFocusable {
            get => false; 
            set
            {
                indicatorLower.IsFocusable = value;
                indicatorUpper.IsFocusable = value;
            }
        }
        protected readonly ScrollTrough track;
        readonly Panel visualTrackLeft;
        readonly Panel visualTrackMiddle;
        readonly Panel visualTrackRight;
        protected readonly ScrollThumb indicatorLower;
        protected readonly ScrollThumb indicatorUpper;
        public DualSliderChangeHandler OnChanged;
        protected float PortionLower { get; private set; }
        protected float PortionUpper { get; private set; }
        readonly int indicatorInset;

        public DualSlider(Rectangle frame, int trackHeight, Point indicatorSize)
            : base(frame)
        {
            PortionLower = 1f;
            PortionUpper = 1f;
            var visualTrackSize = new Point(frame.Width - indicatorSize.X * 2, trackHeight);
            indicatorInset = indicatorSize.X;
            var visualTrackFrameLeft = new Rectangle(indicatorInset, (Frame.Height - trackHeight) / 2, visualTrackSize.X, visualTrackSize.Y);
            var visualTrackFrameMiddle = new Rectangle(frame.Width - indicatorInset, (Frame.Height - trackHeight) / 2, 0, visualTrackSize.Y);
            var visualTrackFrameRight = new Rectangle(frame.Width - indicatorInset, (Frame.Height - trackHeight) / 2, 0, visualTrackSize.Y);
            var trackFrame = new Rectangle(0, 0, frame.Width, frame.Height);
            var indicatorLowerFrame = new Rectangle(frame.Width - indicatorSize.X * 2, (Frame.Height - indicatorSize.Y) / 2, indicatorSize.X, indicatorSize.Y);
            var indicatorUpperFrame = new Rectangle(frame.Width - indicatorSize.X, (Frame.Height - indicatorSize.Y) / 2, indicatorSize.X, indicatorSize.Y);

            indicatorLower = new ScrollThumb(indicatorLowerFrame, this);
            indicatorLower.SetBackground(Slider.IndicatorTextureNormal, Slider.IndicatorTextureActive, Slider.IndicatorTextureSelected);
            indicatorUpper = new ScrollThumb(indicatorUpperFrame, this);
            indicatorUpper.SetBackground(Slider.IndicatorTextureNormal, Slider.IndicatorTextureActive, Slider.IndicatorTextureSelected);
            track = new ScrollTrough(trackFrame, new List<Layout> { indicatorLower, indicatorUpper }, this)
            {
                BackgroundColor = Color.Transparent,
            };
            indicatorLower.Trough = track;
            indicatorUpper.Trough = track;
            visualTrackLeft = new Panel(visualTrackFrameLeft)
            {
                Texture = TrackTextureLeft,
                IsInteractionEnabled = false,
            };
            visualTrackMiddle = new Panel(visualTrackFrameMiddle)
            {
                Texture = TrackTextureMiddle,
                IsInteractionEnabled = false,
            };
            visualTrackRight = new Panel(visualTrackFrameRight)
            {
                Texture = Slider.TrackTextureRight,
                IsInteractionEnabled = false,
            };
            AddChild(track);
            AddChild(visualTrackLeft);
            AddChild(visualTrackMiddle);
            AddChild(visualTrackRight);
            AddChild(indicatorLower);
            AddChild(indicatorUpper);
            BackgroundColor = Color.Transparent;
        }

        public void SetValues(float portionLower, float portionUpper)
        {
            if (portionLower > portionUpper)
            {
                var swap = portionUpper;
                portionUpper = portionLower;
                portionLower = swap;
            }
            this.PortionLower = portionLower.Clamp(0f, 1f);
            this.PortionUpper = portionUpper.Clamp(0f, 1f);
            UpdateFrames();
        }

        public virtual void ScrollLine(Layout scroller, int multiple)
        {
            if (scroller == indicatorLower)
            {
                ChangeLowerPortion(multiple * 0.1f);
            }
            else
            {
                ChangeUpperPortion(multiple * 0.1f);
            }
        }

        public void ScrollPage(Layout scroller, int multiple)
        {
            ScrollLine(scroller, multiple);
        }

        public virtual void Scroll(Layout scroller, int delta, bool animated = true)
        {
            var portionDelta = delta / (float)(track.Frame.Width - indicatorInset * 2);
            if (scroller == indicatorLower)
            {
                ChangeLowerPortion(portionDelta);
            }
            else
            {
                ChangeUpperPortion(portionDelta);
            }
        }

        public void MouseScroll(Layout scroller, int scrollDistance)
        {
            ScrollLine(scroller, Math.Sign(scrollDistance));
        }

        public virtual void OnBarDragEnded(Layout scroller)
        {

        }

        void ChangeLowerPortion(float delta)
        {
            var newPortion = PortionLower - delta;
            newPortion = newPortion.Clamp(0f, 1f);
            SetLowerPortion(newPortion);
        }

        protected void SetLowerPortion(float newPortion)
        {
            PortionLower = newPortion;
            if (PortionLower > PortionUpper)
            {
                PortionUpper = PortionLower;
            }
            UpdateFrames();
            OnChanged?.Invoke(PortionLower, PortionUpper);
        }

        void ChangeUpperPortion(float delta)
        {
            var newPortion = PortionUpper - delta;
            newPortion = newPortion.Clamp(0f, 1f);
            SetUpperPortion(newPortion);
        }

        protected void SetUpperPortion(float newPortion)
        {
            PortionUpper = newPortion;
            if (PortionUpper < PortionLower)
            {
                PortionLower = PortionUpper;
            }
            UpdateFrames();
            OnChanged?.Invoke(PortionLower, PortionUpper);
        }

        void UpdateFrames()
        {
            var width = track.Frame.Width;
            var visualWidth = width - indicatorInset * 2;
            var lowerLeft = (int)Math.Round(visualWidth * PortionLower);
            var upperLeft = (int)Math.Round(visualWidth * PortionUpper);
            var rightWidth = (int)Math.Round(visualWidth * (1 - PortionUpper));

            indicatorLower.Frame = new Rectangle(lowerLeft, indicatorLower.Frame.Y, indicatorLower.Frame.Width, indicatorLower.Frame.Height);
            indicatorUpper.Frame = new Rectangle(upperLeft + indicatorInset, indicatorUpper.Frame.Y, indicatorUpper.Frame.Width, indicatorUpper.Frame.Height);
            visualTrackLeft.Frame = new Rectangle(indicatorInset, visualTrackLeft.Frame.Y, lowerLeft, visualTrackLeft.Frame.Height);
            visualTrackRight.Frame = new Rectangle(visualTrackRight.Frame.Right - rightWidth, visualTrackRight.Frame.Y, rightWidth, visualTrackRight.Frame.Height);
            visualTrackMiddle.Frame = new Rectangle(visualTrackLeft.Frame.Right, visualTrackMiddle.Frame.Y, visualTrackRight.Frame.Left - visualTrackLeft.Frame.Right, visualTrackMiddle.Frame.Height);
            visualTrackLeft.IsVisible = visualTrackLeft.Frame.Width > indicatorInset;
            visualTrackRight.IsVisible = visualTrackRight.Frame.Width > indicatorInset;
        }
    }
}
