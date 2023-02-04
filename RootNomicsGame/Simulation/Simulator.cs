using EconomySim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNomicsGame.Simulation
{
    internal class Simulator
    {
        Economy economy;

        internal Simulator()
        {
        }

        internal void Initialize(IDictionary<string, int> agentTypeCount)
        {
            // Create RootNomicEconomy
            // Create Markets
            // Add Markets to RootNomicEconomy
            // Construct Market with RootNomicEconomy
            // init Market with MarketData

            // or

            // Create DoranAndParberryEconomy - has some data in already
            economy = new DoranAndParberryEconomy();
        }

        internal SimulationState Simulate(IDictionary<string, int> agentTypeCount, int magicJuiceForPlants)
        {
            economy.simulate(1);
            var market = economy.getMarket("default");
            var allGoodsCounts = market.countAllGoods();
            var result = new SimulationState
            {
                TotalWealth = allGoodsCounts["wealth"],
                TotalFood = allGoodsCounts["food"],
                TotalMagicJuice = allGoodsCounts["tools"]
            };

            foreach (var agent in market._agents)
            {
                result.Agents.Add(new Agent
                {
                    Id = agent.id.ToString(),
                    Type = agent.className,
                    Wealth = (int)Math.Round(agent.money),
                });
            }

            return result;
        }
    }
}
