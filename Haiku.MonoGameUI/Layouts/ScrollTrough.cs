using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Haiku.MonoGameUI.Layouts
{
    public class ScrollTrough : Control
    {
        internal Orientation Orientation => scrollable.Orientation;
        readonly Scrollable scrollable;
        readonly List<Layout> thumbs;
        Layout activeThumb;

        internal ScrollTrough(Rectangle frame, Layout thumb, Scrollable scrollable)
            : this(frame, new List<Layout> { thumb }, scrollable)
        {
        }

        internal ScrollTrough(Rectangle frame, List<Layout> thumbs, Scrollable scrollable)
            : base(frame)
        {
            this.thumbs = thumbs;
            this.scrollable = scrollable;
            IsDraggingEnabled = true;
            activeThumb = thumbs.First();
        }

        internal void SetActiveThumb(Layout thumb)
        {
            activeThumb = thumb;
        }

        protected override bool OnPress(Point point, Rectangle container)
        {
            var worldFrame = WorldFrame(container);

            ScrollPage(point, worldFrame);
            if (IsOnThumb(point, worldFrame))
            {
                DoOnRelease(point, container);
                DoOnCursorExited(point, container);
                var thumbContainer = activeThumb.Parent.CalculateWorldFrame();
                activeThumb.DoOnCursorEntered(point, thumbContainer);
                activeThumb.DoOnPress(point, thumbContainer);
                ParentWindow?.SetLayoutUnderCursor(activeThumb, point);
            }
            return true;
        }

        public override bool OnCursorDrag(Point point, Rectangle container)
        {
            var worldFrame = WorldFrame(container);

            ScrollPage(point, worldFrame);
            if (IsOnThumb(point, worldFrame))
            {
                EndDragging(point, container);
                var thumbContainer = activeThumb.Parent.CalculateWorldFrame();
                activeThumb.StartDragging(point, thumbContainer);
            }
            return true;
        }

        public override bool OnCursorDragOutside(Point point)
        {
            ScrollPage(point, CalculateWorldFrame());
            return true;
        }

        public override void OnDragEnd(Point point, Rectangle container)
        {
            scrollable.OnBarDragEnded(activeThumb);
        }

        protected override bool OnCursorMove(Point point, Rectangle container) => IsVisible;

        protected override bool OnRelease(Point point, Rectangle container) => IsVisible;

        protected override bool OnAltPress(Point point, Rectangle container) => IsVisible;

        protected override bool OnAltRelease(Point point, Rectangle container) => IsVisible;

        protected override bool OnScroll(int scrollDistance, Point point, Rectangle container)
        {
            scrollable.MouseScroll(activeThumb, scrollDistance);
            return true;
        }

        bool IsOnThumb(Point point, Rectangle worldFrame)
        {
            var localPoint = point - worldFrame.Location;

            foreach (var thumb in thumbs)
            {
                if (thumb.Frame.Contains(localPoint))
                {
                    activeThumb = thumb;
                    return true;
                }
            }

            return false;
        }

        void ScrollPage(Point point, Rectangle worldFrame)
        {
            point -= worldFrame.Location;
            if (scrollable.Orientation == Orientation.Horizontal)
            {
                if (point.X > activeThumb.Frame.Right)
                {
                    scrollable.ScrollPage(activeThumb, - 1);
                }
                else if (point.X < activeThumb.Frame.Left)
                {
                    scrollable.ScrollPage(activeThumb, 1);
                }
            }
            else
            {
                if (point.Y > activeThumb.Frame.Bottom)
                {
                    scrollable.ScrollPage(activeThumb, - 1);
                }
                else if (point.Y < activeThumb.Frame.Top)
                {
                    scrollable.ScrollPage(activeThumb, 1);
                }
            }
        }
    }
}
