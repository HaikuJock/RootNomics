using Haiku.MonoGameUI;
using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNomicsGame.UI
{
    internal class AgentSlider : LinearLayout
    {
        public const int Width = 200;

        readonly string id;
        Label nameLabel;
        Label minLabel;
        Label maxLabel;
        Label valueLabel;

        internal AgentSlider(string id, string name, int max)
            : base(new Rectangle(0, 0, 200, 44), Orientation.Vertical, 4, 8)
        {
            this.id = id;
            nameLabel = new Label(name, BodyFont);
            AddChild(nameLabel);

            var minMaxFrame = new Rectangle(0, 0, Width, 24);
            var minMaxLayout = new FormLayout(minMaxFrame);

            minLabel = new Label("0");
            maxLabel = new Label(max.ToString());
            minMaxLayout.AddChildren(new[] { minLabel, maxLabel });
            AddChild(minMaxLayout);

            var sliderFrame = new Rectangle(0, 0, 200, 44);
            var slider = new OrdinalSlider(sliderFrame, 32, new Point(44, 44), 0, max);
            slider.OnChanged = SetAgentCount;
            slider.SetValue(max / 2);

            AddChild(slider);

            valueLabel = new Label((max / 2).ToString());
            AddChild(valueLabel);
            valueLabel.CenterXInParent();
        }

        void SetAgentCount(int value)
        {
            valueLabel.Text = value.ToString();
            valueLabel.CenterXInParent();
        }

    }
}
