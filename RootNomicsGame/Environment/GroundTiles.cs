using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RootNomics.Environment
{
    class GroundTiles
    {
        private Matrix[,] transforms;
        private Vector3[,,] colors;
        public float[,] tileHeights;

        private Model wedge0;
        private Model wedge1;
        private int gridSize = 21;
        private int offset;

        public GroundTiles(Model wedge0, Model wedge1)
        {
            this.wedge0 = wedge0;
            this.wedge1 = wedge1;
            this.offset = gridSize / 2;
            transforms = new Matrix[gridSize, gridSize];
            tileHeights = new float[gridSize, gridSize];
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    int randomInt = RandomNum.GetRandomInt(0, 1000);
                    tileHeights[i,j] = (float) randomInt * 0.10f / 1000;
                    if (tileHeights[i, j] < 0.01f)
                    {
                        tileHeights[i, j] = 0.01f;
                    }
                    Matrix S = Matrix.CreateScale(1, 1, tileHeights[i, j]);
                    bool rotateTile = randomInt % 2 == 1;
                    float rotateBy = rotateTile ? MathF.PI / 2 : 0;
                    Matrix R = Matrix.CreateRotationZ(rotateBy);
                    Matrix T = Matrix.CreateTranslation(i - offset, j - offset, 0);

                    Matrix transform = Matrix.Multiply(S,R);
                    transform = Matrix.Multiply(transform,T);
                    transforms[i, j] = transform;
                }
            }

            colors = new Vector3[gridSize, gridSize, 2];
            ColorSampler colorSampler1 = new ColorSampler(0x008C00);
            ColorSampler colorSampler2 = new ColorSampler(0x008300);

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    colors[i, j, 0] = colorSampler1.GetVariationVector3();
                    colors[i, j, 1] = colorSampler2.GetVariationVector3();
                    // colors[i, j, 1] = colors[i, j, 0];
                }
            }
            Debug.WriteLine($"done");
        }


        public float GetTileHeight(int x, int y)
        {
            return tileHeights[x, y];
        }


        public void DrawGroundTiles(CameraTransforms cameraTransforms)
        {
            foreach (ModelMesh mesh in wedge0.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    for (int i = 0; i < transforms.GetLength(0); i++)
                    {
                        for (int j = 0; j < transforms.GetLength(1); j++)
                        {
                            BasicEffect basicEffect = (BasicEffect) effect;
                            CommonBasicEffects.SetEffectsDarker(basicEffect);
                            basicEffect.DiffuseColor = new Vector3(colors[i, j, 0].X, colors[i, j, 0].Y, colors[i, j, 0].Z);
                            basicEffect.World = cameraTransforms.worldMatrix;
                            basicEffect.View = cameraTransforms.viewMatrix;
                            basicEffect.Projection = cameraTransforms.projectionMatrix;
                            basicEffect.World = Matrix.Multiply(transforms[i, j], basicEffect.World);
                            mesh.Draw();
                        }
                    }
                }
            }

            foreach (ModelMesh mesh in wedge1.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    for (int i = 0; i < transforms.GetLength(0); i++)
                    {
                        for (int j = 0; j < transforms.GetLength(1); j++)
                        {
                            BasicEffect basicEffect = (BasicEffect) effect;
                            CommonBasicEffects.SetEffectsDarker(basicEffect);
                            basicEffect.DiffuseColor = new Vector3(colors[i, j, 1].X, colors[i, j, 1].Y, colors[i, j, 1].Z);
                            basicEffect.World = cameraTransforms.worldMatrix;
                            basicEffect.View = cameraTransforms.viewMatrix;
                            basicEffect.Projection = cameraTransforms.projectionMatrix;
                            basicEffect.World = Matrix.Multiply(transforms[i, j], basicEffect.World);
                            mesh.Draw();
                        }
                    }
                }
            }
        }
    }
}

