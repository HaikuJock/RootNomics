using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNomicsGame
{
    internal static class Configuration
    {
        internal static readonly Dictionary<string, string> AgentTypeNames = new Dictionary<string, string>
        {
            { "crocus-type", "Crocus" },
            { "wheat-type", "Wheat" },
            { "mandrake-type", "Mandrake" },
        };

        internal static readonly Dictionary<string, int> InitialAgentTypeCount = new Dictionary<string, int>
        {
            { "crocus-type", 3 },
            { "wheat-type", 3 },
            { "mandrake-type", 3 },
        };
    }
}
