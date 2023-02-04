using Haiku.MonoGameUI;
using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RootNomicsGame.UI
{
    internal class LinkedSlider : LinearLayout
    {
        public const int Width = 200;

        public List<LinkedSlider> Others {
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
        public Label TotalLabel { get; internal set; }
        List<LinkedSlider> others;
        public int Value => slider?.Value ?? 0;
        readonly string id;
        int max;
        List<LinkedSlider>.Enumerator othersIterator;
        OrdinalSlider slider;
        Label nameLabel;
        Label minLabel;
        Label maxLabel;
        Label valueLabel;
        Layout minMaxLayout;

        internal LinkedSlider(string id, string name, int max)
            : base(new Rectangle(0, 0, Width, 44), Orientation.Vertical, 0, 0)
        {
            this.id = id;
            this.max = max;
            var nameLayout = new LinearLayout(Orientation.Horizontal, 4);
            
            nameLabel = new Label(name, BodyFont);
            nameLayout.AddChild(nameLabel);
            valueLabel = new Label((0).ToString());
            nameLayout.AddChild(valueLabel);
            valueLabel.CenterYInParent();
            AddChild(nameLayout);

            var minMaxFrame = new Rectangle(0, 0, Width, 20);
            minMaxLayout = new FormLayout(minMaxFrame);

            minLabel = new Label("0");
            maxLabel = new Label(max.ToString());
            minMaxLayout.AddChildren(new[] { minLabel, maxLabel });
            AddChild(minMaxLayout);

            var sliderFrame = new Rectangle(0, 0, Width, 22);
            slider = new OrdinalSlider(sliderFrame, 12, new Point(22, 22), 0, max);
            slider.OnChanged = OnCountChanged;

            AddChild(slider);
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
        }

        void OnCountChanged(int value)
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
            var available = max - total;
            TotalLabel.Text = $"Available: {available}";
            SetValueLabel(value);
        }

        internal void SetMax(int max)
        {
            this.max = max;
            TotalLabel.Text = $"Available: {max}";
            slider?.RemoveFromParent();
            var sliderFrame = new Rectangle(0, 0, Width, 22);
            slider = new OrdinalSlider(sliderFrame, 12, new Point(22, 22), 0, max);
            slider.OnChanged = OnCountChanged;
            maxLabel.Text = max.ToString();
            maxLabel.SizeToFit();
            minMaxLayout.DoLayout();

            AddChild(slider);
        }
    }
}
