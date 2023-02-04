using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class ScrollPanel : Panel
    {
        public static SpriteFrame BottomShadow;
        public static SpriteFrame TopShadow;
        public static SpriteFrame LeftAndCornersShadow;
        public static SpriteFrame RightAndCornersShadow;

        public ScrollContent Content { get; }
        public bool IsFocusable {
            get => false;
            set
            {
                if (Content.ScrollBar != null)
                {
                    Content.ScrollBar.IsFocusable = value;
                }
            }
        }
        List<Layout> shadows;

        public ScrollPanel(
            Rectangle frame,
            Orientation orientation,
            int spacing,
            int padding,
            bool includeScrollBar = true,
            bool wrapContent = false,
            bool showShadows = true) 
            : base(frame)
        {
            BackgroundColor = Color.Transparent;
            if (includeScrollBar)
            {
                Rectangle contentFrame;
                Rectangle barFrame;

                if (orientation == Orientation.Horizontal)
                {
                    contentFrame = new Rectangle(0, 0, frame.Width, frame.Height - ScrollBar.BarSize);
                    barFrame = new Rectangle(0, frame.Height - ScrollBar.BarSize, frame.Width, ScrollBar.BarSize);
                }
                else
                {
                    contentFrame = new Rectangle(0, 0, frame.Width - ScrollBar.BarSize, frame.Height);
                    barFrame = new Rectangle(frame.Width - ScrollBar.BarSize, 0, ScrollBar.BarSize, frame.Height);
                }
                Content = new ScrollContent(contentFrame, orientation, spacing, padding, padding, wrapContent);
                var scrollBar = new ScrollBar(barFrame, Content);
                Content.ScrollBar = scrollBar;
                Content.UpdateScrollingEnabled();
                AddChild(Content);
                AddChild(scrollBar);
            }
            else
            {
                Content = new ScrollContent(new Rectangle(Point.Zero, frame.Size), orientation, spacing, padding, padding, wrapContent);
                AddChild(Content);
            }
            if (showShadows)
            {
                AddShadows();
            }
        }

        public void ReplaceContent(IEnumerable<Layout> layouts)
        {
            Content.ReplaceContent(layouts);
        }

        public void AddContent(Layout layout)
        {
            Content.AddChild(layout);
        }

        public void AddContent(IEnumerable<Layout> layouts)
        {
            Content.AddChildren(layouts);
        }

        public void MoveContent(int indexFrom, int indexTo)
        {
            Content.MoveChild(indexFrom, indexTo);
        }

        public void ClearContent()
        {
            ReplaceContent(new List<Layout>());
        }

        public override void SetFrameWithSideEffects(Rectangle frame)
        {
            base.SetFrameWithSideEffects(frame);
            Rectangle contentFrame;
            Rectangle barFrame;

            if (Content.Orientation == Orientation.Horizontal)
            {
                contentFrame = new Rectangle(0, 0, frame.Width, frame.Height - ScrollBar.BarSize);
                barFrame = new Rectangle(0, frame.Height - ScrollBar.BarSize, frame.Width, ScrollBar.BarSize);
            }
            else
            {
                contentFrame = new Rectangle(0, 0, frame.Width - ScrollBar.BarSize, frame.Height);
                barFrame = new Rectangle(frame.Width - ScrollBar.BarSize, 0, ScrollBar.BarSize, frame.Height);
            }
            Content.SetFrameWithSideEffects(contentFrame);
            Content.ScrollBar.SetFrameWithSideEffects(barFrame);

            if (shadows != null)
            {
                foreach (var shadow in shadows)
                {
                    shadow.RemoveFromParent();
                }
                AddShadows();
            }
            DoLayout();
        }

        void AddShadows()
        {
            Point size = Content.Frame.Size;
            var horizontalWidth = LeftAndCornersShadow.SourceRectangle.Width;
            var horizontalHeight = size.Y;
            var verticalWidth = size.X - 2 * horizontalWidth;
            var verticalHeight = TopShadow.SourceRectangle.Height;

            var topFrame = new Rectangle(horizontalWidth, 0, verticalWidth, verticalHeight);
            var topPanel = new Panel(topFrame)
            {
                Texture = TopShadow,
                IsInteractionEnabled = false,
            };
            var bottomFrame = new Rectangle(horizontalWidth, size.Y - verticalHeight, verticalWidth, verticalHeight);
            var bottomPanel = new Panel(bottomFrame)
            {
                Texture = BottomShadow,
                IsInteractionEnabled = false,
            };

            var leftFrame = new Rectangle(0, 0, horizontalWidth, horizontalHeight);
            var leftPanel = new Panel(leftFrame)
            {
                Texture = LeftAndCornersShadow,
                IsInteractionEnabled = false,
            };
            var rightFrame = new Rectangle(size.X - horizontalWidth, 0, horizontalWidth, horizontalHeight);
            var rightPanel = new Panel(rightFrame)
            {
                Texture = RightAndCornersShadow,
                IsInteractionEnabled = false,
            };

            shadows = new List<Layout> { topPanel, leftPanel, bottomPanel, rightPanel };
            AddChildren(shadows);
        }
    }
}
