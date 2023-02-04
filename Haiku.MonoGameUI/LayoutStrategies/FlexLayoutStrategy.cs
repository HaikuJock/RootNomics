using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haiku.MonoGameUI.LayoutStrategies
{
    public struct Flex
    {
        public ContentJustification ContentJustification;
        public ItemAlignment ItemAlignment;
        public FlexDirection FlexDirection;
    }

    public class FlexLayoutStrategy : LayoutStrategy
    {
        public Point ParentSize { get; private set; }
        public Point ContentSize { get; private set; }
        readonly Flex flex;

        public FlexLayoutStrategy()
            : this(new Flex())
        {
        }

        public FlexLayoutStrategy(Flex flex)
        {
            this.flex = flex;
        }

        class Transformer
        {
            internal readonly Func<Point, List<Layout>, Point> Initiator;
            internal readonly Func<Point, Layout, Point> Transform;

            public Transformer((Func<Point, List<Layout>, Point>, Func<Point, Layout, Point>) init)
            {
                Initiator = init.Item1;
                Transform = init.Item2;
            }

            public Transformer(Func<Point, List<Layout>, Point> initiator, Func<Point, Layout, Point> transform)
            {
                Initiator = initiator;
                Transform = transform;
            }
        }

        static readonly Dictionary<
            ContentJustification,
            Dictionary<
                ItemAlignment,
                Dictionary<FlexDirection, Transformer>>>
            mapOfLayoutsToInitializersAndTransforms = new Dictionary<ContentJustification, Dictionary<ItemAlignment, Dictionary<FlexDirection, Transformer>>>
            {
                { ContentJustification.Start, new Dictionary<ItemAlignment, Dictionary<FlexDirection, Transformer>> {
                        { ItemAlignment.Start, new Dictionary<FlexDirection, Transformer> {
                            { FlexDirection.Row, new Transformer(
                              (_,__) => Point.Zero,
                              (Point point, Layout child) => {
                               child.Frame = new Rectangle(new Point(point.X, child.Frame.Y), child.Size);
                                  return new Point(child.Frame.Right, child.Frame.Top);
                              }
                              ) }
                            }
                        }
                    }
                },
                { ContentJustification.End, new Dictionary<ItemAlignment, Dictionary<FlexDirection, Transformer>> {
                        { ItemAlignment.Start, new Dictionary<FlexDirection, Transformer> {
                            { FlexDirection.Row, new Transformer(
                                (parentSize, children) => new Point(parentSize.X - children.Sum(child => child.Size.X), 0),
                                (Point left, Layout child) => {
                                    child.Frame = new Rectangle(new Point(left.X, child.Frame.Y), child.Size);
                                    return new Point(child.Frame.Right, child.Frame.Top); }
                              ) }
                            }
                        }
                    }
                },
                { ContentJustification.Center, new Dictionary<ItemAlignment, Dictionary<FlexDirection, Transformer>> {
                        { ItemAlignment.Start, new Dictionary<FlexDirection, Transformer> {
                            { FlexDirection.Row, new Transformer( (Point
                                parentSize, List<Layout>
                                children) =>
                            new Point(parentSize.X / 2 - children.Sum(child => child.Size.X) / 2, 0),
                                (Point left, Layout child) => {
                                    child.Frame = new Rectangle(new Point(left.X, child.Frame.Y), child.Size);
                                    return new Point(child.Frame.Right, child.Frame.Top); }
                              ) }
                            }
                        }
                    }
                },
                { ContentJustification.SpaceBetween, new Dictionary<ItemAlignment, Dictionary<FlexDirection, Transformer>> {
                        { ItemAlignment.Start, new Dictionary<FlexDirection, Transformer> {
                            { FlexDirection.Row, new Transformer( (Point
                                parentSize, List<Layout>
                                children) =>
                            {
                                if (children.Count == 1)
                                {
                                    return new Point(0, 0);
                                }
                                return new Point(0, (parentSize.X - children.Sum(child => child.Size.X)) / (children.Count - 1));
                            },
                                (Point leftAndBetween, Layout child) => {
                                    child.Frame = new Rectangle(leftAndBetween.X, child.Frame.Y, child.Size.X, child.Size.Y);
                                    return new Point(child.Frame.Right + leftAndBetween.Y, leftAndBetween.Y); }
                                ) },
                            { FlexDirection.RowReverse, new Transformer( (Point
                                parentSize, List<Layout>
                                children) => new Point(parentSize.X, 0),
                                (Point right, Layout child) => {
                                    child.Frame = new Rectangle(new Point(right.X - child.Size.X, child.Frame.Y), child.Size);
                                    return new Point(child.Frame.Left, 0); }
                                ) }
                            }
                        }
                    }
                },
                { ContentJustification.SpaceAround, new Dictionary<ItemAlignment, Dictionary<FlexDirection, Transformer>> {
                        { ItemAlignment.Start, new Dictionary<FlexDirection, Transformer> {
                            { FlexDirection.Row, new Transformer( (Point
                                parentSize, List<Layout>
                                children) =>
                                {
                                    var contentSize = children.Sum(child => child.Size.X);
                                    var space = parentSize.X - contentSize;
                                    var around = space / children.Count;
                                    return new Point(around / 2, around);
                                },
                                (Point leftAndAround, Layout child) => {
                                    child.Frame = new Rectangle(leftAndAround.X, child.Frame.Y, child.Size.X, child.Size.Y);
                                    return new Point(child.Frame.Right + leftAndAround.Y, leftAndAround.Y); }
                                ) }
                        }
                    }
                } },
            };


        public void LayoutChildren(Point parentSize, List<Layout> children)
        {
            ParentSize = parentSize;
            children.ForEach(child => child.Frame = new Rectangle(Point.Zero, child.Size));
            if (children.Count > 0)
            {
                Transformer transformer = mapOfLayoutsToInitializersAndTransforms[flex.ContentJustification][flex.ItemAlignment][flex.FlexDirection];

                children.Aggregate(
                    transformer.Initiator(parentSize, children),
                    transformer.Transform
                    );
            }
            // assumption all layouts can be achieved with a single aggregation over children
            // The initial Point provided is the position of the first child
            // The transform takes the position and adjusts it according to its properties, sets the frame of the next child
            // and returns the next position for the next child.
            // Provide a function 
            //Func<>

            //children.ForEach(child => child.Frame = new Rectangle(Point.Zero, child.Size));
            //ApplyContentJustification(parentSize, children);
            //switch (flex.ItemAlignment)
            //{
            //    case ItemAlignment.Start:
            //        if (flex.FlexDirection == FlexDirection.Column)
            //        {
            //            children.Aggregate(
            //                0,
            //                (top, child) => (child.Frame = new Rectangle(child.Frame.X, top, child.Size.X, child.Size.Y)).Bottom
            //                );
            //        }
            //        else if (flex.FlexDirection == FlexDirection.ColumnReverse)
            //        {
            //            children.Aggregate(
            //                parentSize.Y,
            //                (bottom, child) => (child.Frame = new Rectangle(child.Frame.X, bottom - child.Size.Y, child.Size.X, child.Size.Y)).Top
            //                );
            //        }
            //        break;
            //    case ItemAlignment.End:
            //        children.Aggregate(
            //            0,
            //            (_, child) => (child.Frame = new Rectangle(child.Frame.X, parentSize.Y - child.Size.Y, child.Size.X, child.Size.Y)).Top
            //            );
            //        break;
            //    case ItemAlignment.Center:
            //        children.Aggregate(
            //            parentSize.Y / 2,
            //            (center, child) => (child.Frame = new Rectangle(child.Frame.X, center - child.Size.Y / 2, child.Size.X, child.Size.Y)).Top
            //            );
            //        break;
            //    case ItemAlignment.Baseline:
            //        throw new NotImplementedException("Requires a baseline from SpriteFont which looks fraught with danger. See commit 02d41d1");
            //    case ItemAlignment.Stretch:
            //        children.Aggregate(
            //            0,
            //            (_, child) => (child.Frame = new Rectangle(child.Frame.X, 0, child.Size.X, parentSize.Y)).Top
            //            );
            //        break;
            //    default:
            //        break;
            //}
        }

        void ApplyContentJustification(Point parentSize, List<Layout> children)
        {
            switch (flex.ContentJustification)
            {
                case ContentJustification.Start:
                    if (flex.FlexDirection == FlexDirection.Row)
                    {
                        children.Aggregate(
                            0,
                            (left, child) => (child.Frame = new Rectangle(left, child.Frame.Y, child.Size.X, child.Size.Y)).Right
                            );
                    }
                    else if (flex.FlexDirection == FlexDirection.RowReverse)
                    {
                        children.Aggregate(
                            parentSize.X,
                            (right, child) => (child.Frame = new Rectangle(right - child.Size.X, child.Frame.Y, child.Size.X, child.Size.Y)).Left
                            );
                    }
                    break;
                case ContentJustification.End:
                    if (flex.FlexDirection == FlexDirection.Row)
                    {
                        children.Aggregate(
                            parentSize.X - children.Sum(child => child.Size.X),
                            (left, child) => (child.Frame = new Rectangle(left, child.Frame.Y, child.Size.X, child.Size.Y)).Right
                            );
                    }
                    else if (flex.FlexDirection == FlexDirection.RowReverse)
                    {
                        children.Aggregate(
                            children.Sum(child => child.Size.X),
                            (right, child) => (child.Frame = new Rectangle(right - child.Size.X, child.Frame.Y, child.Size.X, child.Size.Y)).Left
                            );
                    }
                    break;
                case ContentJustification.Center:
                    if (flex.FlexDirection == FlexDirection.Row)
                    {
                        children.Aggregate(
                            parentSize.X / 2 - children.Sum(child => child.Size.X) / 2,
                            (left, child) => (child.Frame = new Rectangle(left, child.Frame.Y, child.Size.X, child.Size.Y)).Right
                            );
                    }
                    else if (flex.FlexDirection == FlexDirection.RowReverse)
                    {
                        children.Aggregate(
                            parentSize.X / 2 + children.Sum(child => child.Size.X) / 2,
                            (right, child) => (child.Frame = new Rectangle(right - child.Size.X, child.Frame.Y, child.Size.X, child.Size.Y)).Left
                            );
                    }
                    break;
                case ContentJustification.SpaceBetween:
                    if (children.Count == 1)
                    {
                        var child = children.First();

                        if (flex.FlexDirection == FlexDirection.Row)
                        {
                            child.Frame = new Rectangle(0, child.Frame.Y, child.Size.X, child.Size.Y);
                        }
                        else if (flex.FlexDirection == FlexDirection.RowReverse)
                        {
                            child.Frame = new Rectangle(parentSize.X - child.Size.X, child.Frame.Y, child.Size.X, child.Size.Y);
                        }
                    }
                    else
                    {
                        var contentSize = children.Sum(child => child.Size.X);
                        var space = parentSize.X - contentSize;
                        var between = space / (children.Count - 1);

                        if (flex.FlexDirection == FlexDirection.Row)
                        {
                            children.Aggregate(
                                0,
                                (left, child) => (child.Frame = new Rectangle(left, child.Frame.Y, child.Size.X, child.Size.Y)).Right + between
                                );
                        }
                        else if (flex.FlexDirection == FlexDirection.RowReverse)
                        {
                            children.Aggregate(
                                parentSize.X,
                                (right, child) => (child.Frame = new Rectangle(right - child.Size.X, child.Frame.Y, child.Size.X, child.Size.Y)).Left - between
                                );
                        }
                    }
                    break;
                case ContentJustification.SpaceAround:
                    {
                        var contentSize = children.Sum(child => child.Size.X);
                        var space = parentSize.X - contentSize;
                        var around = space / children.Count;

                        if (flex.FlexDirection == FlexDirection.Row)
                        {
                            children.Aggregate(
                                around / 2,
                                (left, child) => (child.Frame = new Rectangle(left, child.Frame.Y, child.Size.X, child.Size.Y)).Right + around
                                );
                        }
                        else if (flex.FlexDirection == FlexDirection.RowReverse)
                        {
                            children.Aggregate(
                                parentSize.X - around / 2,
                                (right, child) => (child.Frame = new Rectangle(right - child.Size.X, child.Frame.Y, child.Size.X, child.Size.Y)).Left - around
                                );
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
