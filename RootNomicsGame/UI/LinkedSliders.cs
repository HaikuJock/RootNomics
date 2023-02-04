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
    internal class LinkedSliders : Panel
    {
        private readonly int total;
        Label totalCountLabel;
        Dictionary<string, LinkedSlider> sliders;
        
        internal LinkedSliders(Rectangle frame, IDictionary<string, string> sliderTypeNames, int total)
            : base(frame, new LinearLayoutStrategy(Orientation.Vertical, 8, 16, 16))
        {
            BackgroundColor = Color.MintCream;

            sliders = new Dictionary<string, LinkedSlider>();
            foreach (var typeNames in sliderTypeNames)
            {
                var id = typeNames.Key;
                var name = typeNames.Value;
                var slider = new LinkedSlider(id, name, total);

                sliders[id] = slider;
            }

            totalCountLabel = new Label($"Available: {total}", BodyFont);
            foreach (var slider in sliders.Values)
            {
                slider.Others = sliders.Values.Except(new[] { slider }).ToList();
                slider.TotalLabel = totalCountLabel;
            }
            AddChild(totalCountLabel);
            AddChildren(sliders.Values);
            this.total = total;
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
            var available = total - values.Values.Sum();
            totalCountLabel.Text = $"Available: {available}";
        }
    }
}
