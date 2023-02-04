using Microsoft.Xna.Framework;

namespace Haiku.MonoGameUI.Layouts
{
    public delegate void OnCursorEnteredDelegate<T>(T control);

    public class GroupButton : Button
    {
        public OnCursorEnteredDelegate<GroupButton> OnCursorEnteredAction;
        public ButtonGrouping ButtonGroup => buttonGroup ?? Parent as ButtonGrouping;
        readonly ButtonGrouping buttonGroup;

        public GroupButton(Rectangle frame)
            : base(frame)
        {
        }

        public GroupButton(Rectangle frame, ButtonTriggerHandler action)
            : base(frame, action)
        {
        }
        public GroupButton(Rectangle frame, ButtonTriggerHandler action, string title)
            : base(frame, action, title)
        {
        }

        public GroupButton(Rectangle frame, ButtonTriggerHandler action, ButtonGrouping buttonGroup)
            : base(frame, action)
        {
            this.buttonGroup = buttonGroup;
        }

        public override void Trigger()
        {
            ParentWindow?.PlaySound(TriggerOnSound);
            ButtonGroup?.DeselectAll();
            State = ControlState.Selected;
            Action(this);
        }

        protected override bool OnPress(Point point, Rectangle container)
        {
            if (State != ControlState.Selected)
            {
                State = ControlState.Highlighted;
            }
            return true;
        }

        protected override bool OnRelease(Point point, Rectangle container)
        {
            if (State == ControlState.Highlighted
                || State == ControlState.Selected)
            {
                Trigger();
                return true;
            }
            return false;
        }

        protected override void OnCursorEntered(Point point, Rectangle container)
        {
            OnCursorEnteredAction?.Invoke(this);
            base.OnCursorEntered(point, container);
        }
    }
}
