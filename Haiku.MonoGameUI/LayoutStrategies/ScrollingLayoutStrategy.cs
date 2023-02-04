using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Haiku.MonoGameUI.LayoutStrategies
{
    public class ScrollingLayoutStrategy : LinearLayoutStrategy
    {
        public ScrollingLayoutStrategy(Orientation orientation, int spacing, int startPadding, int endPadding) 
            : base(orientation, spacing, startPadding, endPadding, Direction.Forward)
        {
        }

        public override void LayoutChildren(Point parentSize, List<Layout> children)
        {
            base.LayoutChildren(parentSize, children);
            ParentSize = parentSize;
        }
    }
}
