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
        Dictionary<string, AgentSlider> sliders;
        
        internal AgentCountSliders(Rectangle frame, IDictionary<string, string> agentTypeNames, int totalAgents)
            : base(frame)
        {
            BackgroundColor = Color.MintCream;

            sliders = new Dictionary<string, AgentSlider>();
            var slidersFrame = new Rectangle(16, 0, 200, 200);
            var slidersContainer = new LinearLayout(slidersFrame, Orientation.Vertical, 0, 16);
            foreach (var typeNames in agentTypeNames)
            {
                var id = typeNames.Key;
                var name = typeNames.Value;
                var slider = new AgentSlider(id, name, totalAgents);

                sliders[id] = slider;
            }

            foreach (var slider in sliders.Values)
            {
                slider.Others = sliders.Values.Except(new[] { slider }).ToList();
            }
            slidersContainer.AddChildren(sliders.Values);
            AddChild(slidersContainer);
            Frame = new Rectangle(frame.Left, frame.Top, frame.Width, slidersContainer.Frame.Height);
        }

        internal void SetValues(Dictionary<string, int> values)
        {
            foreach (var value in values)
            {
                var type = value.Key;
                var count = value.Value;
                var slider = sliders[type];

                slider.SetValue(count);
            }
        }
    }
}
