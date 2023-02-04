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

        public List<AgentSlider> Others {
            get
            {
                return others;
            }
            internal set
            {
                others = value;
                othersIterator = others.GetEnumerator();
            }
        }
        List<AgentSlider> others;
        public int Value => slider?.Value ?? 0;
        readonly string id;
        readonly int max;
        List<AgentSlider>.Enumerator othersIterator;
        OrdinalSlider slider;
        Label nameLabel;
        Label minLabel;
        Label maxLabel;
        Label valueLabel;

        internal AgentSlider(string id, string name, int max)
            : base(new Rectangle(0, 0, 200, 44), Orientation.Vertical, 0, 0)
        {
            this.id = id;
            this.max = max;
            nameLabel = new Label(name, BodyFont);
            AddChild(nameLabel);

            var minMaxFrame = new Rectangle(0, 0, Width, 20);
            var minMaxLayout = new FormLayout(minMaxFrame);

            minLabel = new Label("0");
            maxLabel = new Label(max.ToString());
            minMaxLayout.AddChildren(new[] { minLabel, maxLabel });
            AddChild(minMaxLayout);

            var sliderFrame = new Rectangle(0, 0, 200, 22);
            slider = new OrdinalSlider(sliderFrame, 12, new Point(22, 22), 0, max);
            slider.OnChanged = SetAgentCount;

            AddChild(slider);

            valueLabel = new Label((0).ToString());
            AddChild(valueLabel);
            valueLabel.CenterXInParent();
            SetValue(0);
        }

        public void SetValue(int value)
        {
            slider.SetValue(value);
            SetValueLabel(value);
        }

        private void SetValueLabel(int value)
        {
            valueLabel.Text = value.ToString();
            valueLabel.CenterXInParent();
        }

        void SetAgentCount(int value)
        {
            var total = Others.Sum(s => s.Value) + value;

            while (total > max)
            {
                if (othersIterator.MoveNext())
                {
                    var other = othersIterator.Current;

                    if (other.Value > 0)
                    {
                        other.SetValue(other.Value - 1);
                        --total;
                        if (total <= max)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    othersIterator = others.GetEnumerator();
                }
            }
            SetValueLabel(value);
        }

    }
}
