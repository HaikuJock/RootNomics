using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TexturePackerLoader
{
    public class TiledNinePatch : SpriteFrame
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
        readonly int noise;
        readonly bool randomEdges;

        public TiledNinePatch(SpriteFrame spriteFrame, Point[] partSizes, bool randomEdges)
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
                  partSizes,
                  randomEdges)
        {
        }

        public TiledNinePatch(
            Texture2D texture, 
            Rectangle sourceRect, 
            Vector2 size, 
            Vector2 pivotPoint, 
            bool isRotated, 
            Point[] partSizes,
            bool randomEdges)
            : base(texture, sourceRect, size, pivotPoint, isRotated)
        {
            this.randomEdges = randomEdges;
            var s = sourceRect;
            tl = new Rectangle(s.X, s.Y, partSizes[0].X, partSizes[0].Y);
            top = new Rectangle(s.X + partSizes[0].X, s.Y, partSizes[1].X, partSizes[1].Y);
            tr = new Rectangle(s.X + s.Width - partSizes[2].X, s.Y, partSizes[2].X, partSizes[2].Y);
            right = new Rectangle(s.X + s.Width - partSizes[3].X, s.Y + partSizes[2].Y, partSizes[3].X, partSizes[3].Y);
            br = new Rectangle(s.X + s.Width - partSizes[4].X, s.Y + s.Height - partSizes[4].Y, partSizes[4].X, partSizes[4].Y);
            bottom = new Rectangle(s.X + partSizes[6].X, s.Y + s.Height - partSizes[5].Y, partSizes[5].X, partSizes[5].Y);
            bl = new Rectangle(s.X, s.Y + s.Height - partSizes[6].Y, partSizes[6].X, partSizes[6].Y);
            left = new Rectangle(s.X, s.Y + partSizes[0].Y, partSizes[7].X, partSizes[7].Y);
            centre = new Rectangle(s.X + partSizes[0].X, s.Y + partSizes[0].Y, partSizes[8].X, partSizes[8].Y);
            noise = (int)DateTime.Now.Ticks;
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color, float rotation, Vector2 origin)
        {
            var width = ((destination.Width - tl.Width - tr.Width) / top.Width) * top.Width + tl.Width + tr.Width;
            var xOffset = (destination.Width - width) / 2;
            var height = ((destination.Height - tl.Height - bl.Height) / left.Height) * left.Height + tl.Height + bl.Height;
            var yOffset = (destination.Height - height) / 2;
            Rectangle d = new Rectangle(destination.X + xOffset, destination.Y + yOffset, width, height);
            Rectangle dtl = new Rectangle(d.X, d.Y, tl.Width, tl.Height);
            Rectangle dtop = new Rectangle(d.X + tl.Width, d.Y, d.Width - tl.Width - tr.Width, top.Height);
            Rectangle dtr = new Rectangle(d.X + d.Width - tr.Width, d.Y, tr.Width, tr.Height);
            Rectangle dright = new Rectangle(d.X + d.Width - right.Width, d.Y + tr.Height, right.Width, d.Height - tr.Height - br.Height);
            Rectangle dbr = new Rectangle(d.X + d.Width - br.Width, d.Y + d.Height - br.Height, br.Width, br.Height);
            Rectangle dbottom = new Rectangle(d.X + bl.Width, d.Y + d.Height - bottom.Height, d.Width - bl.Width - br.Width, bottom.Height);
            Rectangle dbl = new Rectangle(d.X, d.Y + d.Height - bl.Height, bl.Width, bl.Height);
            Rectangle dleft = new Rectangle(d.X, d.Y + tl.Height, left.Width, d.Height - tl.Height - bl.Height);
            Rectangle dcentre = new Rectangle(d.X + tl.Width, d.Y + tl.Height, d.Width - tl.Width - tr.Width, d.Height - tl.Height - bl.Height);
            var horizontalCount = dtop.Width / top.Width;
            var verticalCount = dleft.Height / left.Height;
            var rtop = new Rectangle(dtop.X, dtop.Y, top.Width, top.Height);
            var rbottom = new Rectangle(dbottom.X, dbottom.Y, bottom.Width, bottom.Height);
            var rright = new Rectangle(dright.X, dright.Y, right.Width, right.Height);
            var rleft = new Rectangle(dleft.X, dleft.Y, left.Width, left.Height);
            var rcentre = new Rectangle(dcentre.X, dcentre.Y, centre.Width, centre.Height);

            if (randomEdges)
            {
                for (int i = 0; i < verticalCount; i++)
                {
                    SpriteEffects effects = (SpriteEffects)((noise >> i) & 0x00000002);
                    spriteBatch.Draw(Texture, rright, right, color, 0f, Vector2.Zero, effects, 0f);
                    effects = (SpriteEffects)((~noise >> i) & 0x00000002);
                    spriteBatch.Draw(Texture, rleft, left, color, 0f, Vector2.Zero, effects, 0f);
                    rright.Offset(0, right.Height);
                    rleft.Offset(0, left.Height);
                }
            }
            else
            {
                for (int i = 0; i < verticalCount; i++)
                {
                    spriteBatch.Draw(Texture, rright, right, color);
                    spriteBatch.Draw(Texture, rleft, left, color);
                    rright.Offset(0, right.Height);
                    rleft.Offset(0, left.Height);
                }
            }
            spriteBatch.Draw(Texture, dbr, br, color);
            if (randomEdges)
            {
                for (int i = 0; i < horizontalCount; i++)
                {
                    SpriteEffects effects = (SpriteEffects)((noise >> i) & 0x00000001);
                    spriteBatch.Draw(Texture, rtop, top, color, 0f, Vector2.Zero, effects, 0f);
                    effects = (SpriteEffects)((~noise >> i) & 0x00000001);
                    spriteBatch.Draw(Texture, rbottom, bottom, color, 0f, Vector2.Zero, effects, 0f);
                    rtop.Offset(top.Width, 0);
                    rbottom.Offset(bottom.Width, 0);
                }
            }
            else
            {
                for (int i = 0; i < horizontalCount; i++)
                {
                    spriteBatch.Draw(Texture, rtop, top, color);
                    spriteBatch.Draw(Texture, rbottom, bottom, color);
                    rtop.Offset(top.Width, 0);
                    rbottom.Offset(bottom.Width, 0);
                }
            }

            spriteBatch.Draw(Texture, dbl, bl, color);
            for (int y = 0; y < verticalCount; y++)
            {
                for (int x = 0; x < horizontalCount; x++)
                {
                    SpriteEffects effects = (SpriteEffects)(((noise >> (x ^ y))) & 0x00000003);
                    spriteBatch.Draw(Texture, rcentre, centre, color, 0f, Vector2.Zero, effects, 0f);
                    rcentre.Offset(centre.Width, 0);
                }
                rcentre.Offset(-horizontalCount * centre.Width, centre.Height);
            }
            spriteBatch.Draw(Texture, dtl, tl, color);
            spriteBatch.Draw(Texture, dtr, tr, color);
        }
    }
}
