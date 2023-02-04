using Haiku.MathExtensions;
using Microsoft.Xna.Framework;
using System;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public delegate void SliderChangeHandler(float portion);

    public class Slider : Control, Scrollable
    {
        public static SpriteFrame TrackTextureLeft;
        public static SpriteFrame TrackTextureRight;
        public static SpriteFrame IndicatorTextureNormal;
        public static SpriteFrame IndicatorTextureActive;
        public static SpriteFrame IndicatorTextureSelected;
        public Point ContentSize => track.Frame.Size;
        public Point ContentOffset => Point.Zero;
        public Point FrameSize => Point.Zero;
        public Orientation Orientation => Orientation.Horizontal;
        public override bool IsFocusable { get => false; set => indicator.IsFocusable = value; }
        protected readonly ScrollTrough track;
        readonly Panel visualTrackLeft;
        readonly Panel visualTrackRight;
        readonly ScrollThumb indicator;
        readonly int indicatorInset;
        public SliderChangeHandler OnChanged;
        protected float Portion { get; private set; }

        public Slider(Rectangle frame, int trackHeight, Point indicatorSize)
            : base(frame)
        {
            Portion = 1f;
            var visualTrackSize = new Point(frame.Width - indicatorSize.X, trackHeight);
            indicatorInset = indicatorSize.X / 2;
            var visualTrackFrameLeft = new Rectangle(indicatorInset, (Frame.Height - trackHeight) / 2, visualTrackSize.X, visualTrackSize.Y);
            var visualTrackFrameRight = new Rectangle(frame.Width - indicatorInset, (Frame.Height - trackHeight) / 2, 0, visualTrackSize.Y);
            var trackFrame = new Rectangle(0, 0, frame.Width, frame.Height);
            var indicatorFrame = new Rectangle(frame.Width - indicatorSize.X, (Frame.Height - indicatorSize.Y) / 2, indicatorSize.X, indicatorSize.Y);

            indicator = new ScrollThumb(indicatorFrame, this);
            indicator.SetBackground(IndicatorTextureNormal, IndicatorTextureActive, IndicatorTextureSelected);
            track = new ScrollTrough(trackFrame, indicator, this)
            {
                BackgroundColor = Color.Transparent,
            };
            indicator.Trough = track;
            visualTrackLeft = new Panel(visualTrackFrameLeft)
            {
                Texture = TrackTextureLeft,
                IsInteractionEnabled = false,
            };
            visualTrackRight = new Panel(visualTrackFrameRight)
            {
                Texture = TrackTextureRight,
                IsInteractionEnabled = false,
            };
            AddChild(track);
            AddChild(visualTrackLeft);
            AddChild(visualTrackRight);
            AddChild(indicator);
            BackgroundColor = Color.Transparent;
        }

        public void SetValue(float portion)
        {
            this.Portion = portion.Clamp(0f, 1f);
            UpdateFrames();
        }

        public virtual void ScrollLine(Layout scroller, int multiple)
        {
            ChangePortion(multiple * 0.1f);
        }

        public void ScrollPage(Layout scroller, int multiple)
        {
            ScrollLine(scroller, multiple);
        }

        public virtual void Scroll(Layout scroller, int delta, bool animated = true)
        {
            var portionDelta = delta / (float)(track.Frame.Width - indicatorInset * 2);
            ChangePortion(portionDelta);
        }

        public void MouseScroll(Layout scroller, int scrollDistance)
        {
            ScrollLine(scroller, Math.Sign(scrollDistance));
        }

        public virtual void OnBarDragEnded(Layout scroller)
        {

        }

        void ChangePortion(float delta)
        {
            var newPortion = Portion - delta;
            newPortion = newPortion.Clamp(0f, 1f);
            SetPortion(newPortion);
        }

        protected void SetPortion(float newPortion)
        {
            Portion = newPortion;
            UpdateFrames();
            OnChanged?.Invoke(Portion);
        }

        protected void UpdateFrames()
        {
            var width = track.Frame.Width;
            var visualWidth = width - indicatorInset * 2;
            var leftWidth = (int)Math.Round(visualWidth * Portion);
            
            indicator.Frame = new Rectangle(leftWidth, indicator.Frame.Y, indicator.Frame.Width, indicator.Frame.Height);
            visualTrackLeft.Frame = new Rectangle(indicatorInset, visualTrackLeft.Frame.Y, leftWidth, visualTrackLeft.Frame.Height);
            visualTrackRight.Frame = new Rectangle(visualTrackLeft.Frame.Right, visualTrackRight.Frame.Y, visualTrackRight.Frame.Right - visualTrackLeft.Frame.Right, visualTrackRight.Frame.Height);
            visualTrackLeft.IsVisible = visualTrackLeft.Frame.Width > indicatorInset;
            visualTrackRight.IsVisible = visualTrackRight.Frame.Width > indicatorInset;
        }
    }
}
