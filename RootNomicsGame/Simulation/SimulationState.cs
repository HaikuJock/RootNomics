using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNomicsGame.Simulation
{
    internal class SimulationState
    {
        public List<Agent> Agents;
        public Dictionary<string, string> RebornAgents; // map of the ids of the agents that have died to the new class they have been reborn with
        public IDictionary<string, int> AgentTypeCounts;
        public int TotalFood;   // Maybe don't need this?
        public int TotalWealth; // I guess I could calculate this?
        public int TotalMagicJuice; // The product that the player either consumes to stay alive or feeds to the simulation to keep the plants alive.

        internal SimulationState()
        {
            Agents = new List<Agent>();
            RebornAgents = new Dictionary<string, string>();
            AgentTypeCounts = new Dictionary<string, int>();
            TotalFood = 0;
            TotalWealth = 0;
            TotalMagicJuice = 0;
        }

        internal static SimulationState FakeState()
        {
            var state = new SimulationState();

            var random = new Random();

            state.TotalFood = random.Next(10, 100);
            state.TotalWealth = 0;
            state.TotalMagicJuice = random.Next(10, 100);

            var agentCount = random.Next(0, 100);
            var typeIds = Configuration.AgentTypeNames.Keys.ToList();

            foreach (var type in typeIds)
            {
                state.AgentTypeCounts[type] = 0;
            }

            for (int i = 0; i < agentCount; i++)
            {
                var typeIndex = random.Next(0, typeIds.Count);
                var type = typeIds[typeIndex];
                var wealth = random.Next(0, 33);

                state.TotalWealth += wealth;
                state.Agents.Add(new Agent()
                {
                    Id = i.ToString(),
                    Type = type,
                    Wealth = wealth,
                });
                state.AgentTypeCounts[type] = state.AgentTypeCounts[type] +1;
            }



            return state;
        }
    }
}
