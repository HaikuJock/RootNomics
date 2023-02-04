using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class Panel : Layout
    {
        public static GraphicsDevice Graphics;

        static readonly Dictionary<Color, SpriteFrame> coloredTextureCache = new Dictionary<Color, SpriteFrame>();
        public static SpriteFrame ColorSprite(Color color)
        {
            if (!coloredTextureCache.TryGetValue(color, out SpriteFrame result))
            {
                var colorTexture = new Texture2D(Graphics, 1, 1);
                colorTexture.SetData(new Color[] { color });
                result = new SpriteFrame(colorTexture, new Rectangle(0, 0, 1, 1), Vector2.One, new Vector2(0.5f, 0.5f), false);
                coloredTextureCache[color] = result;
            }

            return result;
        }

        public SpriteFrame Texture;
        public Vector2 Origin = Vector2.Zero;
        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                Texture = ColorSprite(backgroundColor);
            }
        }
        Color backgroundColor;
        public Color Tint;

        public Panel()
            : this(0, 0, 0, 0)
        {
        }

        public Panel(float x, float y, float width, float height)
            : this((int)x, (int)y, (int)width, (int)height)
        {
        }

        public Panel(int size)
            : this(size, size)
        {
        }

        public Panel(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public Panel(int x, int y, int width, int height)
            : this(new Rectangle(x, y, width, height))
        {
        }

        public Panel(Rectangle frame)
            : this(frame, new FrameLayoutStrategy())
        {
        }

        public Panel(Rectangle frame, LayoutStrategy layoutStrategy)
            : base(frame, layoutStrategy)
        {
            BackgroundColor = Color.White;
            Tint = Color.White;
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle container, float alpha)
        {
            if (!IsVisible)
            {
                return;
            }
            Rectangle destination = WorldFrame(container);

            if (destination.Intersects(container))
            {
                DrawContent(spriteBatch, Texture, destination, Tint, alpha);
                base.Draw(spriteBatch, container, alpha);
            }
        }

        protected virtual void DrawContent(
            SpriteBatch spriteBatch, 
            SpriteFrame texture, 
            Rectangle destination, 
            Color clr,
            float alpha)
        {
            texture.Draw(spriteBatch, destination, clr * alpha, LocalRotation, Origin);
        }

        protected override bool OnCursorMove(Point point, Rectangle container)
        {
            return IsVisible && BackgroundColor.A > 0;
        }

        protected override bool OnPress(Point point, Rectangle container)
        {
            return IsVisible && BackgroundColor.A > 0;
        }

        protected override bool OnRelease(Point point, Rectangle container)
        {
            return IsVisible && BackgroundColor.A > 0;
        }

        protected override bool OnAltPress(Point point, Rectangle container)
        {
            return IsVisible && BackgroundColor.A > 0;
        }

        protected override bool OnAltRelease(Point point, Rectangle container)
        {
            return IsVisible && BackgroundColor.A > 0;
        }
    }
}
