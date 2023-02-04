namespace TexturePackerLoader
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;

    public class SpriteSheet
    {
        private readonly IDictionary<string, SpriteFrame> spriteList;
        private readonly string fallback;

        public static int SizeForTiled(int min, int corner, int centre)
        {
            var centreCount = (min - corner * 2) / centre;
            var size = centreCount * centre + (corner * 2);

            while (size < min)
            {
                ++centreCount;
                size = centreCount * centre + (corner * 2);
            }

            return size;
        }

        public static int SizeForObliqueTiled(int min, int corner, int centre)
        {
            var centreCount = (min - corner * 2 - 1) / centre;
            var size = centreCount * centre + (corner * 2 - 1);

            while (size < min)
            {
                ++centreCount;
                size = centreCount * centre + (corner * 2 - 1);
            }

            return size;
        }

        public SpriteSheet()
            : this("")
        {
        }

        public SpriteSheet(string fallback)
        {
            spriteList = new Dictionary<string, SpriteFrame>();
            this.fallback = fallback;
        }

        public void Add(string name, SpriteFrame sprite)
        {
            spriteList.Add(name, sprite);
        }

        public void Add(SpriteSheet otherSheet)
        {
            foreach (var sprite in otherSheet.spriteList)
            {
                spriteList.Add(sprite);
            }
        }

        public SpriteFrame Sprite(string spriteName)
        {
            if (spriteList.TryGetValue(spriteName, out SpriteFrame sprite))
            {
                return sprite;
            }
            else
            {
                return spriteList[fallback];
            }
        }

        public SpriteFrame TiledObliqueNinePatch(string sprite, int cornerSize, int centreSize)
        {
            return new TiledNinePatch(
                spriteList[sprite],
                new Point[] {
                    new Point(cornerSize, cornerSize),  // tl
                    new Point(centreSize, cornerSize),  // top
                    new Point(cornerSize - 1, cornerSize),  // tr
                    new Point(cornerSize - 1, centreSize),  // right
                    new Point(cornerSize - 1, cornerSize - 1),  // br
                    new Point(centreSize, cornerSize - 1),  // bottom
                    new Point(cornerSize, cornerSize - 1),  // bl
                    new Point(cornerSize, centreSize),  // left
                    new Point(centreSize, centreSize),  // centre
                },
                randomEdges: false);
        }

        public SpriteFrame TiledNinePatch(string sprite, int cornerSize, int centreSize, bool randomEdges)
        {
            return new TiledNinePatch(
                spriteList[sprite],
                new Point[] {
                    new Point(cornerSize, cornerSize),  // tl
                    new Point(centreSize, cornerSize),  // top
                    new Point(cornerSize, cornerSize),  // tr
                    new Point(cornerSize, centreSize),  // right
                    new Point(cornerSize, cornerSize),  // br
                    new Point(centreSize, cornerSize),  // bottom
                    new Point(cornerSize, cornerSize),  // bl
                    new Point(cornerSize, centreSize),  // left
                    new Point(centreSize, centreSize),  // centre
                },
                randomEdges);
        }

        public SpriteFrame TiledNinePatch(string sprite, int tileSize, bool randomEdges)
        {
            return new TiledNinePatch(
                spriteList[sprite], 
                new Point[] {
                    new Point(tileSize, tileSize),
                    new Point(tileSize, tileSize),
                    new Point(tileSize, tileSize),
                    new Point(tileSize, tileSize),
                    new Point(tileSize, tileSize),
                    new Point(tileSize, tileSize),
                    new Point(tileSize, tileSize),
                    new Point(tileSize, tileSize),
                    new Point(tileSize, tileSize),
                },
                randomEdges);
        }

        public SpriteFrame TiledNinePatch(string sprite, Point[] tileSizes, bool randomEdges)
        {
            return new TiledNinePatch(spriteList[sprite], tileSizes, randomEdges);
        }

        public NinePatch NinePatch(string sprite)
        {
            return new NinePatch(spriteList[sprite]);
        }

        public VerticalPatch VerticalPatch(string sprite)
        {
            return new VerticalPatch(spriteList[sprite]);
        }

        public SpriteFrame DecorationNinePatch(string sprite, int decorationWidth)
        {
            return new DecorationNinePatch(spriteList[sprite], decorationWidth);
        }
    }
}
