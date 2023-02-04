using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class ImageControl : Control
    {
        readonly ContentAlignment alignment;
        public ContentFit Fit;

        public ImageControl(int size)
            : this(size, size)
        {
        }

        public ImageControl(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public ImageControl(int x, int y, int width, int height)
            : this(new Rectangle(x, y, width, height), ContentAlignment.Centre)
        {
        }

        public ImageControl(Rectangle frame, ContentAlignment alignment)
            : base(frame, new FrameLayoutStrategy())
        {
            this.alignment = alignment;
        }

        protected override void DrawContent(
            SpriteBatch spriteBatch,
            SpriteFrame texture,
            Rectangle destination,
            Color clr,
            float alpha)
        {
            var width = Fit == ContentFit.Source ? texture.SourceRectangle.Width : Frame.Width;
            var height = Fit == ContentFit.Source ? texture.SourceRectangle.Height : Frame.Height;

            destination = new Rectangle(
                destination.X + alignment.HorizontalPositionIn(Frame.Width, width),
                destination.Y + Frame.Height / 2 - height / 2,
                width,
                height);
            texture.Draw(spriteBatch, destination, clr * alpha, LocalRotation, Origin);
        }

        protected override bool OnCursorMove(Point point, Rectangle container)
        {
            return base.OnCursorMove(point, container) || Texture != null;
        }

        protected override bool OnPress(Point point, Rectangle container)
        {
            return base.OnPress(point, container) || Texture != null;
        }

        protected override bool OnRelease(Point point, Rectangle container)
        {
            return base.OnRelease(point, container) || Texture != null;
        }

        protected override bool OnAltPress(Point point, Rectangle container)
        {
            return base.OnAltPress(point, container) || Texture != null;
        }

        protected override bool OnAltRelease(Point point, Rectangle container)
        {
            return base.OnAltRelease(point, container) || Texture != null;
        }
    }
}
