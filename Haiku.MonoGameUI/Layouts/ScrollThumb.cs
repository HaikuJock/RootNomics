using Haiku.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Haiku.MonoGameUI.Layouts
{
    public class ScrollThumb : Button
    {
        internal ScrollTrough Trough;
        readonly Scrollable scrollable;
        Point previousDragPoint;

        internal ScrollThumb(Rectangle frame, Scrollable scrollable)
            : base(frame)
        {
            this.scrollable = scrollable;
            IsDraggingEnabled = true;
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
            Trough.SetActiveThumb(this);
            scrollable.OnBarDragEnded(this);
        }

        protected override bool OnScroll(int scrollDistance, Point point, Rectangle container)
        {
            Trough.SetActiveThumb(this);
            scrollable.MouseScroll(this, scrollDistance);
            return true;
        }

        internal override bool HandleKeyPress(Keys key, IEnumerable<Keys> activeModifierKeys, TextClipboarding clipboard)
        {
            if (IsFocusable)
            {
                int end;

                if (Trough.Orientation == Orientation.Horizontal)
                {
                    end = scrollable.ContentSize.X;
                    if (key == Keys.Left)
                    {
                        Trough.SetActiveThumb(this);
                        scrollable.MouseScroll(this, 1);
                        return true;
                    }
                    else if (key == Keys.Right)
                    {
                        Trough.SetActiveThumb(this);
                        scrollable.MouseScroll(this, -1);
                        return true;
                    }
                }
                else
                {
                    end = scrollable.ContentSize.Y;
                    if (key == Keys.Up)
                    {
                        Trough.SetActiveThumb(this);
                        scrollable.MouseScroll(this, -1);
                        return true;
                    }
                    else if (key == Keys.Down)
                    {
                        Trough.SetActiveThumb(this);
                        scrollable.MouseScroll(this, 1);
                        return true;
                    }
                }
                if (key == Keys.Home)
                {
                    Trough.SetActiveThumb(this);
                    scrollable.Scroll(this, end, animated: false);
                    return true;
                }
                else if (key == Keys.End)
                {
                    Trough.SetActiveThumb(this);
                    scrollable.Scroll(this, -end, animated: false);
                    return true;
                }
                return false;
            }
            return base.HandleKeyPress(key, activeModifierKeys, clipboard);
        }

        void Drag(Point point)
        {
            int pixelDelta;
            double troughToContent;

            Trough.SetActiveThumb(this);
            if (scrollable.Orientation == Orientation.Horizontal)
            {
                pixelDelta = previousDragPoint.X - point.X;
                troughToContent = scrollable.ContentSize.X / (double)Trough.Frame.Width;
            }
            else
            {
                pixelDelta = previousDragPoint.Y - point.Y;
                troughToContent = scrollable.ContentSize.Y / (double)Trough.Frame.Height;
            }
            var scrollDelta = Math.Round(pixelDelta * troughToContent);
            scrollable.Scroll(this, (int)scrollDelta, animated: false);
            previousDragPoint = point;
        }
    }
}
