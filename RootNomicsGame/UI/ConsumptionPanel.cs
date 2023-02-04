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
    internal class ConsumptionPanel : Panel
    {
        LinkedSliders consumptionSliders;
        const int GrowPanelHeight = 60;

        public ConsumptionPanel(Rectangle frame)
            : base(frame, new LinearLayoutStrategy(Orientation.Vertical, 8, 16))
        {
            BackgroundColor = Color.AntiqueWhite;

            var slidersFrame = new Rectangle(0, 0, frame.Width, frame.Height - GrowPanelHeight);
            var consumption = new Dictionary<string, string>
            {
                { "potions-for-plants-id", "Heal Plants:" },
                { "potions-for-player-id", "Heal Self:" },
            };
            consumptionSliders = new LinkedSliders(slidersFrame, consumption, 0);

            AddChild(consumptionSliders);
        }
    }
}
