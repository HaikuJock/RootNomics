namespace TexturePackerLoader
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    public class SpriteRender
    {
        private const float ClockwiseNinetyDegreeRotation = (float)(Math.PI / 2.0f);
        readonly private SpriteBatch spriteBatch;

        public SpriteRender (SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        // <param name="position">This should be where you want the pivot point of the sprite image to be rendered.</param>
        public void Draw(SpriteFrame sprite, Vector2 position, Color? color = null, float rotation = 0, float scale = 1, SpriteEffects spriteEffects = SpriteEffects.None)
        {
            Vector2 origin = sprite.Origin;
            if (sprite.IsRotated)
            {
                rotation -= ClockwiseNinetyDegreeRotation;
                switch (spriteEffects)
                {
                    case SpriteEffects.FlipHorizontally: spriteEffects = SpriteEffects.FlipVertically; break;
                    case SpriteEffects.FlipVertically: spriteEffects = SpriteEffects.FlipHorizontally; break;
                }
            }
            switch (spriteEffects)
            {
                case SpriteEffects.FlipHorizontally: origin.X = sprite.SourceRectangle.Width - origin.X; break;
                case SpriteEffects.FlipVertically: origin.Y = sprite.SourceRectangle.Height - origin.Y; break;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            this.spriteBatch.Draw(
                texture: sprite.Texture,
                position: position,
                sourceRectangle: sprite.SourceRectangle,
                color: (Color)(color == null ? Color.White : color),
                rotation: rotation,
                origin: origin,
                scale: new Vector2(scale, scale),
                effects: spriteEffects,
                layerDepth: 0);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}