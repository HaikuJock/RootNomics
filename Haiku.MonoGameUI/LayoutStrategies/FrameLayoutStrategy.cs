using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Haiku.MonoGameUI.LayoutStrategies
{
    public class FrameLayoutStrategy : LayoutStrategy
    {
        public Point ParentSize { get; private set; }
        public Point ContentSize { get; private set; }

        public void LayoutChildren(Point parentSize, List<Layout> children)
        {
            ParentSize = parentSize;
            ContentSize = parentSize;
        }
    }
}
