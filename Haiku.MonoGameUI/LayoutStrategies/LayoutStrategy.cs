using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Haiku.MonoGameUI.LayoutStrategies
{
    public interface LayoutStrategy
    {
        Point ParentSize { get; }
        Point ContentSize { get; }
        void LayoutChildren(Point parentSize, List<Layout> children);
    }
}
