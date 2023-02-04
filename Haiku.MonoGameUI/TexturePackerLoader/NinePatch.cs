using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TexturePackerLoader
{
    public class NinePatch : SpriteFrame
    {
        readonly Rectangle tl;
        readonly Rectangle top;
        readonly Rectangle tr;
        readonly Rectangle right;
        readonly Rectangle br;
        readonly Rectangle bottom;
        readonly Rectangle bl;
        readonly Rectangle left;
        readonly Rectangle centre;

        public NinePatch(SpriteFrame spriteFrame)
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

        public NinePatch(Texture2D texture, Rectangle sourceRect, Vector2 size, Vector2 pivotPoint, bool isRotated) 
            : base(texture, sourceRect, size, pivotPoint, isRotated)
        {
            var cornerWidth = (sourceRect.Width - 1) / 2;
            var cornerHeight = (sourceRect.Height - 1) / 2;
            var s = sourceRect;
            tl = new Rectangle(s.X, s.Y, cornerWidth, cornerHeight);
            top = new Rectangle(s.X + cornerWidth, s.Y, 1, cornerHeight);
            tr = new Rectangle(s.X + cornerWidth + 1, s.Y, cornerWidth, cornerHeight);
            right = new Rectangle(s.X + cornerWidth + 1, s.Y + cornerHeight, cornerWidth, 1);
            br = new Rectangle(s.X + cornerWidth + 1, s.Y + cornerHeight + 1, cornerWidth, cornerHeight);
            bottom = new Rectangle(s.X + cornerWidth, s.Y + cornerHeight + 1, 1, cornerHeight);
            bl = new Rectangle(s.X, s.Y + cornerHeight + 1, cornerWidth, cornerHeight);
            left = new Rectangle(s.X, s.Y + cornerHeight, cornerWidth, 1);
            centre = new Rectangle(s.X + cornerWidth, s.Y + cornerHeight, 1, 1);
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color, float rotation, Vector2 origin)
        {
            Rectangle d = destination;
            Rectangle dtl = new Rectangle(d.X, d.Y, tl.Width, tl.Height);
            Rectangle dtop = new Rectangle(d.X + tl.Width, d.Y, d.Width - tl.Width - tr.Width, top.Height);
            Rectangle dtr = new Rectangle(d.X + d.Width - tr.Width, d.Y, tr.Width, tr.Height);
            Rectangle dright = new Rectangle(d.X + d.Width - right.Width, d.Y + tr.Height, right.Width, d.Height - tr.Height - br.Height);
            Rectangle dbr = new Rectangle(d.X + d.Width - br.Width, d.Y + d.Height - br.Height, br.Width, br.Height);
            Rectangle dbottom = new Rectangle(d.X + bl.Width, d.Y + d.Height - bottom.Height, d.Width - bl.Width - br.Width, bottom.Height);
            Rectangle dbl = new Rectangle(d.X, d.Y + d.Height - bl.Height, bl.Width, bl.Height);
            Rectangle dleft = new Rectangle(d.X, d.Y + tl.Height, left.Width, d.Height - tl.Height - bl.Height);
            Rectangle dcentre = new Rectangle(d.X + tl.Width, d.Y + tl.Height, d.Width - tl.Width - tr.Width, d.Height - tl.Height - bl.Height);

            spriteBatch.Draw(Texture, dright, right, color);
            spriteBatch.Draw(Texture, dbr, br, color);
            spriteBatch.Draw(Texture, dbottom, bottom, color);
            spriteBatch.Draw(Texture, dbl, bl, color);
            spriteBatch.Draw(Texture, dleft, left, color);
            spriteBatch.Draw(Texture, dcentre, centre, color);
            spriteBatch.Draw(Texture, dtl, tl, color);
            spriteBatch.Draw(Texture, dtop, top, color);
            spriteBatch.Draw(Texture, dtr, tr, color);
        }
    }
}
