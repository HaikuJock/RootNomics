namespace TexturePackerLoader
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class SpriteFrame
    {
        public SpriteFrame(Texture2D texture, Rectangle sourceRect, Vector2 size, Vector2 pivotPoint, bool isRotated)
        {
            Texture = texture;
            SourceRectangle = sourceRect;
            Size = size;
            Origin = isRotated ? new Vector2(sourceRect.Width * (1 - pivotPoint.Y), sourceRect.Height * pivotPoint.X)
                               : new Vector2(sourceRect.Width * pivotPoint.X, sourceRect.Height * pivotPoint.Y);
            IsRotated = isRotated;
        }

        public Texture2D Texture { get; }

        public Rectangle SourceRectangle { get; }

        public Vector2 Size { get; }

        public bool IsRotated { get; }

        public Vector2 Origin { get; }

        public virtual void Draw(SpriteBatch spriteBatch, Rectangle destination, Color color, float rotation, Vector2 origin)
        {
            spriteBatch.Draw(Texture, destination, SourceRectangle, color, rotation, origin, SpriteEffects.None, 0f);
        }
    }
}
