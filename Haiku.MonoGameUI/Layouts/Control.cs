using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Haiku.MonoGameUI.Layouts
{
    public enum ControlState
    {
        Normal,
        Active,
        Highlighted,
        Selected,

        Count
    }

    public class Control : Panel
    {
        public virtual ControlState State {
            get
            {
                return state;
            }
            set
            {
                state = value;
                OnNewState();
            }
        }

        public virtual bool IsFocusable { get; set; }
        public ControlTipping Tip;
        public string TipMessage
        {
            get { return tipMessage; }
            set
            {
                if (Tip != null
                    && tipMessage != value
                    && Tip.IsVisible
                    && CalculateWorldFrame().Contains(Mouse.GetState().Position))
                {
                    Tip.Show(value);
                }
                tipMessage = value;
            }
        }

        ControlState state;
        string tipMessage;

        public Control(int size)
            : this(size, size)
        {
        }

        public Control(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public Control(int x, int y, int width, int height)
            : this(new Rectangle(x, y, width, height))
        {
        }

        public Control(Rectangle frame)
            : this(frame, new FrameLayoutStrategy())
        {
        }

        public Control(Rectangle frame, LayoutStrategy layoutStrategy)
            : base(frame, layoutStrategy)
        {
        }

        public override void SetFrameWithSideEffects(Rectangle newFrame)
        {
            base.SetFrameWithSideEffects(newFrame);
            if (Tip != null
                && !string.IsNullOrEmpty(tipMessage)
                && Tip.IsVisible
                && CalculateWorldFrame().Contains(Mouse.GetState().Position))
            {
                Tip.Show(tipMessage);
            }
        }

        public virtual void Trigger()
        {
            State = ControlState.Active;
        }

        internal virtual void OnNewState()
        {
        }

        internal virtual bool HandleTextInput(Haiku.UI.Keys key, string keyAsString, IEnumerable<Haiku.UI.Keys> activeModifierKeys)
        {
            return false;
        }

        internal virtual bool HandleKeyPress(Haiku.UI.Keys key, IEnumerable<Haiku.UI.Keys> activeModifierKeys, TextClipboarding clipboard)
        {
            return false;
        }

        internal virtual void UpdateFocused(double deltaSeconds)
        {
        }

        internal virtual void OnLoseFocus()
        {
            State = ControlState.Normal;
        }

        internal virtual void OnGainFocus()
        {
            if (State == ControlState.Normal)
            {
                State = ControlState.Active;
            }
        }

        protected override void OnCursorEntered(Point point, Rectangle container)
        {
            if (IsInteractable() 
                && Tip != null
                && !string.IsNullOrEmpty(TipMessage))
            {
                Tip.ShowAnimated(tipMessage);
            }
        }

        protected override void OnCursorExited(Point point, Rectangle container)
        {
            Tip?.OnCursorExitedControl();
        }

        protected override bool OnCursorMove(Point point, Rectangle container)
        {
            Tip?.OnCursorMovedInControl(point);
            return base.OnCursorMove(point, container);
        }
    }
}
