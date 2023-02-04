using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TexturePackerLoader
{
    public class DecorationNinePatch : SpriteFrame
    {
        readonly Rectangle tl;
        readonly Rectangle top1;
        readonly Rectangle decorTop;
        readonly Rectangle top2;
        readonly Rectangle tr;
        readonly Rectangle right;
        readonly Rectangle br;
        readonly Rectangle bottom1;
        readonly Rectangle decorBottom;
        readonly Rectangle bottom2;
        readonly Rectangle bl;
        readonly Rectangle left;
        readonly Rectangle centre;

        public DecorationNinePatch(SpriteFrame spriteFrame, int decorationWidth)
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
                  spriteFrame.IsRotated,
                  decorationWidth)
        {
        }

        public DecorationNinePatch(
            Texture2D texture, 
            Rectangle sourceRect, 
            Vector2 size, 
            Vector2 pivotPoint, 
            bool isRotated,
            int decorationWidth)
            : base(texture, sourceRect, size, pivotPoint, isRotated)
        {
            var cornerWidth = (sourceRect.Width - 2 - decorationWidth) / 2;
            var cornerHeight = (sourceRect.Height - 1) / 2;
            var s = sourceRect;
            tl = new Rectangle(s.X, s.Y, cornerWidth, cornerHeight);
            top1 = new Rectangle(s.X + cornerWidth, s.Y, 1, cornerHeight);
            decorTop = new Rectangle(top1.Right, s.Y, decorationWidth, cornerHeight);
            top2 = new Rectangle(decorTop.Right, s.Y, 1, cornerHeight);
            tr = new Rectangle(top2.Right, s.Y, cornerWidth, cornerHeight);
            right = new Rectangle(tr.X, s.Y + cornerHeight, cornerWidth, 1);
            br = new Rectangle(tr.X, s.Y + cornerHeight + 1, cornerWidth, cornerHeight);
            bottom1 = new Rectangle(s.X + cornerWidth, s.Y + cornerHeight + 1, 1, cornerHeight);
            decorBottom = new Rectangle(top1.Right, s.Y + cornerHeight + 1, decorationWidth, cornerHeight);
            bottom2 = new Rectangle(decorBottom.Right, s.Y + cornerHeight + 1, 1, cornerHeight);
            bl = new Rectangle(s.X, s.Y + cornerHeight + 1, cornerWidth, cornerHeight);
            left = new Rectangle(s.X, s.Y + cornerHeight, cornerWidth, 1);
            centre = new Rectangle(s.X + cornerWidth, s.Y + cornerHeight, 1, 1);
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color, float rotation, Vector2 origin)
        {
            Rectangle d = destination;
            Rectangle dtl = new Rectangle(d.X, d.Y, tl.Width, tl.Height);
            Rectangle dtop1 = new Rectangle(d.X + tl.Width, d.Y, (d.Width - tl.Width - tr.Width - decorTop.Width) / 2, top1.Height);
            Rectangle ddecorTop = new Rectangle(dtop1.Right, d.Y, decorTop.Width, top1.Height);
            Rectangle dtop2 = new Rectangle(ddecorTop.Right, d.Y, d.Width - tl.Width - tr.Width - decorTop.Width - dtop1.Width, top1.Height);
            Rectangle dtr = new Rectangle(d.X + d.Width - tr.Width, d.Y, tr.Width, tr.Height);
            Rectangle dright = new Rectangle(d.X + d.Width - right.Width, d.Y + tr.Height, right.Width, d.Height - tr.Height - br.Height);
            Rectangle dbr = new Rectangle(d.X + d.Width - br.Width, d.Y + d.Height - br.Height, br.Width, br.Height);
            Rectangle dbottom1 = new Rectangle(d.X + bl.Width, d.Y + d.Height - bottom1.Height, (d.Width - bl.Width - br.Width - decorBottom.Width) / 2, bottom1.Height);
            Rectangle ddecorBottom = new Rectangle(dbottom1.Right, dbottom1.Y, decorBottom.Width, bottom1.Height);
            Rectangle dbottom2 = new Rectangle(ddecorBottom.Right, dbottom1.Y, d.Width - bl.Width - br.Width - decorBottom.Width - dbottom1.Width, dbottom1.Height);
            Rectangle dbl = new Rectangle(d.X, d.Y + d.Height - bl.Height, bl.Width, bl.Height);
            Rectangle dleft = new Rectangle(d.X, d.Y + tl.Height, left.Width, d.Height - tl.Height - bl.Height);
            Rectangle dcentre = new Rectangle(d.X + tl.Width, d.Y + tl.Height, d.Width - tl.Width - tr.Width, d.Height - tl.Height - bl.Height);

            spriteBatch.Draw(Texture, dright, right, color);
            spriteBatch.Draw(Texture, dbr, br, color);
            spriteBatch.Draw(Texture, dbottom1, bottom1, color);
            spriteBatch.Draw(Texture, ddecorBottom, decorBottom, color);
            spriteBatch.Draw(Texture, dbottom2, bottom2, color);
            spriteBatch.Draw(Texture, dbl, bl, color);
            spriteBatch.Draw(Texture, dleft, left, color);
            spriteBatch.Draw(Texture, dcentre, centre, color);
            spriteBatch.Draw(Texture, dtl, tl, color);
            spriteBatch.Draw(Texture, dtop1, top1, color);
            spriteBatch.Draw(Texture, ddecorTop, decorTop, color);
            spriteBatch.Draw(Texture, dtop2, top2, color);
            spriteBatch.Draw(Texture, dtr, tr, color);
        }
    }
}
