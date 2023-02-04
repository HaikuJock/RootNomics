using Haiku.MathExtensions;
using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haiku.MonoGameUI.Layouts
{
    public interface Scrollable
    {
        Point ContentSize { get; }
        Point ContentOffset { get; }
        Point FrameSize { get; }
        Orientation Orientation { get; }

        void ScrollLine(Layout scroller, int multiple);
        void ScrollPage(Layout scroller, int multiple);
        void Scroll(Layout scroller, int delta, bool animated = true);
        void MouseScroll(Layout scroller, int scrollDistance);
        void OnBarDragEnded(Layout scroller);
    }

    public interface ScrollListening
    {
        void OnUserScrolled();
        void OnScrolled();
        void OnDragScrollEnded();
    }

    public class ScrollContent : Control, Scrollable
    {
        const double DefaultAnimationSpeed = 150;
        const double AnimationAcceleration = 75;

        public Point ContentSize { get; private set; }
        public Point ContentOffset { get; private set; }
        public Point FrameSize { get; private set; }
        public Orientation Orientation { get; }
        public int MinimumScrollDistance;
        public int MouseScrollMultiple;
        public ScrollListening ScrollListener;
        public bool IsBeingDragged => IsDragging || ScrollBar?.IsBeingDragged == true;
        internal bool IsScrollingEnabled { get; private set; }
        internal ScrollBar ScrollBar;
        readonly int spacing;
        readonly bool wrap;
        Point previousDragPoint;
        Animation scrollAnimation;
        Point targetContentOffset;
        double animationSpeed = DefaultAnimationSpeed;

        internal ScrollContent(Rectangle frame, Orientation orientation, int spacing, int startPadding, int endPadding, bool wrap)
            : base(frame, new ScrollingLayoutStrategy(orientation, spacing, startPadding, endPadding))
        {
            this.spacing = spacing;
            this.wrap = wrap;
            IsClippingChildren = true;
            ContentOffset = Point.Zero;
            ContentSize = Point.Zero;
            FrameSize = frame.Size;
            Orientation = orientation;
            MinimumScrollDistance = int.MaxValue - spacing - 1;
            MouseScrollMultiple = 3;
            IsDraggingEnabled = true;
        }

        public override void SetFrameWithSideEffects(Rectangle newFrame)
        {
            base.SetFrameWithSideEffects(newFrame);
            FrameSize = newFrame.Size;
            var content = new List<Layout>(Children);
            ReplaceContent(content);
        }

        public void MouseScroll(Layout scroller, int scrollDistance)
        {
            if (scrollDistance > 0)
            {
                ScrollLine(scroller, - MouseScrollMultiple);
            }
            else if (scrollDistance < 0)
            {
                ScrollLine(scroller, MouseScrollMultiple);
            }
        }

        public void OnBarDragEnded(Layout scroller)
        {
            ScrollListener?.OnDragScrollEnded();
        }

        public void ScrollLine(Layout scroller, int multiple)
        {
            Scroll(scroller, multiple * MinimumScrollDistance);
        }

        public void ScrollToChild(Layout child, bool animated)
        {
            if (Orientation == Orientation.Horizontal)
            {
                if (child.Frame.Right > Frame.Width)
                {
                    var distance = Frame.Width - child.Frame.Right;

                    Scroll(this, distance - MinimumScrollDistance * 2, animated);
                }
                else if (child.Frame.Left < 0)
                {
                    var distance = -child.Frame.Left;

                    Scroll(this, distance + MinimumScrollDistance * 2, animated);
                }
            }
            else
            {
                if (child.Frame.Top > Frame.Height)
                {
                    var distance = Frame.Height - child.Frame.Bottom;

                    Scroll(this, distance - MinimumScrollDistance * 2, animated);
                }
                else if (child.Frame.Top < 0)
                {
                    var distance = -child.Frame.Top;

                    Scroll(this, distance + MinimumScrollDistance * 2, animated);
                }
            }
        }

        public void ScrollPage(Layout scroller, int multiple)
        {
            if (Orientation == Orientation.Horizontal)
            {
                Scroll(scroller, multiple * FrameSize.X, animated: false);
            }
            else
            {
                Scroll(scroller, multiple * FrameSize.Y, animated: false);
            }
        }

        public void Scroll(Layout scroller, int delta, bool animated = true)
        {
            if (!IsScrollingEnabled)
            {
                return;
            }
            if (Orientation == Orientation.Horizontal)
            {
                if (!wrap)
                {
                    delta = DeltaWithinContentX(delta);
                }
                if (animated)
                {
                    OffsetContentAnimated(new Point(delta, 0));
                }
                else
                {
                    OffsetContent(new Point(delta, 0));
                }
            }
            else
            {
                if (!wrap)
                {
                    delta = DeltaWithinContentY(delta);
                }
                if (animated)
                {
                    OffsetContentAnimated(new Point(0, delta));
                }
                else
                {
                    OffsetContent(new Point(0, delta));
                }
            }
            ScrollListener?.OnUserScrolled();
        }

        public void ScrollToEnd()
        {
            if (Orientation == Orientation.Horizontal)
            {
                if (ContentSize.X > Frame.Width)
                {
                    SetContentOffset(new Point(-(ContentSize.X - Frame.Width), 0));
                }
            }
            else
            {
                if (ContentSize.Y > Frame.Height)
                {
                    SetContentOffset(new Point(0, -(ContentSize.Y - Frame.Height)));
                }
            }
        }

        public void ScrollToStart()
        {
            SetContentOffset(Point.Zero);
        }

        public bool IsOffsetAtEnd()
        {
            if (Orientation == Orientation.Horizontal)
            {
                if (ContentSize.X > Frame.Width)
                {
                    return ContentOffset.X <= -(ContentSize.X - Frame.Width);
                }
            }
            else
            {
                if (ContentSize.Y > Frame.Height)
                {
                    return ContentOffset.Y <= -(ContentSize.Y - Frame.Height);
                }
            }
            return true;
        }

        internal void MoveChild(int indexFrom, int indexTo)
        {
            if (indexFrom == indexTo)
            {
                return;
            }
            var item = Children[indexFrom];

            Children.RemoveAt(indexFrom);
            Children.Insert(indexTo, item);
            DoLayout();
            OnContentChanged(item.Frame.Width, item.Frame.Height);
        }

        public override void AddChild(Layout child)
        {
            base.AddChild(child);
            OnContentChanged(child.Frame.Width, child.Frame.Height);
        }

        protected override bool RemoveChild(Layout child)
        {
            if (base.RemoveChild(child))
            {
                int minNewChildWidth = Children.Any() ? Children.Select(c => c.Frame.Width).Min() : 0;
                int minNewChildHeight = Children.Any() ? Children.Select(c => c.Frame.Height).Min() : 0;

                MinimumScrollDistance = int.MaxValue - spacing - 1;
                OnContentChanged(minNewChildWidth, minNewChildHeight);
                return true;
            }
            return false;
        }

        public override void AddChildren(IEnumerable<Layout> children)
        {
            int minNewChildWidth = children.Any() ? children.Select(c => c.Frame.Width).Min() : 0;
            int minNewChildHeight = children.Any() ? children.Select(c => c.Frame.Height).Min() : 0;

            base.AddChildren(children);
            OnContentChanged(minNewChildWidth, minNewChildHeight);
        }

        public override void OnDragStart(Point point, Rectangle container)
        {
            previousDragPoint = point;
        }

        public override bool OnCursorDrag(Point point, Rectangle container)
        {
            Drag(point);
            return true;
        }

        public override bool OnCursorDragOutside(Point point)
        {
            Drag(point);
            return true;
        }

        public override void OnDragEnd(Point point, Rectangle container)
        {
            ScrollListener?.OnDragScrollEnded();
        }

        public void OffsetContent(Point delta)
        {
            ParentWindow?.CancelAnimation(scrollAnimation);
            scrollAnimation = null;
            SetContentOffset(new Point(ContentOffset.X + delta.X, ContentOffset.Y + delta.Y));
        }

        internal void UpdateScrollingEnabled()
        {
            if (Orientation == Orientation.Horizontal)
            {
                if (FrameSize.X >= ContentSize.X)
                {
                    DisableScrolling();
                }
                else
                {
                    EnableScrolling();
                }
            }
            else
            {
                if (FrameSize.Y >= ContentSize.Y)
                {
                    DisableScrolling();
                }
                else
                {
                    EnableScrolling();
                }
            }
        }

        internal void ReplaceContent(IEnumerable<Layout> layouts)
        {
            float xOffsetPortion = ContentSize.X > 0 ? ContentOffset.X / (float)ContentSize.X : 0f;
            float yOffsetPortion = ContentSize.Y > 0 ? ContentOffset.Y / (float)ContentSize.Y : 0f;

            while (Children.Count > 0)
            {
                Children[0].Parent = null;
                Children.RemoveAt(0);
            }
            SetContentOffset(Point.Zero);
            MinimumScrollDistance = int.MaxValue - spacing - 1;
            if (layouts.Any())
            {
                AddChildren(layouts);
            }
            else
            {
                DoLayout();
                OnContentChanged(0, 0);
                SetContentOffset(Point.Zero);
            }
            if (ContentSize.X > 0
                && xOffsetPortion != 0f
                && FrameSize.X < ContentSize.X)
            {
                SetContentOffset(new Point((int)(xOffsetPortion * ContentSize.X), 0));
            }
            else if (ContentSize.Y > 0
                     && yOffsetPortion != 0f
                     && FrameSize.Y < ContentSize.Y)
            {
                SetContentOffset(new Point(0, (int)(yOffsetPortion * ContentSize.Y)));
            }
        }

        protected override bool OnScroll(int scrollDistance, Point point, Rectangle container)
        {
            MouseScroll(this, scrollDistance);
            return true;
        }

        void OnContentChanged(int minNewChildWidth, int minNewChildHeight)
        {
            if (Orientation == Orientation.Horizontal)
            {
                MinimumScrollDistance = Math.Min(minNewChildWidth + spacing, MinimumScrollDistance + spacing);
            }
            else
            {
                MinimumScrollDistance = Math.Min(minNewChildHeight + spacing, MinimumScrollDistance + spacing);
            }
            ContentSize = LayoutStrategy.ContentSize;
            UpdateScrollingEnabled();
            foreach (var child in Children)
            {
                child.SetFrameWithSideEffects(new Rectangle(child.Frame.Location + ContentOffset, child.Frame.Size));
            }
        }

        void DisableScrolling()
        {
            IsScrollingEnabled = false;
            ScrollBar?.Disable();
            ContentOffset = Point.Zero;
        }

        void EnableScrolling()
        {
            IsScrollingEnabled = true;
            ScrollBar?.Enable();
        }

        void Drag(Point point)
        {
            int pixelDelta;

            if (Orientation == Orientation.Horizontal)
            {
                pixelDelta = point.X - previousDragPoint.X;
            }
            else
            {
                pixelDelta = point.Y - previousDragPoint.Y;
            }
            Scroll(this, pixelDelta, animated: false);
            previousDragPoint = point;
        }

        int DeltaWithinContentY(int delta)
        {
            if (delta < 0)
            {
                delta = Math.Max(delta, -(ContentOffset.Y + (ContentSize.Y - Frame.Height)));
            }
            else
            {
                delta = Math.Min(delta, -ContentOffset.Y);
            }
            return delta;
        }

        int DeltaWithinContentX(int delta)
        {
            if (delta < 0)
            {
                delta = Math.Max(delta, -(ContentOffset.X + (ContentSize.X - Frame.Width)));
            }
            else
            {
                delta = Math.Min(delta, -ContentOffset.X);
            }
            return delta;
        }

        void OffsetContentAnimated(Point delta)
        {
            if (scrollAnimation != null
                && !scrollAnimation.IsComplete)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    var diffX = targetContentOffset.X - ContentOffset.X;

                    if (diffX == 0)
                    {
                        animationSpeed = DefaultAnimationSpeed;
                    }
                    else if (Math.Sign(delta.X) == Math.Sign(diffX))
                    {
                        animationSpeed += AnimationAcceleration;
                        if (wrap)
                        {
                            delta = new Point(delta.X + diffX, delta.Y);
                        }
                        else
                        {
                            delta = new Point(DeltaWithinContentX(delta.X + diffX), delta.Y);
                        }
                    }
                    else
                    {
                        animationSpeed = DefaultAnimationSpeed / 2;
                    }
                }
                else
                {
                    var diffY = targetContentOffset.Y - ContentOffset.Y;

                    if (diffY == 0)
                    {
                        animationSpeed = DefaultAnimationSpeed;
                    }
                    else if (Math.Sign(delta.Y) == Math.Sign(diffY))
                    {
                        animationSpeed += AnimationAcceleration;
                        if (wrap)
                        {
                            delta = new Point(delta.X, delta.Y + diffY);
                        }
                        else
                        {
                            delta = new Point(delta.X, DeltaWithinContentY(delta.Y + diffY));
                        }
                    }
                    else
                    {
                        animationSpeed = DefaultAnimationSpeed / 2;
                    }
                }
                ParentWindow?.CancelAnimation(scrollAnimation);
            }
            else
            {
                var start = ContentOffset;
                var end = new Point(ContentOffset.X + delta.X, ContentOffset.Y + delta.Y);
                var vector = end - start;
                var length = Math.Max(Math.Abs(vector.X), Math.Abs(vector.Y));
                var factor = DefaultAnimationSpeed / (MinimumScrollDistance * 2);
                var speed = length * factor;

                animationSpeed = Math.Max(speed, DefaultAnimationSpeed);
            }
            var startContentOffset = ContentOffset;
            targetContentOffset = new Point(ContentOffset.X + delta.X, ContentOffset.Y + delta.Y);
            var distance = targetContentOffset - startContentOffset;
            double duration = Math.Max(Math.Abs(distance.X), Math.Abs(distance.Y)) / animationSpeed;

            if (duration <= 0)
            {
                SetContentOffset(targetContentOffset);
            }
            else
            {
                scrollAnimation = Animate()
                    .Over(duration)
                    .Curving(RCurve.QuadEaseOut)
                    .Function(
                        (portion, _) =>
                        {
                            double animatedX = Math.Round(startContentOffset.X + distance.X * portion);
                            double animatedY = Math.Round(startContentOffset.Y + distance.Y * portion);

                            SetContentOffset(new Point((int)animatedX, (int)animatedY));
                        },
                        completion: (_, __) => SetContentOffset(targetContentOffset)
                    )
                    .Build();
                ParentWindow?.Add(scrollAnimation);
            }
        }

        void SetContentOffset(Point offset)
        {
            var delta = offset - ContentOffset;
            ContentOffset = offset;
            foreach (var child in Children)
            {
                child.SetFrameWithSideEffects(new Rectangle(child.Frame.Location + delta, child.Frame.Size));
            }
            if (wrap)
            {
                foreach (var child in Children)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        if (child.Frame.Right <= 0)
                        {
                            var x = ContentSize.X + child.Frame.Left;
                            child.SetFrameWithSideEffects(new Rectangle(x, child.Frame.Y, child.Frame.Width, child.Frame.Height));
                        }
                        else if (child.Frame.Right > ContentSize.X)
                        {
                            var x = -(ContentSize.X - child.Frame.Left);
                            child.SetFrameWithSideEffects(new Rectangle(x, child.Frame.Y, child.Frame.Width, child.Frame.Height));
                        }
                    }
                    else
                    {
                        if (child.Frame.Bottom <= 0)
                        {
                            var y = ContentSize.Y + child.Frame.Top;
                            child.SetFrameWithSideEffects(new Rectangle(child.Frame.X, y, child.Frame.Width, child.Frame.Height));
                        }
                        else if (child.Frame.Bottom > ContentSize.Y)
                        {
                            var y = -(ContentSize.Y - child.Frame.Top);
                            child.SetFrameWithSideEffects(new Rectangle(child.Frame.X, y, child.Frame.Width, child.Frame.Height));
                        }
                    }
                }
                ContentOffset = new Point(
                    ContentOffset.X % ContentSize.X,
                    ContentOffset.Y % ContentSize.Y
                );
            }
            ScrollBar?.OnScrolled();
            ScrollListener?.OnScrolled();
        }
    }
}
