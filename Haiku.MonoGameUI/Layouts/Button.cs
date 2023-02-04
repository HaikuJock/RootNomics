using Haiku.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public delegate void ButtonTriggerHandler(object button);

    public class Button : Control
    {
        public string TriggerOnSound { get; set; } = "UI/buttonPress";
        public virtual int Tag { get; set; }
        public virtual SpriteFrame ForegroundImage { get { return foregroundPanel?.Texture; } }
        public ButtonTriggerHandler Action;
        public string Title => (foregroundPanel as Label)?.Text ?? "";
        readonly SpriteFrame[] stateBackgrounds = new SpriteFrame[(int)ControlState.Count];
        protected Panel foregroundPanel;

        public Button()
            : this(0, 0)
        {
        }

        public Button(int width, int height)
            : this(new Rectangle(0, 0, width, height))
        {
        }

        public Button(Point size)
            : this(new Rectangle(Point.Zero, size))
        {
        }

        public Button(Point size, ButtonTriggerHandler action, string title)
            : this(new Rectangle(Point.Zero, size), action, title)
        {
        }

        public Button(Rectangle frame)
            : this(frame, (_) => { })
        {
        }

        public Button(Rectangle frame, ButtonTriggerHandler action, string title)
            : this(Rectangle.Empty, action)
        {
            var stringSize = MainFont.MeasureString(title);
            var width = Math.Max((int)stringSize.X, frame.Width);
            var height = Math.Max((int)stringSize.Y, frame.Height);
            Frame = new Rectangle(frame.X, frame.Y, width, height);
            SetForeground(title);
            SetBackground(Color.White);
        }

        public Button(Rectangle frame, ButtonTriggerHandler action)
            : base(frame)
        {
            Action = action;
        }

        public void SetForeground(SpriteFrame icon)
        {
            var x = Frame.Width / 2 - icon.SourceRectangle.Width / 2;
            var y = Frame.Height / 2 - icon.SourceRectangle.Height / 2;
            var imagePanel = new Panel(new Rectangle(
                x, y,
                icon.SourceRectangle.Width, icon.SourceRectangle.Height))
            {
                Texture = icon
            };
            SetForegroundPanel(imagePanel);
        }

        public void SetForeground(string text)
        {
            SetForeground(text, MainFont);
        }

        public void SetForeground(string text, ContentAlignment alignment)
        {
            SetForeground(text, MainFont, Color.Black, Color.Transparent, alignment);
        }

        public void SetForeground(string text, SpriteFont font)
        {
            SetForeground(text, font, Color.Black);
        }

        public void SetForeground(string text, SpriteFont font, Color foreground)
        {
            SetForeground(text, font, foreground, Color.Transparent, ContentAlignment.Centre);
        }

        public void SetForeground(string text, SpriteFont font, Color foreground, Color background, ContentAlignment alignment)
        {
            var size = font.MeasureString(text);
            var label = new Label(0, 0, (int)size.X, (int)size.Y)
            {
                Font = font,
                Text = text,
                TextColor = foreground,
                BackgroundColor = background
            };
            SetForeground(label, alignment);
        }

        public void SetForeground(Label label, ContentAlignment alignment)
        {
            var size = label.Frame.Size;
            var x = alignment == ContentAlignment.Left ? 8 : alignment.HorizontalPositionIn(Frame.Width, size.X);
            var y = Frame.Height / 2 - size.Y / 2;

            label.Frame = new Rectangle(x, y, size.X, size.Y);
            SetForegroundPanel(label);
        }

        public void SetForegroundPanel(Panel panel)
        {
            foregroundPanel?.RemoveFromParent();
            foregroundPanel = panel;
            foregroundPanel.IsInteractionEnabled = false;
            AddChild(foregroundPanel);
        }

        public void SetBackground(Color backgroundColor)
        {
            SetBackground(backgroundColor, backgroundColor, backgroundColor, backgroundColor);
        }

        public void SetBackground(Color normal, Color selected)
        {
            SetBackground(normal, normal, selected, selected);
        }

        public void SetBackground(Color normal, Color active, Color highlighted, Color selected)
        {
            SetBackground(normal, ControlState.Normal);
            SetBackground(active, ControlState.Active);
            SetBackground(highlighted, ControlState.Highlighted);
            SetBackground(selected, ControlState.Selected);
        }

        public void SetBackground(Color backgroundColor, ControlState forState)
        {
            SetBackground(ColorSprite(backgroundColor), forState);
        }

        public void SetBackground(SpriteFrame background, ControlState forState)
        {
            stateBackgrounds[(int)forState] = background;
            if (State == forState)
            {
                Texture = stateBackgrounds[(int)State];
            }
        }

        public void SetBackground(SpriteFrame normal, SpriteFrame active, SpriteFrame selected)
        {
            SetBackground(normal, ControlState.Normal);
            SetBackground(active, ControlState.Active);
            SetBackground(selected, ControlState.Highlighted);
            SetBackground(selected, ControlState.Selected);
        }

        protected override bool OnPress(Point point, Rectangle container)
        {
            State = ControlState.Highlighted;
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

        public override void Trigger()
        {
            if (!string.IsNullOrEmpty(TriggerOnSound))
            {
                ParentWindow?.PlaySound(TriggerOnSound);
            }
            base.Trigger();
            Action(this);
        }

        protected override void OnCursorEntered(Point point, Rectangle container)
        {
            if (State == ControlState.Normal)
            {
                State = ControlState.Active;
            }
            base.OnCursorEntered(point, container);
        }

        protected override void OnCursorExited(Point point, Rectangle container)
        {
            if (State == ControlState.Highlighted
                || State == ControlState.Active)
            {
                State = ControlState.Normal;
            }
            base.OnCursorExited(point, container);
        }

        internal override bool HandleKeyPress(Keys key, IEnumerable<Keys> activeModifierKeys, TextClipboarding clipboard)
        {
            if (IsFocusable
                && (key == Keys.Space
                    || key == Keys.Enter))
            {
                Trigger();
                return true;
            }
            return base.HandleKeyPress(key, activeModifierKeys, clipboard);
        }

        internal override void OnNewState()
        {
            Texture = stateBackgrounds[(int)State];
        }
    }
}
