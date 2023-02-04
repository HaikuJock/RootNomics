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
        internal Button GrowButton;
        LinkedSliders consumptionSliders;
        const int GrowPanelHeight = 60;

        public ConsumptionPanel(Rectangle frame, SpriteSheet uiTextureAtlas)
            : base(frame, new LinearLayoutStrategy(Orientation.Vertical, 8, 16))
        {
            BackgroundColor = Color.AntiqueWhite;

            var slidersFrame = new Rectangle(0, 0, frame.Width, frame.Height - GrowPanelHeight);
            var consumption = new Dictionary<string, string>
            {
                { PlantHealingKey, "Heal Plants:" },
                { "potions-for-player-id", "Heal Self:" },
            };
            consumptionSliders = new LinkedSliders(slidersFrame, consumption, 0);

            var growFrame = new Rectangle(0, 0, frame.Width, GrowPanelHeight);
            var growLayout = new Layout(growFrame);

            GrowButton = new Button();
            GrowButton.SetBackground(
                uiTextureAtlas.NinePatch(UITextureAtlas.ButtonNormal),
                uiTextureAtlas.NinePatch(UITextureAtlas.ButtonActive),
                uiTextureAtlas.NinePatch(UITextureAtlas.ButtonSelected));
            growLayout.AddChild(GrowButton);
            GrowButton.Frame = new Rectangle(0, 0, frame.Width - 32, 44);
            GrowButton.SetForeground("End Turn");
            GrowButton.CenterInParent();

            AddChild(consumptionSliders);
            AddChild(growLayout);
        }

        internal Dictionary<string, int> GetValues() => consumptionSliders.GetValues();

        internal void Update(int totalMagicJuice) => consumptionSliders.Update(totalMagicJuice);
    }
}
