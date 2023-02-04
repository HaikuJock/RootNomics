using Microsoft.Xna.Framework;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class CheckBox : ToggleButton
    {
        public static SpriteFrame BoxTexture;
        public static SpriteFrame BoxTextureActive;
        public static SpriteFrame CheckTexture;
        public const int CheckBoxSize = 32;
        readonly Panel activePanel;

        public bool IsChecked
        {
            get => IsOn;
            set
            {
                bool changing = IsOn != value || isPartiallyChecked;
                IsOn = value;
                isPartiallyChecked = false;
                if (changing)
                {
                    OnNewState();
                }
            }
        }

        public bool IsPartiallyChecked 
        {
            get => isPartiallyChecked;
            set
            {
                bool changing = (value && IsOn) || isPartiallyChecked != value;
                if (value)
                {
                    IsOn = false;
                }
                isPartiallyChecked = value;
                if (changing)
                {
                    OnNewState();
                }
            }
        }
        bool isPartiallyChecked;

        public CheckBox()
            : this(0, 0, false)
        {
        }

        public CheckBox(int x, int y, bool isChecked = false)
            : this(new Rectangle(x, y, CheckBoxSize, CheckBoxSize), isChecked)
        {
        }

        public CheckBox(Rectangle frame, bool isChecked) 
            : base(frame)
        {
            SetBackground(BoxTexture, BoxTexture, BoxTexture);
            IsFocusable = true;
            IsChecked = isChecked;
            var activeSize = BoxTextureActive.SourceRectangle.Width;
            activePanel = new Panel(activeSize, activeSize)
            {
                IsInteractionEnabled = false,
                Texture = BoxTextureActive
            };
            AddChild(activePanel);
            activePanel.CenterInParent();
            activePanel.IsVisible = false;
        }

        public override void Trigger()
        {
            if (IsChecked)
            {
                IsChecked = false;
            }
            else
            {
                IsChecked = true;
            }
            Action(this);
        }

        internal override void OnLoseFocus()
        {
            if (State != ControlState.Selected)
            {
                State = ControlState.Normal;
            }
            activePanel.IsVisible = false;
        }

        internal override void OnGainFocus()
        {
            activePanel.IsVisible = true;
            base.OnGainFocus();
        }

        internal override void OnNewState()
        {
            switch (State)
            {
                case ControlState.Normal:
                case ControlState.Active:
                case ControlState.Highlighted:
                    if (isPartiallyChecked)
                    {
                        SetForeground(CheckTexture);
                        foregroundPanel.Alpha = 0.5f;
                    }
                    else
                    {
                        SetForeground("");
                    }
                    break;
                case ControlState.Selected:
                    SetForeground(CheckTexture);
                    break;
                default:
                    break;
            }
            base.OnNewState();
        }
    }
}
