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
            { "woodcutter", "Crocus" },
            { "farmer", "Wheat" },
            { "blacksmith", "Mandrake" },
        };

        internal static readonly Dictionary<string, int> InitialAgentTypeCount = new Dictionary<string, int>
        {
            { "woodcutter", 3 },
            { "farmer", 3 },
            { "blacksmith", 3 },
        };
    }
}
