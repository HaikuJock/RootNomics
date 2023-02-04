using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;

namespace Haiku.MonoGameUI.Layouts
{
    public class FlexLayout : Layout
    {
        public FlexLayout()
            : base(Rectangle.Empty, new FlexLayoutStrategy())
        {
        }
    }
}
