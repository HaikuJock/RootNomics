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
            healthLabel = new Label("100");

            AddChild(healthTitle);
            AddChild(healthLabel);
        }
    }
}
