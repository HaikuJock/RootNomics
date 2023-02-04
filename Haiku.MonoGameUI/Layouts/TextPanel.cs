using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Haiku.MonoGameUI.Layouts
{
    public class TextPanel : Panel
    {
        readonly Label label;
        readonly ContentAlignment alignment;
        public string Text { get { return label.Text; } set { label.Text = value; SizeToFit(); } }
        public SpriteFont Font { get { return label.Font; } set { label.Font = value; SizeToFit(); } }
        public Color TextColor { get { return label.TextColor; } set { label.TextColor = value; } }
        public Point ShadowOffset { get => label.ShadowOffset; set => label.ShadowOffset = value; }
        public Color ShadowColor { get => label.ShadowColor; set => label.ShadowColor = value; }

        public TextPanel(ContentAlignment alignment)
            : this(Rectangle.Empty, new Label(""), alignment)
        {
        }

        public TextPanel(Rectangle frame, string text, ContentAlignment alignment)
            : this(frame, new Label(text), alignment)
        {
        }

        public TextPanel(Rectangle frame, Label label, ContentAlignment alignment)
            : base(frame)
        {
            this.label = label;
            this.alignment = alignment;
            BackgroundColor = Color.Transparent;
            AddChild(label);
            SizeToFit();
        }

        public void SizeToFit()
        {
            label.SizeToFit();

            if (Frame.Width < label.Frame.Width)
            {
                Frame = new Rectangle(Frame.X, Frame.Y, label.Frame.Width, Frame.Height);
            }
            if (Frame.Height < label.Frame.Height)
            {
                Frame = new Rectangle(Frame.X, Frame.Y, Frame.Width, label.Frame.Height);
            }
            var x = alignment.HorizontalPositionIn(Frame.Width, label.Frame.Size.X);
            label.Frame = new Rectangle(x, 0, label.Frame.Width, label.Frame.Height);
            label.CenterYInParent();
        }
    }
}
