using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;

namespace Haiku.MonoGameUI.Layouts
{
    public class FormLayout : Layout
    {
        public FormLayout(Rectangle frame)
            : base(frame, new FormLayoutStrategy())
        {
        }
    }
}
