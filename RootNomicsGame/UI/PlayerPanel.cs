using Haiku.MathExtensions;
using Haiku.MonoGameUI;
using Haiku.MonoGameUI.Layouts;
using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootNomicsGame.UI
{
    internal class PlayerPanel : Panel
    {
        Label healthLabel;
        internal int Health { get; private set; }

        public PlayerPanel(Rectangle frame)
            : base(frame, new FlexLayoutStrategy(new Flex
            {
                FlexDirection = FlexDirection.Row,
                ContentJustification = ContentJustification.SpaceAround,
                ItemAlignment = ItemAlignment.Center,
            }))
        {
            BackgroundColor = Color.Tomato;

            var healthTitle = new Label("Health:", BodyFont);
            Health = 100;
            healthLabel = new Label(Health.ToString());

            AddChild(healthTitle);
            AddChild(healthLabel);
        }

        internal void Update(int damage)
        {
            Health -= damage;
            Health = Health.Clamp(0, 100);
            healthLabel.Text = Health.ToString();
        }
    }
}
