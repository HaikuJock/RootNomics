using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TexturePackerLoader
{
    public class VerticalPatch : SpriteFrame
    {
        Rectangle top;
        Rectangle bottom;
        Rectangle centre;

        public VerticalPatch(SpriteFrame spriteFrame)
            : this(
                  spriteFrame.Texture,
                  spriteFrame.SourceRectangle,
                  spriteFrame.Size,
                  spriteFrame.IsRotated
                  ? new Vector2(
                      (spriteFrame.SourceRectangle.Height - spriteFrame.Origin.Y) / (float)spriteFrame.SourceRectangle.Width,
                      spriteFrame.Origin.X / (float)spriteFrame.SourceRectangle.Height)
                  : new Vector2(
                      spriteFrame.Origin.X / spriteFrame.SourceRectangle.Width,
                      spriteFrame.Origin.Y / spriteFrame.SourceRectangle.Height),
                  spriteFrame.IsRotated)
        {
        }

        public VerticalPatch(Texture2D texture, Rectangle sourceRect, Vector2 size, Vector2 pivotPoint, bool isRotated)
            : base(texture, sourceRect, size, pivotPoint, isRotated)
        {
            var edgeSize = (sourceRect.Height - 1) / 2;
            var s = sourceRect;
            top = new Rectangle(s.X, s.Y, s.Width, edgeSize);
            bottom = new Rectangle(s.X, s.Y + edgeSize + 1, s.Width, edgeSize);
            centre = new Rectangle(s.X, s.Y + edgeSize, s.Width, 1);
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color, float rotation, Vector2 origin)
        {
            Rectangle d = destination;
            Rectangle dtop = new Rectangle(d.X, d.Y, d.Width, top.Height);
            Rectangle dbottom = new Rectangle(d.X, d.Y + d.Height - bottom.Height, d.Width, bottom.Height);
            Rectangle dcentre = new Rectangle(d.X, d.Y + top.Height, d.Width, d.Height - top.Height - bottom.Height);

            spriteBatch.Draw(Texture, dbottom, bottom, color);
            spriteBatch.Draw(Texture, dcentre, centre, color);
            spriteBatch.Draw(Texture, dtop, top, color);
        }
    }
}
