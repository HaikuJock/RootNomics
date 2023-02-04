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
    }
}
