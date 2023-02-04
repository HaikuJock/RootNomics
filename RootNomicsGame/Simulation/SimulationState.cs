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
        public Dictionary<string, string> RebornAgents; // map of the ids of the agents that have died to the new id they were replaced with
        public int TotalFood;   // Maybe don't need this?
        public int TotalWealth; // I guess I could calculate this?
        public int TotalMagicJuice; // The product that the player either consumes to stay alive or feeds to the simulation to keep the plants alive.

        internal SimulationState()
        {
            Agents = new List<Agent>();
            RebornAgents = new Dictionary<string, string>();
        }
    }
}
