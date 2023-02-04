using Microsoft.Xna.Framework;

namespace Haiku.MonoGameUI.Layouts
{
    public class ToggleButton : Button
    {
        public string TriggerOffSound { get; set; } = "UI/buttonToggleOff";
        public bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                State = value ? ControlState.Selected : ControlState.Normal;
            }
        }
        bool isOn;

        public ToggleButton(Rectangle frame)
            : base(frame)
        {
            TriggerOnSound = "UI/buttonToggleOn";
        }

        public override void Trigger()
        {
            if (IsOn)
            {
                ParentWindow?.PlaySound(TriggerOffSound);
                IsOn = false;
                State = ControlState.Active;
                Action(this);
            }
            else
            {
                ParentWindow?.PlaySound(TriggerOnSound);
                IsOn = true;
                Action(this);
            }
        }

        protected override bool OnRelease(Point point, Rectangle container)
        {
            if (State == ControlState.Highlighted)
            {
                Trigger();
            }
            return true;
        }

        protected override void OnCursorExited(Point point, Rectangle container)
        {
            if (State == ControlState.Highlighted)
            {
                if (IsOn)
                {
                    State = ControlState.Selected;
                }
                else
                {
                    State = ControlState.Normal;
                }
            }
            base.OnCursorExited(point, container);
        }
    }
}
