using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RootNomics.Environment
{
    class CommonBasicEffects
    {

        public static void SetEffects(BasicEffect basicEffect)
        {
            basicEffect.EnableDefaultLighting();
            basicEffect.AmbientLightColor = new Vector3(0.4f, 0.3f, 0.3f);
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = new Vector3(0.8f, 0.8f, -1);
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.8f, 0.8f, 0.8f);
        }

        public static void SetEffectsDarker(BasicEffect basicEffect)
        {
            basicEffect.EnableDefaultLighting();
            basicEffect.AmbientLightColor = new Vector3(0.4f, 0.3f, 0.3f);
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = new Vector3(0.8f, 0.8f, -1);
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.65f, 0.65f, 0.5f);
        }
    }
}
