using Microsoft.Xna.Framework;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class DropDownButton : Button
    {
        public static SpriteFrame ArrowNormal;
        public static SpriteFrame ArrowActive;
        public static SpriteFrame ArrowSelected;
        readonly ImageControl arrow;

        public DropDownButton(Rectangle frame, int indent) 
            : base(frame)
        {
            var arrowSize = ArrowNormal.SourceRectangle.Size;
            var arrowFrame = new Rectangle(new Point(frame.Width - arrowSize.X - indent, 0), arrowSize);

            arrow = new ImageControl(arrowFrame, ContentAlignment.Centre)
            {
                Texture = ArrowNormal,
                IsInteractionEnabled = false,
            };
            AddChild(arrow);
            arrow.CenterYInParent();
        }

        public void Disable()
        {
            IsInteractionEnabled = false;
            arrow.Alpha = 0.5f;
            foregroundPanel.Alpha = 0.75f;
        }

        protected override bool OnPress(Point point, Rectangle container)
        {
            State = ControlState.Highlighted;
            Trigger();
            return true;
        }

        protected override bool OnRelease(Point point, Rectangle container)
        {
            if (State == ControlState.Highlighted)
            {
                Trigger();
            }
            return true;
        }

        internal override void OnNewState()
        {
            base.OnNewState();
            arrow.Texture = State switch
            {
                ControlState.Normal => ArrowNormal,
                ControlState.Active => ArrowActive,
                ControlState.Highlighted => ArrowSelected,
                ControlState.Selected => ArrowSelected,
                _ => ArrowNormal,
            };
        }
    }
}
