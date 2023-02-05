using EconomySim;
using RootNomics.SimulationRender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace RootNomicsGame.Simulation
{
    internal class Simulator
    {
        public static int PlantHealingFactor = 3;

        DoranAndParberryEconomy economy;

        internal Simulator()
        {
        }

        public static SimulationRenderer simulationRenderer;

        internal SimulationState Initialize(IDictionary<string, int> agentTypeCounts)
        {

            simulationRenderer.Reset();

            // simulationRenderer = new SimulationRenderer();

            // Create RootNomicEconomy
            // Create Markets
            // Add Markets to RootNomicEconomy
            // Construct Market with RootNomicEconomy
            // init Market with MarketData

            // or

            // Create DoranAndParberryEconomy - has some data in already
            economy = new DoranAndParberryEconomy();
            economy.enforceAgentTypeCounts("default", agentTypeCounts);

            return CalculateSimulationState();
        }

        internal SimulationState Simulate(IDictionary<string, int> agentTypeCounts, int healingForPlants, int healingForPlayer)
        {
            economy.enforceAgentTypeCounts("default", agentTypeCounts);
            var market = economy.getMarket("default");
            if (healingForPlants > 0)
            {
                // add money to the economy
                market.addMoney(healingForPlants * PlantHealingFactor);
                economy.Funds = healingForPlants * PlantHealingFactor;
            }
            if (healingForPlayer > 0)
            {
                // remove tools from the economy
                market.removeGood("tools", healingForPlayer);
                market.taxTheRich(healingForPlayer);
            }
            //var state = CalculateSimulationState();
            //var typeIds = Configuration.InitialAgentTypeCount.Keys.ToList();

            //foreach (var type in typeIds)
            //{
            //    System.Diagnostics.Debug.WriteLine($"{type}: {state.AgentTypeCounts[type]}");
            //}

            economy.simulate(60);

            SimulationState simulationState = CalculateSimulationState();
            simulationRenderer.Update(simulationState.Agents);
            return simulationState;
        }

        private SimulationState CalculateSimulationState()
        {
            var market = economy.getMarket("default");
            var allGoodsCounts = market.countAllGoods();
            var result = new SimulationState
            {
                TotalWealth = allGoodsCounts["wealth"],
                TotalFood = allGoodsCounts["food"],
                TotalMagicJuice = allGoodsCounts["tools"]
            };

            var typeIds = Configuration.AgentTypeNames.Keys.ToList();

            foreach (var type in typeIds)
            {
                result.AgentTypeCounts[type] = 0;
            }

            foreach (var agent in market._agents)
            {
                if (typeIds.Contains(agent.className))
                {
                    result.Agents.Add(new Agent
                    {
                        Id = agent.id.ToString(),
                        Type = agent.className,
                        Wealth = (int)Math.Round(agent.money),
                    });
                    result.AgentTypeCounts[agent.className] = result.AgentTypeCounts[agent.className] + 1;
                }
            }


            return result;
        }
    }
}
