
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using RootNomics.Environment;
using RootNomicsGame.Simulation;

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
        private CameraTransforms cameraTransforms;
        private GroundTiles groundTiles;
        private List<AgentRenderingModel> models = new List<AgentRenderingModel>();

        public SimulationRenderer(List<string> agentModelNames)
        {
        }

        public void RegisterCameraTransforms(CameraTransforms cameraTransforms)
        {
            this.cameraTransforms = cameraTransforms;
        }

        public void SetGroundTiles(GroundTiles groundTiles)
        {
            this.groundTiles = groundTiles;
        }

        private Dictionary<string, GameModel> modelsLookup = new();

        public void RegisterGameModel(string modelName, Model model)
        {
            // AgentRenderingModel agentRenderingModel = new AgentRenderingModel(model);

            GameModel gameModelRenderer = new GameModel(modelName, model);
            modelsLookup.Add(modelName, gameModelRenderer);
        }


        public void DrawGroundTiles()
        {
            groundTiles.DrawGroundTiles(cameraTransforms);
        }

        public GameModel GetGameModel(string modelName)
        {
            return modelsLookup[modelName];
        }

        public void Update(List<Agent> agents)
        {

        }
    }
}



