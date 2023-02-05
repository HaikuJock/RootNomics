using System;

namespace RootNomics.Environment
{
    class RandomNum
    {
        static int randomSeed = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        static Random random = new Random(randomSeed);
        public static int GetRandomInt(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}
