using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using System;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class ScrollBar : Panel
    {
        public static SpriteFrame UpArrowTexture;
        public static SpriteFrame UpArrowTextureActive;
        public static SpriteFrame UpArrowTextureSelected;
        public static SpriteFrame DownArrowTexture;
        public static SpriteFrame DownArrowTextureActive;
        public static SpriteFrame DownArrowTextureSelected;
        public static SpriteFrame LeftArrowTexture;
        public static SpriteFrame LeftArrowTextureActive;
        public static SpriteFrame LeftArrowTextureSelected;
        public static SpriteFrame RightArrowTexture;
        public static SpriteFrame RightArrowTextureActive;
        public static SpriteFrame RightArrowTextureSelected;
        public static SpriteFrame ScrollThumbTexture;
        public static SpriteFrame ScrollThumbTextureActive;
        public static SpriteFrame ScrollThumbTextureSelected;
        public static SpriteFrame TroughTexture;
        public const int BarSize = 16;
        const int scrollButtonMinorSize = 12;
        internal bool IsBeingDragged => thumb.IsDragging || trough.IsDragging;
        internal bool IsFocusable { get => false; set => thumb.IsFocusable = value; }
        readonly Scrollable scrollable;
        readonly ScrollBarButton backArrow;
        readonly ScrollBarButton foreArrow;
        readonly ScrollTrough trough;
        readonly ScrollThumb thumb;

        internal ScrollBar(Rectangle frame, Scrollable scrollable)
            : base(frame, new LinearLayoutStrategy(scrollable.Orientation))
        {
            this.scrollable = scrollable;
            BackgroundColor = Color.Transparent;
            thumb = new ScrollThumb(new Rectangle(0, 0, BarSize, BarSize), scrollable);
            if (scrollable.Orientation == Orientation.Horizontal)
            {
                backArrow = new ScrollBarButton(new Rectangle(0, 0, scrollButtonMinorSize, BarSize), scrollable);
                backArrow.SetBackground(LeftArrowTexture, LeftArrowTextureActive, LeftArrowTextureSelected);
                foreArrow = new ScrollBarButton(new Rectangle(frame.Width - scrollButtonMinorSize, 0, scrollButtonMinorSize, BarSize), scrollable);
                foreArrow.SetBackground(RightArrowTexture, RightArrowTextureActive, RightArrowTextureSelected);
                trough = new ScrollTrough(new Rectangle(scrollButtonMinorSize, 0, frame.Width - scrollButtonMinorSize * 2, BarSize), thumb, scrollable);
            }
            else
            {
                backArrow = new ScrollBarButton(new Rectangle(0, 0, BarSize, scrollButtonMinorSize), scrollable);
                backArrow.SetBackground(UpArrowTexture, UpArrowTextureActive, UpArrowTextureSelected);
                foreArrow = new ScrollBarButton(new Rectangle(0, frame.Height - scrollButtonMinorSize, BarSize, scrollButtonMinorSize), scrollable);
                foreArrow.SetBackground(DownArrowTexture, DownArrowTextureActive, DownArrowTextureSelected);
                trough = new ScrollTrough(new Rectangle(0, scrollButtonMinorSize, BarSize, frame.Height - scrollButtonMinorSize * 2), thumb, scrollable);
            }
            thumb.Trough = trough;
            thumb.SetBackground(ScrollThumbTexture, ScrollThumbTextureActive, ScrollThumbTextureSelected);
            trough.Texture = TroughTexture;
            backArrow.Action = (_) => scrollable.ScrollLine(backArrow, 1);
            foreArrow.Action = (_) => scrollable.ScrollLine(foreArrow , - 1);
            AddChild(backArrow);
            trough.AddChild(thumb);
            AddChild(trough);
            AddChild(foreArrow);
        }

        public override void SetFrameWithSideEffects(Rectangle frame)
        {
            base.SetFrameWithSideEffects(frame);
            if (scrollable.Orientation == Orientation.Horizontal)
            {
                trough.Frame = new Rectangle(scrollButtonMinorSize, 0, frame.Width - scrollButtonMinorSize * 2, BarSize);
            }
            else
            {
                trough.Frame = new Rectangle(0, scrollButtonMinorSize, BarSize, frame.Height - scrollButtonMinorSize * 2);
            }
            DoLayout();
        }

        internal void OnScrolled()
        {
            if (scrollable.Orientation == Orientation.Horizontal)
            {
                double thumbPosition = 0;
                if (scrollable.ContentSize.X > 0)
                {
                    thumbPosition = Math.Abs(scrollable.ContentOffset.X) / (double)scrollable.ContentSize.X;
                }
                thumbPosition = Math.Min(thumbPosition * trough.Frame.Width, trough.Frame.Width - thumb.Frame.Width);
                thumbPosition = Math.Round(thumbPosition);
                thumb.Frame = new Rectangle((int)thumbPosition, thumb.Frame.Y, thumb.Frame.Width, thumb.Frame.Height);
            }
            else
            {
                double thumbPosition = 0;
                if (scrollable.ContentSize.Y > 0)
                {
                    thumbPosition = Math.Abs(scrollable.ContentOffset.Y) / (double)scrollable.ContentSize.Y;
                }
                thumbPosition = Math.Min(thumbPosition * trough.Frame.Height, trough.Frame.Height - thumb.Frame.Height);
                thumbPosition = Math.Round(thumbPosition);
                thumb.Frame = new Rectangle(thumb.Frame.X, (int)thumbPosition, thumb.Frame.Width, thumb.Frame.Height);
            }
        }

        internal void Disable()
        {
            backArrow.IsInteractionEnabled = false;
            backArrow.Alpha = 0.5f;
            foreArrow.IsInteractionEnabled = false;
            foreArrow.Alpha = 0.5f;
            trough.Alpha = 0.5f;
            thumb.IsVisible = false;
        }

        internal void Enable()
        {
            backArrow.IsInteractionEnabled = true;
            backArrow.Alpha = 1;
            foreArrow.IsInteractionEnabled = true;
            foreArrow.Alpha = 1;
            trough.Alpha = 0.75f;
            thumb.IsVisible = true;
            float thumbSize;
            if (scrollable.Orientation == Orientation.Horizontal)
            {
                thumbSize = scrollable.FrameSize.X / (float)scrollable.ContentSize.X;
                thumbSize = Math.Max(BarSize, thumbSize * trough.Frame.Width);
                var thumbX = Math.Min(thumb.Frame.X, trough.Frame.Width - (int)thumbSize);
                thumb.Frame = new Rectangle(new Point(thumbX, thumb.Frame.Y), new Point((int)thumbSize, BarSize));
            }
            else
            {
                thumbSize = scrollable.FrameSize.Y / (float)scrollable.ContentSize.Y;
                thumbSize = Math.Max(BarSize, thumbSize * trough.Frame.Height);
                var thumbY = Math.Min(thumb.Frame.Y, trough.Frame.Height - (int)thumbSize);
                thumb.Frame = new Rectangle(new Point(thumb.Frame.X, thumbY), new Point(BarSize, (int)thumbSize));
            }
        }
    }
}
