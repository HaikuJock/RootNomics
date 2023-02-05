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

        static readonly Dictionary<string, int> typeCounts = new Dictionary<string, int>
        {
            { PlantHealingKey, 0 },
            { PlayerHealingKey, 0 },
        };

        internal void Update(int totalMagicJuice)
        {
            consumptionSliders.Update(typeCounts, totalMagicJuice);
        }
    }
}
