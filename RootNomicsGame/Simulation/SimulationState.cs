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

        internal SimulationState()
        {
            Agents = new List<Agent>();
            RebornAgents = new Dictionary<string, string>();
        }
    }
}
