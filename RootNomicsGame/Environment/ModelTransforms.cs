using Microsoft.Xna.Framework;

namespace RootNomics.Environment
{
    class ModelTransforms
    {

        public static Matrix Translation(float tX, float tY, float tZ)
        {
            return Matrix.CreateTranslation(new Vector3(tX, tY, tZ));
        }


    }
}



