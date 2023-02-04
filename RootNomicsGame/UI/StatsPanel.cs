using Haiku.MonoGameUI;
using Haiku.MonoGameUI.Layouts;
using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using RootNomicsGame.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNomicsGame.UI
{
    internal class StatsPanel : Panel
    {
        Label foodLabel;
        Label wealthLabel;
        Label juiceLabel;

        public StatsPanel(Rectangle frame)
            : base(frame, new FlexLayoutStrategy(new Flex
            {
                FlexDirection = FlexDirection.Row,
                ContentJustification = ContentJustification.SpaceAround,
                ItemAlignment = ItemAlignment.Center,
            }))
        {
            BackgroundColor = Color.WhiteSmoke;

            var foodLayout = new LinearLayout(Orientation.Horizontal, 4);
            var foodTitle = new Label("Nutrients:");
            foodLabel = new Label("0");
            foodLayout.AddChildren(new[] { foodTitle, foodLabel });

            var wealthLayout = new LinearLayout(Orientation.Horizontal, 4);
            var wealthTitle = new Label("Growth:");
            wealthLabel = new Label("0");
            wealthLayout.AddChildren(new[] { wealthTitle, wealthLabel });

            var juiceLayout = new LinearLayout(Orientation.Horizontal, 4);
            var juiceTitle = new Label("Healing:");
            juiceLabel = new Label("0");
            juiceLayout.AddChildren(new[] { juiceTitle, juiceLabel });

            AddChildren(new[] {foodLayout, wealthLayout, juiceLayout});
        }

        internal void Update(SimulationState state)
        {
            foodLabel.Text = state.TotalFood.ToString();
            wealthLabel.Text = state.TotalWealth.ToString();
            juiceLabel.Text = state.TotalMagicJuice.ToString();
        }
    }
}
