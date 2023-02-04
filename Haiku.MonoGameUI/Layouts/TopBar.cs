using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public delegate void TopBarPressHandler();

    public class TopBar : Control
    {
        public const int Height = 44;
        const int ControlSpacing = 4;
        public static SpriteFrame Background;
        public TopBarPressHandler OnTopBarPress;
        public TopBarPressHandler OnTopBarAltPress;
        readonly Layout layout;
        Point previousMovePoint;

        public TopBar(int width, Layout layout)
            : this(new Rectangle(0, 0, width, Height), layout)
        {
        }

        public TopBar(Rectangle frame, Layout layout)
            : this(frame, layout, new List<Layout>(), new List<Layout>())
        {
        }

        public TopBar(Rectangle frame, Layout layout, List<Layout> leftControls, List<Layout> rightControls)
            : base(frame, new FormLayoutStrategy())
        {
            this.layout = layout;
            Texture = Background;
            IsDraggingEnabled = true;
            AddControls(leftControls, rightControls);
        }

        void AddControls(List<Layout> leftControls, List<Layout> rightControls)
        {
            if (leftControls.Count > 0)
            {
                var leftLayout = new LinearLayout(Orientation.Horizontal, ControlSpacing);
                var leftSpacing = new Layout(new Rectangle(0, 0, ControlSpacing - 2, Height))
                {
                    IsInteractionEnabled = false,
                };

                leftLayout.AddChild(leftSpacing);
                leftLayout.AddChildren(leftControls);
                AddChild(leftLayout);
                foreach (var child in leftControls)
                {
                    child.CenterYInParent();
                    child.Frame = new Rectangle(child.Frame.X, child.Frame.Y - 1, child.Frame.Width, child.Frame.Height);
                }
            }
            if (rightControls.Count > 0)
            {
                var rightLayout = new LinearLayout(Orientation.Horizontal, ControlSpacing);
                var rightSpacing = new Layout(new Rectangle(0, 0, ControlSpacing, Height))
                {
                    IsInteractionEnabled = false,
                };

                rightLayout.AddChildren(rightControls);
                rightLayout.AddChild(rightSpacing);
                if (leftControls.Count > 0)
                {
                    AddChild(rightLayout);
                }
                else
                {
                    AddChildren(new[] { new Layout(0), rightLayout });
                }
                foreach (var child in rightControls)
                {
                    child.CenterYInParent();
                    child.Frame = new Rectangle(child.Frame.X, child.Frame.Y - 1, child.Frame.Width, child.Frame.Height);
                }
            }
        }

        protected override bool OnPress(Point point, Rectangle container)
        {
            OnTopBarPress?.Invoke();
            previousMovePoint = point;
            return true;
        }

        protected override bool OnAltPress(Point point, Rectangle container)
        {
            OnTopBarAltPress?.Invoke();
            return true;
        }

        public override bool OnCursorDrag(Point point, Rectangle container)
        {
            UpdateFrame(point);
            return true;
        }

        public override bool OnCursorDragOutside(Point point)
        {
            UpdateFrame(point);
            return true;
        }

        protected override bool OnRelease(Point point, Rectangle container)
        {
            return true;
        }

        protected override bool OnAltRelease(Point point, Rectangle container)
        {
            return true;
        }

        void UpdateFrame(Point point)
        {
            var distance = point - previousMovePoint;

            layout.Frame = new Rectangle(layout.Frame.X + distance.X, layout.Frame.Y + distance.Y, layout.Frame.Width, layout.Frame.Height);
            previousMovePoint = point;
        }
    }
}
