using Haiku.MonoGameUI;
using Haiku.MonoGameUI.Layouts;
using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TexturePackerLoader;
using TexturePackerMonoGameDefinitions;

namespace RootNomicsGame.UI
{
    internal class ConsumptionPanel : Panel
    {
        internal const string PlantHealingKey = "potions-for-plants-id";
        internal const string PlayerHealingKey = "potions-for-player-id";
        LinkedSliders consumptionSliders;

        public ConsumptionPanel(Rectangle frame, SpriteSheet uiTextureAtlas)
            : base(frame, new LinearLayoutStrategy(Orientation.Vertical, 8, 16))
        {
            BackgroundColor = Color.AntiqueWhite;

            var slidersFrame = new Rectangle(0, 0, frame.Width, frame.Height);
            var consumption = new Dictionary<string, string>
            {
                { PlantHealingKey, "Heal Plants:" },
                { PlayerHealingKey, "Heal Self:" },
            };
            consumptionSliders = new LinkedSliders(slidersFrame, consumption, 0);

            AddChild(consumptionSliders);
        }

        internal Dictionary<string, int> GetValues() => consumptionSliders.GetValues();

        internal void Update(int totalMagicJuice)
        {
            var currentHealingValues = consumptionSliders.GetValues();

            // Maintain ratio of healing values given new total
            var currentTotal = (double)consumptionSliders.Total;

            var newPlant = 0;
            var newPlayer = 0;

            if (currentTotal > 0)
            {
                var plantRatio = currentHealingValues[PlantHealingKey] / currentTotal;
                var playerRatio = currentHealingValues[PlayerHealingKey] / currentTotal;
                var newTotal = (double)totalMagicJuice;

                newPlant = (int)Math.Round(newTotal * plantRatio);
                newPlayer = (int)Math.Round(newTotal * playerRatio);
                while (newPlant + newPlayer > totalMagicJuice)
                {
                    --newPlayer;
                    if (newPlant + newPlayer > totalMagicJuice)
                    {
                        --newPlant;
                    }
                }
            }

            var typeCounts = new Dictionary<string, int>
            {
                { PlantHealingKey, newPlant },
                { PlayerHealingKey, newPlayer },
            };

            consumptionSliders.Update(typeCounts, totalMagicJuice);
        }
    }
}
