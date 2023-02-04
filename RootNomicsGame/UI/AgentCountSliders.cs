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
        private readonly int totalAgents;
        Label totalAgentCountLabel;
        Dictionary<string, AgentSlider> sliders;
        
        internal AgentCountSliders(Rectangle frame, IDictionary<string, string> agentTypeNames, int totalAgents)
            : base(frame, new LinearLayoutStrategy(Orientation.Vertical, 8, 16, 16))
        {
            BackgroundColor = Color.MintCream;

            sliders = new Dictionary<string, AgentSlider>();
            foreach (var typeNames in agentTypeNames)
            {
                var id = typeNames.Key;
                var name = typeNames.Value;
                var slider = new AgentSlider(id, name, totalAgents);

                sliders[id] = slider;
            }

            totalAgentCountLabel = new Label($"Available: {totalAgents}", BodyFont);
            foreach (var slider in sliders.Values)
            {
                slider.Others = sliders.Values.Except(new[] { slider }).ToList();
                slider.TotalLabel = totalAgentCountLabel;
            }
            AddChild(totalAgentCountLabel);
            AddChildren(sliders.Values);
            this.totalAgents = totalAgents;
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
            var available = totalAgents - values.Values.Sum();
            totalAgentCountLabel.Text = $"Available: {available}";
        }
    }
}
