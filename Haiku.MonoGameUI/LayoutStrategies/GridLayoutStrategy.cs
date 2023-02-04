using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Haiku.MonoGameUI.LayoutStrategies
{
    public class GridLayoutStrategy : LayoutStrategy
    {
        readonly Point cellSize;
        readonly Point gridSize;

        public Point ParentSize { get; private set; }

        public Point ContentSize { get; private set; }

        public GridLayoutStrategy(Point gridSize, Point cellSize)
        {
            this.cellSize = cellSize;
            this.gridSize = gridSize;
        }

        public void LayoutChildren(Point parentSize, List<Layout> children)
        {
            var childIndex = 0;
            var contentSize = Point.Zero;

            ParentSize = parentSize;

            for (int j = 0; j < gridSize.Y; j++)
            {
                for (int i = 0; i < gridSize.X; i++)
                {
                    if (childIndex < children.Count)
                    {
                        var cellFrame = new Rectangle(new Point(i * cellSize.X, j * cellSize.Y), cellSize);
                        var cell = children[childIndex];

                        cell.Frame = cellFrame;

                        childIndex++;
                        contentSize.X = Math.Max(contentSize.X, i * cellSize.X);
                        contentSize.Y = Math.Max(contentSize.Y, j * cellSize.Y);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            ContentSize = contentSize;
        }
    }
}
