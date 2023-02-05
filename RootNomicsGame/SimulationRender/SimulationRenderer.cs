
using System;
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
        // private List<AgentRenderingModel> models = new List<AgentRenderingModel>();

        GroundTilesOccupancy groundTilesOccupancy = new();

        public SimulationRenderer() { }

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


        private Dictionary<string, AgentRenderingModel> activeAgents = new();

        public void DrawAgents(CameraTransforms cameraTransforms)
        {
            foreach (AgentRenderingModel agentRenderingModel in activeAgents.Values)
            {
                agentRenderingModel.DrawModels(cameraTransforms);
            }
        }

        public void Reset()
        {
            groundTilesOccupancy = new();
            activeAgents = new();
        }

        public List<GameModel> GetAgentGameModel(string agentType)
        {
            List<GameModel> agentGameModels = new();
            switch (agentType)
            {
                case "farmer":
                    agentGameModels.Add(modelsLookup["reeds1"]);
                    agentGameModels.Add(modelsLookup["reeds1"]);
                    agentGameModels.Add(modelsLookup["acaciaTree1"]);
                    agentGameModels.Add(modelsLookup["acaciaTree2"]);
                    return agentGameModels;

                case "blacksmith":
                    agentGameModels.Add(modelsLookup["plant1"]);
                    agentGameModels.Add(modelsLookup["plant1"]);
                    agentGameModels.Add(modelsLookup["acaciaTree1"]);
                    agentGameModels.Add(modelsLookup["acaciaTree2"]);
                    return agentGameModels;

                case "miner":
                    agentGameModels.Add(modelsLookup["cactus1"]);
                    agentGameModels.Add(modelsLookup["cactus2"]);
                    agentGameModels.Add(modelsLookup["acaciaTree1"]);
                    agentGameModels.Add(modelsLookup["acaciaTree2"]);
                    return agentGameModels;

                case "refiner":
                    agentGameModels.Add(modelsLookup["smallPlant1"]);
                    agentGameModels.Add(modelsLookup["smallPlant1"]);
                    agentGameModels.Add(modelsLookup["pineTree1"]);
                    agentGameModels.Add(modelsLookup["pineTree2"]);
                    return agentGameModels;

                case "woodcutter":
                    agentGameModels.Add(modelsLookup["smallPlant1"]);
                    agentGameModels.Add(modelsLookup["smallPlant1"]);
                    agentGameModels.Add(modelsLookup["birchTree1"]);
                    agentGameModels.Add(modelsLookup["birchTree2"]);
                    agentGameModels.Add(modelsLookup["pineTree1"]);
                    agentGameModels.Add(modelsLookup["pineTree2"]);
                    return agentGameModels;

                case "worker":
                    agentGameModels.Add(modelsLookup["fern1"]);
                    agentGameModels.Add(modelsLookup["fern2"]);
                    agentGameModels.Add(modelsLookup["pineTree1"]);
                    agentGameModels.Add(modelsLookup["pineTree2"]);
                    return agentGameModels;

                default:
                    throw new Exception($"unknown agent type {agentType}");
            }
        }

        public void Update(List<Agent> agents)
        {
            groundTilesOccupancy.Update();
            foreach (Agent a in agents)
            {
                if (activeAgents.ContainsKey(a.Id))
                {
                    activeAgents[a.Id].Wealth = a.Wealth;
                    if (a.Wealth <= 0)
                    {
                        AgentRenderingModel agentRenderingModel = activeAgents[a.Id];
                        activeAgents.Remove(a.Id);
                        groundTilesOccupancy.RegisterFreedTile(agentRenderingModel.boardX, agentRenderingModel.boardY);
                    }
                }
                else
                {
                    (int x, int y) xy = groundTilesOccupancy.RequestFreeTile();
                    List<GameModel> agentGameModels = GetAgentGameModel(a.Type);
                    AgentRenderingModel agentRenderingModel = new AgentRenderingModel(a.Id, a.Type, a.Wealth, agentGameModels, xy.x, xy.y);
                    activeAgents[a.Id] = agentRenderingModel;
                }
            }
        }
    }
}



