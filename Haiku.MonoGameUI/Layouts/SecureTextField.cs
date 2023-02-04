using Microsoft.Xna.Framework;
using System;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class SecureTextField : TextField
    {
        const int ButtonSize = 44;
        const int Spacing = 4;
        public static SpriteFrame ShowPasswordIcon;
        public static SpriteFrame HidePasswordIcon;
        public static NinePatch BackgroundNormal;
        public static NinePatch BackgroundActive;
        public static NinePatch BackgroundHighlighted;
        public static NinePatch BackgroundSelected;
        readonly ToggleButton toggleVisibilityButton;

        public SecureTextField(Rectangle frame, Action<string> onDone = null) 
            : base(new Rectangle(frame.X, frame.Y, frame.Width - ButtonSize - Spacing, frame.Height), onDone)
        {
            Frame = frame;
            var buttonFrame = new Rectangle(frame.Width - ButtonSize, 0, ButtonSize, ButtonSize);
            toggleVisibilityButton = new ToggleButton(buttonFrame)
            {
                Action = ToggleTextVisibility,
                IsFocusable = true,
            };
            toggleVisibilityButton.SetBackground(BackgroundNormal, ControlState.Normal);
            toggleVisibilityButton.SetBackground(BackgroundActive, ControlState.Active);
            toggleVisibilityButton.SetBackground(BackgroundHighlighted, ControlState.Highlighted);
            toggleVisibilityButton.SetBackground(BackgroundSelected, ControlState.Selected);
            toggleVisibilityButton.SetForeground(ShowPasswordIcon);
            AddChild(toggleVisibilityButton);
        }

        public void HideText()
        {
            if (toggleVisibilityButton.IsOn)
            {
                ToggleTextVisibility(toggleVisibilityButton);
            }
        }

        protected override void SetLabelText(string text)
        {
            if (toggleVisibilityButton.IsOn)
            {
                base.SetLabelText(text);
            }
            else
            {
                var maskedText = new string('*', text.Length);
                base.SetLabelText(maskedText);
            }
        }

        protected override void Copy(TextClipboarding clipboard)
        {
            if (toggleVisibilityButton.IsOn)
            {
                base.Copy(clipboard);
            }
        }

        protected override void Cut(TextClipboarding clipboard)
        {
            if (toggleVisibilityButton.IsOn)
            {
                base.Cut(clipboard);
            }
        }

        void ToggleTextVisibility(object obj)
        {
            if (toggleVisibilityButton.IsOn)
            {
                toggleVisibilityButton.SetForeground(HidePasswordIcon);
            }
            else
            {
                toggleVisibilityButton.SetForeground(ShowPasswordIcon);
            }
            SetLabelText();
            UpdateChildrenPositions();
        }
    }
}
