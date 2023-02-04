using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class Label : Panel
    {
        public string Text;
        public SpriteFont Font;
        public Color TextColor;
        public Point ShadowOffset;
        public Color ShadowColor;

        public Label()
            : this(Rectangle.Empty)
        {
        }

        public Label(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public Label(int x, int y, int width, int height)
            : this(new Rectangle(x, y, width, height))
        {
        }

        public Label(string text)
            : this(new Rectangle(Point.Zero, CalculateSize(text, ItemFont)), text)
        {
        }

        public Label(Rectangle frame)
            : this(frame, "")
        {
        }

        public Label(string text, SpriteFont font)
            : this(new Rectangle(Point.Zero, CalculateSize(text, font)), text, font)
        {
        }

        public Label(Rectangle frame, string text)
            : this(frame, text, ItemFont)
        {
        }

        public Label(Rectangle frame, string text, SpriteFont font)
            : base(frame)
        {
            TextColor = Color.Black;
            Font = font;
            Text = text;
            BackgroundColor = Color.Transparent;
            //var glyphs = font.GetGlyphs();
            //var glyphX = glyphs['X'];
            //var baseline = glyphX.BoundsInTexture.Height;
        }

        public void SetTextLayedOut(string text)
        {
            Text = text;
            SizeToFit();
            Parent?.DoLayout();
        }

        public void SizeToFit()
        {
            Point size = CalculateSize();
            Frame = new Rectangle(Frame.Location, size);
        }

        protected override void DrawContent(
            SpriteBatch spriteBatch,
            SpriteFrame texture,
            Rectangle destination,
            Color clr,
            float alpha)
        {
            base.DrawContent(spriteBatch, texture, destination, clr, alpha);
            if (!string.IsNullOrEmpty(Text)
                && Font != null)
            {
                if (ShadowOffset != Point.Zero)
                {
                    var shadowPosition = new Vector2(destination.X + ShadowOffset.X, destination.Y + ShadowOffset.Y);
                    spriteBatch.DrawString(Font, Text, shadowPosition, ShadowColor * Alpha * alpha);
                }
                spriteBatch.DrawString(Font, Text, new Vector2(destination.X, destination.Y), TextColor * Alpha * alpha);
            }
        }

        Point CalculateSize()
        {
            return CalculateSize(Text, Font);
        }

        static Point CalculateSize(string text, SpriteFont font)
        {
            if (string.IsNullOrEmpty(text)
                || font == null)
            {
                return Point.Zero;
            }
            else
            {
                Vector2 size = font.MeasureString(text);
                return new Point((int)(size.X + 0.5f), (int)(size.Y + 0.5f));
            }
        }
    }
}
