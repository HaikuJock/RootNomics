using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RootNomics.Environment;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RootNomics.SimulationRender
{
    class AgentRenderingModel
    {
        public string Id;
        //"worker",
        //"blacksmith",
        //"miner",
        //"refiner",
        //"woodcutter"
        public string Type;
        public float Wealth;

        private List<GameModel> agentGameModels;
        public int boardX;
        public int boardY;

        private int modelRotationDegrees;

        public AgentRenderingModel(string Id, string Type, float Wealth, List<GameModel> agentGameModels, int x, int y)
        {
            this.Id = Id;
            this.Type = Type;
            this.Wealth= Wealth;
            this.agentGameModels = agentGameModels;
            this.boardX = x;
            this.boardY = y;
            this.modelRotationDegrees = RandomNum.GetRandomInt(0, 360);
        }

        public void DrawModels(CameraTransforms cameraTransforms)
        {
            Matrix T = Matrix.CreateTranslation(boardX, boardY, 0);
            var wealthScale = Math.Max(0.1f, Wealth);
            Vector3 scale = Vector3.Multiply(new Vector3(1, 1, 1), wealthScale / 50f);
            Matrix S = Matrix.CreateScale(scale);
            Matrix R = Matrix.CreateRotationZ(MathHelper.ToRadians(modelRotationDegrees));


            Matrix transform = Matrix.Multiply(R, T);
            transform = Matrix.Multiply(S, transform);

            foreach (ModelMesh mesh in agentGameModels[0].model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    BasicEffect basicEffect = (BasicEffect)effect;
                    CommonBasicEffects.SetEffects(basicEffect);
                    basicEffect.View = cameraTransforms.viewMatrix;
                    basicEffect.Projection = cameraTransforms.projectionMatrix;
                    basicEffect.World = Matrix.Multiply(transform, cameraTransforms.worldMatrix);
                }
                mesh.Draw();
            }
        }
    }
}

