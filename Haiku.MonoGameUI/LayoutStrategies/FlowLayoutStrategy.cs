using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haiku.MonoGameUI.LayoutStrategies
{
    public class FlowLayoutStrategy : LayoutStrategy
    {
        public Point ParentSize { get; private set; }
        public Point ContentSize { get; private set; }
        readonly Orientation primaryFlowOrientation;
        readonly int primaryFlowLayoutCount;
        readonly int spacing;

        public FlowLayoutStrategy(Orientation primaryFlowOrientation, int primaryFlowLayoutCount, int spacing = 0)
        {
            this.primaryFlowOrientation = primaryFlowOrientation;
            this.primaryFlowLayoutCount = primaryFlowLayoutCount;
            this.spacing = spacing;
        }

        public void LayoutChildren(Point parentSize, List<Layout> children)
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
            if (primaryFlowOrientation == Orientation.Horizontal)
            {
                int x = ColumnX(frame, children);
                int y = RowY(children);

                return new Rectangle(x, y, frame.Width, frame.Height);
            }
            //else
            //{
            //    int y;

            //    if (direction == Direction.Forward)
            //    {
            //        y = BottomY(frame, children);
            //    }
            //    else
            //    {
            //        y = TopY(frame, children);
            //    }
            //    return new Rectangle(frame.X, y, frame.Width, frame.Height);
            //}
            return Rectangle.Empty;
        }

        Point CalculateContentSize(List<Layout> children)
        {
            int width = 0;
            int height = 0;

            foreach (var child in children)
            {
                var frame = child.Frame;
                if (frame.Width > 0)
                {
                    width = Math.Max(frame.Right + spacing / 2, width);
                }
                if (frame.Height > 0)
                {
                    height = Math.Max(frame.Bottom + spacing / 2, height);
                }
            }

            return new Point(width, height);
        }

        int ColumnX(Rectangle frame, List<Layout> children)
        {
            int x = frame.Width > 0 ? spacing / 2 : 0;

            if ((children.Count % primaryFlowLayoutCount) != 0)
            {
                foreach (var child in children.Reverse<Layout>())
                {
                    var lastFrame = child.Frame;
                    if (lastFrame.Width > 0)
                    {
                        x = lastFrame.Right + spacing;
                        break;
                    }
                }
            }

            return x;
        }

        int RowY(List<Layout> children)
        {
            int y = spacing / 2;

            if (children.Count > 0)
            {
                if ((children.Count % primaryFlowLayoutCount) > 0)
                {
                    y = children.Last().Frame.Top;
                }
                else
                {
                    y = children.Last().Frame.Bottom + spacing;
                }
            }

            return y;
        }


    }
}
