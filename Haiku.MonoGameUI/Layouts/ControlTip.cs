using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Haiku.MonoGameUI.Layouts
{
    public interface ControlTipping
    {
        bool IsVisible { get; }
        void ShowAnimated(string message);
        void Show(string message);
        void Show(string message, Point cursorPosition);
        void UpdatePosition(Point cursorPosition);
        void OnCursorMovedInControl(Point point);
        void OnCursorExitedControl();
        void HideAnimated();
        void Hide();
    }

    public class ControlTip : Panel, ControlTipping
    {
        /// <summary>
        /// Mouse cursor enters target area: display visual feedback within 0.1 seconds. - i.e. Change state from Normal to Highlighted
        /// Wait 0.3–0.5 seconds.
        /// If cursor remains stopped within target area, display corresponding hidden content within 0.1 seconds.
        /// Keep displaying the exposed content element until the cursor has left the triggering target area or the exposed content for longer than 0.5 seconds.
        /// </summary>
        const double ShowDelay = 0.2;
        const double HideDelay = 0.51;
        const int HorizontalMargin = 8;
        const int VerticalMargin = 2;
        bool ControlTipping.IsVisible => IsVisible;
        readonly Label label;
        readonly Point cursorOffset;
        readonly Point screenSize;
        Animation showAnimation;
        Animation hideAnimation;
        Point cursorPositionWhenShown;

        public ControlTip(Point cursorOffset, Point screenSize)
            : base(Rectangle.Empty)
        {
            this.cursorOffset = cursorOffset;
            this.screenSize = screenSize;
            label = new Label(new Rectangle(HorizontalMargin, VerticalMargin, 0, 0), "", BodyFont);
            AddChild(label);
            IsInteractionEnabled = false;
        }

        public void ShowAnimated(string message)
        {
            CancelAnimation(ref showAnimation);
            CancelAnimation(ref hideAnimation);
            Hide();
            showAnimation = 
                Animate()
                .After(ShowDelay)
                .Function((_, __) => { }, (_, __) =>
                {
                    var cursorPosition = Mouse.GetState().Position;
                    cursorPositionWhenShown = cursorPosition;
                    Show(message, cursorPosition);
                })
                .Build();
            ParentWindow?.Add(showAnimation);
        }

        public void Show(string message)
        {
            Show(message, Mouse.GetState().Position);
        }

        public void Show(string message, Point cursorPosition)
        {
            label.Text = message;
            label.SizeToFit();
            var size = new Point(label.Frame.Width + HorizontalMargin * 2, label.Frame.Height + VerticalMargin * 2);
            Frame = FrameWithinScreenSize(cursorPosition + cursorOffset, size, screenSize);
            IsVisible = true;
        }

        public void OnCursorExitedControl()
        {
            CancelAnimation(ref showAnimation);
            HideAnimated();
        }

        public void UpdatePosition(Point cursorPosition)
        {
            Frame = FrameWithinScreenSize(cursorPosition + cursorOffset, Frame.Size, screenSize);
        }

        public void OnCursorMovedInControl(Point point)
        {
            if (IsVisible
                && IsMouseMoveThresholdExceeded(cursorPositionWhenShown, point))
            {
                HideAnimated();
            }
        }

        public void HideAnimated()
        {
            if (IsVisible == true
                && hideAnimation == null)
            {
                CancelAnimation(ref showAnimation);
                hideAnimation = 
                    Animate()
                    .After(HideDelay)
                    .Function((_, __) => { }, (_, __) =>
                    {
                        Hide();
                        hideAnimation = null;
                    })
                    .Build();
                ParentWindow?.Add(hideAnimation);
            }
        }

        public void Hide()
        {
            IsVisible = false;
        }

        public static Rectangle FrameWithinScreenSize(Point position, Point size, Point screenSize)
        {
            var x = MathHelper.Clamp(position.X, 0, screenSize.X - size.X);
            var y = position.Y - size.Y;
            y = MathHelper.Clamp(y, 0, screenSize.Y - size.Y);
            return new Rectangle(x, y, size.X, size.Y);
        }

        void CancelAnimation(ref Animation displayAnimation)
        {
            if (displayAnimation != null
                && !displayAnimation.IsComplete)
            {
                ParentWindow?.CancelAnimation(displayAnimation);
                displayAnimation = null;
            }
        }
    }
}
