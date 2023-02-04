using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haiku.MonoGameUI.LayoutStrategies
{
    public class LinearLayoutStrategy : LayoutStrategy
    {
        public Point ParentSize { get; protected set; }
        public Point ContentSize { get; private set; }
        readonly Orientation orientation;
        readonly int spacing;
        readonly int startPadding;
        readonly int endPadding;
        readonly Direction direction;

        public LinearLayoutStrategy(Orientation orientation, int spacing = 0, int startPadding = 0, int endPadding = 0, Direction direction = Direction.Forward)
        {
            this.orientation = orientation;
            this.spacing = spacing;
            this.startPadding = startPadding;
            this.endPadding = endPadding;
            this.direction = direction;
        }

        public virtual void LayoutChildren(Point parentSize, List<Layout> children)
        {
            List<Layout> newChildren = new List<Layout>();

            ParentSize = parentSize;
            foreach (var child in children)
            {
                child.Frame = FrameForNewChild(child.Frame, newChildren);
                newChildren.Add(child);
            }
            ContentSize = CalculateContentSize(children);
            ParentSize = ContentSize;
        }

        Rectangle FrameForNewChild(Rectangle frame, List<Layout> children)
        {
            if (orientation == Orientation.Horizontal)
            {
                int x;

                if (direction == Direction.Forward)
                {
                    x = RightX(frame, children);
                }
                else
                {
                    x = LeftX(frame, children);
                }
                return new Rectangle(x, frame.Y, frame.Width, frame.Height);
            }
            else
            {
                int y;

                if (direction == Direction.Forward)
                {
                    y = BottomY(frame, children);
                }
                else
                {
                    y = TopY(frame, children);
                }
                return new Rectangle(frame.X, y, frame.Width, frame.Height);
            }
        }

        Point CalculateContentSize(List<Layout> children)
        {
            int width = 0;
            int height = 0;

            if (orientation == Orientation.Horizontal)
            {
                if (direction == Direction.Forward)
                {
                    foreach (var child in children.Reverse<Layout>())
                    {
                        var lastFrame = child.Frame;
                        if (lastFrame.Width > 0)
                        {
                            width = lastFrame.Right + endPadding;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var child in children)
                    {
                        var firstFrame = child.Frame;
                        if (firstFrame.Width > 0)
                        {
                            width = firstFrame.Right + endPadding;
                            break;
                        }
                    }
                }
                foreach (var child in children)
                {
                    height = Math.Max(height, child.Frame.Bottom);
                }
            }
            else
            {
                if (direction == Direction.Forward)
                {
                    foreach (var child in children.Reverse<Layout>())
                    {
                        var lastFrame = child.Frame;
                        if (lastFrame.Height > 0)
                        {
                            height = lastFrame.Bottom + endPadding;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var child in children)
                    {
                        var firstFrame = child.Frame;
                        if (firstFrame.Height > 0)
                        {
                            height = firstFrame.Bottom + endPadding;
                            break;
                        }
                    }
                }
                foreach (var child in children)
                {
                    width = Math.Max(width, child.Frame.Right);
                }
            }

            return new Point(width, height);
        }

        int RightX(Rectangle frame, List<Layout> children)
        {
            int x = frame.Width > 0 ? startPadding : 0;

            foreach (var child in children.Reverse<Layout>())
            {
                var lastFrame = child.Frame;
                if (lastFrame.Width > 0)
                {
                    x = lastFrame.Right + spacing;
                    break;
                }
            }

            return x;
        }

        int LeftX(Rectangle frame, List<Layout> children)
        {
            int x = frame.Width > 0 ? ParentSize.X - frame.Width - endPadding : ParentSize.X;

            foreach (var child in children.Reverse<Layout>())
            {
                var firstFrame = child.Frame;
                if (firstFrame.Width > 0)
                {
                    x = firstFrame.Left - frame.Width - spacing;
                    break;
                }
            }

            return x;
        }

        int BottomY(Rectangle frame, List<Layout> children)
        {
            int y = frame.Height > 0 ? startPadding : 0;

            foreach (var child in children.Reverse<Layout>())
            {
                var lastFrame = child.Frame;
                if (lastFrame.Height > 0)
                {
                    y = lastFrame.Bottom + spacing;
                    break;
                }
            }

            return y;
        }

        int TopY(Rectangle frame, List<Layout> children)
        {
            int y = frame.Height > 0 ? ParentSize.Y - frame.Height - endPadding : ParentSize.Y;

            foreach (var child in children.Reverse<Layout>())
            {
                var firstFrame = child.Frame;
                if (firstFrame.Height > 0)
                {
                    y = firstFrame.Top - frame.Height - spacing;
                    break;
                }
            }

            return y;
        }
    }
}
