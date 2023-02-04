using Microsoft.Xna.Framework;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class PressShrinkReleaseGrowBar : Control
    {
        public const int Height = 44;
        public static SpriteFrame Background;
        readonly Layout layout;
        readonly Layout contentLayout;
        public Rectangle LayoutFrame;
        Point startMovePoint;

        public PressShrinkReleaseGrowBar(int width, Layout layout, Layout contentLayout)
            : base(new Rectangle(0, 0, width, Height))
        {
            this.layout = layout;
            this.contentLayout = contentLayout;
            LayoutFrame = layout.Frame;
            Texture = Background;
            IsDraggingEnabled = true;
        }

        protected override bool OnPress(Point point, Rectangle container)
        {
            layout.Frame = new Rectangle(layout.Frame.X, layout.Frame.Y, LayoutFrame.Width, Height);
            startMovePoint = point;
            contentLayout.IsVisible = false;
            return true;
        }

        public override void OnDragStart(Point point, Rectangle container)
        {
            base.OnDragStart(point, container);
        }

        public override bool OnCursorDrag(Point point, Rectangle container)
        {
            UpdateFrame(point);
            return base.OnCursorDrag(point, container);
        }

        public override bool OnCursorDragOutside(Point point)
        {
            UpdateFrame(point);
            return base.OnCursorDragOutside(point);
        }

        public override void OnDragEnd(Point point, Rectangle container)
        {
            base.OnDragEnd(point, container);
        }

        protected override bool OnRelease(Point point, Rectangle container)
        {
            layout.Frame = new Rectangle(layout.Frame.X, layout.Frame.Y, layout.Frame.Width, LayoutFrame.Height);
            contentLayout.IsVisible = true;
            return base.OnRelease(point, container);
        }

        void UpdateFrame(Point point)
        {
            var distance = point - startMovePoint;

            layout.Frame = new Rectangle(layout.Frame.X + distance.X, layout.Frame.Y + distance.Y, layout.Frame.Width, Height);
            startMovePoint = point;
        }
    }
}
