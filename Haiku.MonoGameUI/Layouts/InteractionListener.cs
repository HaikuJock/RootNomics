using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;

namespace Haiku.MonoGameUI.Layouts
{
    public delegate void PressHandler(Point point, Rectangle container);

    public class InteractionListener : Layout
    {
        public PressHandler OnPressed;

        public InteractionListener(Rectangle frame)
            : base(frame, new FrameLayoutStrategy())
        {
        }

        protected override bool OnPress(Point point, Rectangle container)
        {
            OnPressed?.Invoke(point, container);
            return false;
        }

        protected override bool OnAltPress(Point point, Rectangle container)
        {
            OnPressed?.Invoke(point, container);
            return false;
        }

        protected override bool OnScroll(int scrollDistance, Point point, Rectangle container)
        {
            OnPressed?.Invoke(point, container);
            return false;
        }
    }
}
