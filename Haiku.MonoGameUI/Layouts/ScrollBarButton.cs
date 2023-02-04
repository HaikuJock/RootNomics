using Microsoft.Xna.Framework;

namespace Haiku.MonoGameUI.Layouts
{
    class ScrollBarButton : Button
    {
        readonly Scrollable scrollable;

        internal ScrollBarButton(Rectangle frame, Scrollable scrollable) 
            : base(frame)
        {
            this.scrollable = scrollable;
        }

        protected override bool OnScroll(int scrollDistance, Point point, Rectangle container)
        {
            scrollable.MouseScroll(this, scrollDistance);
            return true;
        }
    }
}
