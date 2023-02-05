
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using RootNomics.Environment;

/*
 * previously
 *
 *  groundTiles.DrawGroundTiles(cameraTransforms);
 *  fernModels.DrawFerns(cameraTransforms);
 *
 *
 * fernModels = new PlantModels(modelFern1, modelPlant1, groundTiles.tileHeights);
 *
 */
namespace RootNomics.SimulationRender
{
    class SimulationRenderer
    {

        private GroundTiles groundTiles;
        private List<AgentRenderingModel> models = new List<AgentRenderingModel>();

        public SimulationRenderer()
        {
        }

        public void SetGroundTiles(GroundTiles groundTiles)
        {
            this.groundTiles = groundTiles;
        }

        private Dictionary<string, GameModelRenderer> modelRegister = new();

        public void RegisterGameModel(string modelName, Model model)
        {
            // AgentRenderingModel agentRenderingModel = new AgentRenderingModel(model);

            GameModelRenderer gameModelRenderer = new GameModelRenderer(model);
            modelRegister.Add(modelName, gameModelRenderer);
        }



    }
}



