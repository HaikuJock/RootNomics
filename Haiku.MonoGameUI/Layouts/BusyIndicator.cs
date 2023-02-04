using Microsoft.Xna.Framework;
using System;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class BusyIndicator : ImageControl
    {
        public const int LargeSize = 128;

        public BusyIndicator(int size, SpriteFrame sprite)
            : this(new Rectangle(0, 0, size, size), sprite)
        {
        }

        public BusyIndicator(Rectangle frame, SpriteFrame sprite) 
            : base(frame, ContentAlignment.Centre)
        {
            Texture = sprite;
            Origin = new Vector2(sprite.SourceRectangle.Width / 2, sprite.SourceRectangle.Height / 2);
            Fit = ContentFit.Destination;
        }

        public void StartAnimating()
        {
            var rotation = Animate().Rotation((float)(Math.PI * 2)).Over(1).RepeatForever();
            ParentWindow?.Add(rotation);
        }
    }
}
