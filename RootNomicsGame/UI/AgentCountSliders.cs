using Haiku.MonoGameUI;
using Haiku.MonoGameUI.Layouts;
using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNomicsGame.UI
{
    internal class AgentCountSliders : Panel
    {
        internal const int AgentTypes = 3;
        internal static readonly string[] AgentTypeNames = new string[]
        {
            "Crocus",
            "Wheat",
            "Mandrake"
        };
        AgentSlider[] sliders = new AgentSlider[AgentTypes];
        
        internal AgentCountSliders(Rectangle frame)
            : base(frame, new LinearLayoutStrategy(Orientation.Vertical, 12, 16, 16))
        {
            BackgroundColor = Color.MintCream;

            for (int i = 0; i < AgentTypes; i++)
            {
                var slider = new AgentSlider(i.ToString(), AgentTypeNames[i], 3);
                sliders[i] = slider;
            }

            AddChildren(sliders);
        }
    }
}
