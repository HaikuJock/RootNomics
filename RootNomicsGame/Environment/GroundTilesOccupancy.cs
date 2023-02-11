using System.Collections.Generic;

namespace RootNomics.Environment
{
    class GroundTilesOccupancy
    {
        // the central square is (10,10)
        // the corners are (-11,-11) to (11,11)
        public const int BOARD_WIDTH = 21;
        public const int BOARD_HEIGHT = 21;
        int xOffsetToCentralTile;
        int yOffsetToCentralTile;

        // there are 441 tiles
        List<int> freeTiles = new();

        List<int> occupiedTiles = new();

        List<(int delay, int ordinal)> tilesRecentlyReleased = new();

        public GroundTilesOccupancy()
        {
            int nrTiles = BOARD_HEIGHT * BOARD_WIDTH;
            xOffsetToCentralTile = BOARD_HEIGHT / 2;
            yOffsetToCentralTile = BOARD_HEIGHT / 2;

            List<int> unassignedTiles = new();
            for (int i = 0; i < nrTiles; i++)
            {
                unassignedTiles.Add(i);
            }
            // Randomize the tiles at start
            for (int i = 0; i < 441; i++)
            {
                int randomIndex = RandomNum.GetRandomInt(0, 441 - i);
                int randomTile = unassignedTiles[randomIndex];
                unassignedTiles.RemoveAt(randomIndex);
                freeTiles.Add(randomTile);
            }
        }

        public (int x, int y) RequestFreeTile()
        {
            int freeTile = freeTiles[0];
            occupiedTiles.Add(freeTile);
            freeTiles.RemoveAt(0);
            int x = freeTile / BOARD_HEIGHT;
            int y = freeTile % BOARD_WIDTH;
            return (x - xOffsetToCentralTile, y - yOffsetToCentralTile);
        }

        const int DELAY_TO_RELEASE_TILE = 100;

        public void RegisterFreedTile(int x, int y)
        {
            int ordinal = 21 * (x + xOffsetToCentralTile) + (y + yOffsetToCentralTile);
            tilesRecentlyReleased.Add((DELAY_TO_RELEASE_TILE, ordinal));
        }

        public void Update()
        {
            List<(int delay, int ordinal)> updatedList = new();

            for (int i = 0; i < tilesRecentlyReleased.Count; i++)
            {
                (int delay, int ordinal) tileInfo = tilesRecentlyReleased[i];
                tileInfo.delay--;
                if (tileInfo.delay == 0 && freeTiles.Count > 0)
                {
                    int randomIndex = RandomNum.GetRandomInt(0, freeTiles.Count - 1);
                    freeTiles.Insert(randomIndex, tileInfo.ordinal);
                }
                else
                {
                    updatedList.Add((tileInfo.delay, tileInfo.ordinal));
                }
            }
            tilesRecentlyReleased = updatedList;
        }
    }
}








