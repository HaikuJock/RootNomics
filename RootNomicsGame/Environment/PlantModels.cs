using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RootNomics.Environment
{
    class PlantModels
    {
        private Model fernModel;
        private Model plantModel1;
        private float[,] tileHeight;
        private int offset;
        // private Matrix[,] transforms = new Matrix[2,];
        private Matrix[] transforms0;
        private Matrix[] transforms1;
        private List<(float sX, float sY, float sZ, float rot, int x, int y, float dx, float dy)> fernPlacements = new();
        private List<(float sX, float sY, float sZ, float rot, int x, int y, float dx, float dy)> plantPlacements = new();

        public PlantModels(Model fernModel, Model plantModel1, float[,] tileHeight)
        {
            this.fernModel = fernModel;
            this.plantModel1 = plantModel1;
            this.tileHeight = tileHeight;
            this.offset = tileHeight.GetLength(0) / 2;

            AddFern(sX: 2, sY: 2, sZ: 4, rot: 0, x: -9, y: -10, dx: 0, dy: 0);
            AddFern(sX: 2, sY: 1, sZ: 2, rot: 25, x: -9, y: -8, dx: 0.3f, dy: 0);
            AddFern(sX: 1, sY: 1, sZ: 2, rot: 25, x: -7, y: -9, dx: 0.3f, dy: 0);
            AddFern(sX: 1.5f, sY: 1.5f, sZ: 3, rot: 155, x: 1, y: -1, dx: 0.3f, dy: 0.3f);

            AddPlant1(sX: 9, sY: 9, sZ: 6, rot: 0, x: 0, y: 0, dx: 0, dy: 0);
            AddPlant1(sX: 3, sY: 4, sZ: 8, rot: 65, x: 3, y: 1, dx: 0.4f, dy: 0);
            AddPlant1(sX: 5, sY: 4, sZ: 5, rot: 165, x: 2, y: 1, dx: 0.4f, dy: 0);

            transforms0 = new Matrix[fernPlacements.Count];
            for (int i = 0; i < fernPlacements.Count; i++)
            {
                var fernData = fernPlacements[i];
                Matrix scale = Matrix.CreateScale(fernData.sX, fernData.sY, fernData.sZ);
                Matrix rotation = Matrix.CreateRotationZ(MathHelper.ToRadians(fernData.rot));
                Matrix translation = Matrix.CreateTranslation(fernData.x + fernData.dx, fernData.y + fernData.dy,
                    tileHeight[fernData.x + offset, fernData.y + offset]);
                Matrix transform = Matrix.Multiply(rotation, translation);
                transform = Matrix.Multiply(scale, transform);
                transforms0[i] = transform;
            }
            transforms1 = new Matrix[plantPlacements.Count];
            for (int i = 0; i < plantPlacements.Count; i++)
            {
                var PlantData = plantPlacements[i];
                Matrix scale = Matrix.CreateScale(PlantData.sX, PlantData.sY, PlantData.sZ);
                Matrix rotation1 = Matrix.CreateRotationX(MathF.PI / 2);
                Matrix rotation2 = Matrix.CreateRotationZ(MathHelper.ToRadians(PlantData.rot));
                Matrix translation = Matrix.CreateTranslation(PlantData.x + PlantData.dx, PlantData.y + PlantData.dy,
                    tileHeight[PlantData.x + offset, PlantData.y + offset]);
                Matrix transform = Matrix.Multiply(rotation2, translation);
                transform = Matrix.Multiply(rotation1, transform);
                transform = Matrix.Multiply(scale, transform);
                transforms1[i] = transform;
            }
        }

        private void AddFern(float sX, float sY, float sZ, float rot, int x, int y, float dx, float dy)
        {
            fernPlacements.Add((sX, sY, sY, rot, x, y, dx, dy));
        }
        private void AddPlant1(float sX, float sY, float sZ, float rot, int x, int y, float dx, float dy)
        {
            plantPlacements.Add((sX, sY, sY, rot, x, y, dx, dy));
        }


        public void DrawFerns(CameraTransforms cameraTransform)
        {
            foreach (ModelMesh mesh in fernModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    foreach (Matrix transform in transforms0)
                    {
                        BasicEffect basicEffect = (BasicEffect) effect;
                        CommonBasicEffects.SetEffects(basicEffect);
                        basicEffect.World = cameraTransform.worldMatrix;
                        basicEffect.View = cameraTransform.viewMatrix;
                        basicEffect.Projection = cameraTransform.projectionMatrix;
                        basicEffect.World = Matrix.Multiply(transform, basicEffect.World);
                        mesh.Draw();
                    }

                }
            }
            foreach (ModelMesh mesh in plantModel1.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    foreach (Matrix transform in transforms1)
                    {
                        BasicEffect basicEffect = (BasicEffect) effect;
                        CommonBasicEffects.SetEffects(basicEffect);
                        basicEffect.World = cameraTransform.worldMatrix;
                        basicEffect.View = cameraTransform.viewMatrix;
                        basicEffect.Projection = cameraTransform.projectionMatrix;
                        basicEffect.World = Matrix.Multiply(transform, basicEffect.World);
                        mesh.Draw();
                    }

                }
            }

        }


    }
}
