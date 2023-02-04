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
        AgentSlider[] sliders;
        
        internal AgentCountSliders(Rectangle frame, IDictionary<string, string> agentTypeNames, int totalAgents)
            : base(frame)
        {
            BackgroundColor = Color.MintCream;

            sliders = new AgentSlider[agentTypeNames.Count];
            var slidersFrame = new Rectangle(16, 0, 200, 200);
            var slidersContainer = new LinearLayout(slidersFrame, Orientation.Vertical, 0, 16);
            int i = 0;
            foreach (var typeNames in agentTypeNames)
            {
                var id = typeNames.Key;
                var name = typeNames.Value;
                var slider = new AgentSlider(id, name, totalAgents);

                sliders[i] = slider;
                ++i;
            }

            foreach (var slider in sliders)
            {
                slider.Others = sliders.Except(new[] { slider }).ToList();
            }
            slidersContainer.AddChildren(sliders);
            AddChild(slidersContainer);
            Frame = new Rectangle(frame.Left, frame.Top, frame.Width, slidersContainer.Frame.Height);
        }
    }
}
