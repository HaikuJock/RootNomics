using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RootNomics.Environment;

namespace RootNomics.SimulationRender
{
    class GameModel
    {
        private string modelName;
        private Model model;

        private Vector3 modelScaling = new Vector3(1, 1, 1);
        private Vector3 modelTranslation = new Vector3(0, 0, 0);
        private float modelXRotationDegrees = 0;
        // private float modelYRotation = 0; // not needed for any model
        private float modelZRotationDegrees = 0;
        private Matrix defaultModelTransform;

        private Vector3 defaultAmbientLightingColor = new Vector3(0.4f, 0.3f, 0.3f);
        private Vector3 defaultLightingDirection = new Vector3(0.8f, 0.8f, -1);
        private Vector3 defaultGameModelDiffuseColorLighting = new Vector3(0.8f, 0.8f, 0.8f);


        public GameModel(string modelName, Model model)
        {
            this.modelName = modelName;
            this.model = model;


            switch (modelName)
            {
                case "acaciaTree1":
                    modelScaling = new Vector3(0.7f, 0.7f, 0.7f);
                    break;

                // transforms weren't applied to this model in the Blender export
                case "acaciaTree2":
                    modelXRotationDegrees = -90;
                    modelScaling = new Vector3(0.007f, 0.007f, 0.007f);
                    break;

                case "birchTree1":
                    break;

                case "birchTree2":
                    break;

                case "cactus1":
                    break;

                case "cactus2":
                    break;

                case "fern1":
                    modelScaling = new Vector3(1.5f, 1.5f, 1.5f);
                    modelTranslation = new Vector3(0, 0, 0.1f);
                    break;

                case "fern2":
                    break;

                case "flower1":
                    break;

                case "flower2":
                    break;

                case "flower3":
                    break;

                case "flower4":
                    break;

                case "mushroom1":
                    break;

                case "mushroom2":
                    break;

                case "mushroom3":
                    break;

                case "mushroom4":
                    break;

                case "mushroom5":
                    break;

                case "mushroom6":
                    break;

                case "pineTree1":
                    modelScaling = new Vector3(0.5f, 0.5f, 0.5f);
                    break;

                case "pineTree2":
                    modelScaling = new Vector3(0.5f, 0.5f, 0.5f);
                    break;

                case "smallPlant1":
                    modelScaling = new Vector3(2f, 2f, 2f);
                    break;

                case "plant1":
                    modelXRotationDegrees = -90;
                    modelScaling = new Vector3(3.5f, 3.5f, 4.5f);
                    break;

                case "reeds1":
                    modelScaling = new Vector3(2f, 2f, 2f);
                    break;

                case "terrain1":
                    break;

                default:
                    throw new Exception($"unknown model name gives = {modelName}");
            }
            Matrix T = Matrix.CreateTranslation(modelTranslation);
            Matrix rotationXTransform = Matrix.CreateRotationX(MathHelper.ToDegrees(modelXRotationDegrees));
            Matrix rotationZTransform = Matrix.CreateRotationZ(MathHelper.ToDegrees(modelZRotationDegrees));
            Matrix S = Matrix.CreateScale(modelScaling);
            Matrix transform = Matrix.Multiply(rotationXTransform, T);
            transform = Matrix.Multiply(rotationZTransform, transform);
            transform = Matrix.Multiply(S, transform);
            defaultModelTransform = transform;

        }


        public void DrawModelWithDefaultValues(CameraTransforms cameraTransforms)
        {
            DrawModelWithDefaultValues(cameraTransforms, Matrix.Identity);
        }

        public void DrawModelWithDefaultValues(CameraTransforms cameraTransforms, Matrix transform)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    BasicEffect basicEffect = (BasicEffect) effect;
                    basicEffect.EnableDefaultLighting();
                    basicEffect.AmbientLightColor = defaultAmbientLightingColor;
                    basicEffect.DirectionalLight0.Enabled = true;
                    basicEffect.DirectionalLight0.Direction = defaultLightingDirection;
                    basicEffect.DirectionalLight0.DiffuseColor = defaultGameModelDiffuseColorLighting;

                    basicEffect.World = cameraTransforms.worldMatrix;
                    basicEffect.View = cameraTransforms.viewMatrix;
                    basicEffect.Projection = cameraTransforms.projectionMatrix;

                    transform = Matrix.Multiply(defaultModelTransform, transform);
                    basicEffect.World = Matrix.Multiply(transform, basicEffect.World);
                }
                mesh.Draw();
            }
        }
    }
}

