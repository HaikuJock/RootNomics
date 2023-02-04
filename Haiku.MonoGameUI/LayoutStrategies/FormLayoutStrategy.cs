using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Haiku.MonoGameUI.LayoutStrategies
{
    public class FormLayoutStrategy : LayoutStrategy
    {
        readonly int padding;

        public Point ParentSize { get; private set; }

        public Point ContentSize { get; private set; }

        public FormLayoutStrategy(int padding = 0)
        {
            this.padding = padding;
        }

        public void LayoutChildren(Point parentSize, List<Layout> children)
        {
            ParentSize = parentSize;
            ContentSize = parentSize;
            if (children.Count > 0)
            {
                var leftChild = children[0];
                leftChild.Frame = new Rectangle(padding, 0, leftChild.Frame.Width, leftChild.Frame.Height);
                leftChild.CenterYInParent();
            }
            if (children.Count > 1)
            {
                var rightChild = children[1];
                var left = parentSize.X - rightChild.Frame.Width - padding;
                rightChild.Frame = new Rectangle(left, 0, rightChild.Frame.Width, rightChild.Frame.Height);
                rightChild.CenterYInParent();
            }
        }
    }
}
