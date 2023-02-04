using Haiku.Audio;
using Haiku.MonoGameUI;
using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using RootNomicsGame.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TexturePackerLoader;

namespace RootNomicsGame.UI
{
    internal class HUD : Window
    {
        LinkedSliders agentCountSliders;
        StatsPanel stats;
        ConsumptionPanel consumption;
        PlayerPanel playerPanel;
        readonly Simulator simulator;
        Layout topContent;

        public HUD(Rectangle frame, AudioPlaying audio, Simulator simulator, SpriteSheet uiTextureAtlas) 
            : base(frame, audio)
        {
            this.simulator = simulator;

            topContent = new LinearLayout(Orientation.Horizontal, 16, 16);

            var slidersFrame = new Rectangle(0, 0, frame.Width, Math.Min(500, (int)(frame.Height * 0.26667f)));
            var totalCount = Configuration.InitialAgentTypeCount.Values.Sum(val => val);
            agentCountSliders = new LinkedSliders(slidersFrame, Configuration.AgentTypeNames, totalCount);

            topContent.AddChild(agentCountSliders);
            agentCountSliders.SetValues(Configuration.InitialAgentTypeCount);

            var statsFrame = new Rectangle(agentCountSliders.Frame.Right + 16, 0, frame.Width - 2 * (agentCountSliders.Frame.Right + 16), 44);
            stats = new StatsPanel(statsFrame);

            topContent.AddChild(stats);

            var consumptionFrame = new Rectangle(statsFrame.Right + 16, 0, agentCountSliders.Frame.Width, agentCountSliders.Frame.Height);
            consumption = new ConsumptionPanel(consumptionFrame, uiTextureAtlas);

            topContent.AddChild(consumption);

            consumption.GrowButton.Action += EndTurn;
            simulator.Initialize(Configuration.InitialAgentTypeCount);
            AddChild(topContent);

            var playerFrame = new Rectangle(0, frame.Height - 60 - 32, 200, 60);
            playerPanel = new PlayerPanel(playerFrame);
            AddChild(playerPanel);
            playerPanel.CenterXInParent();
        }

        void EndTurn(object button)
        {
            var agentValues = agentCountSliders.GetValues();
            var healing = consumption.GetValues();

            var state = simulator.Simulate(agentValues, healing[ConsumptionPanel.PlantHealingKey]);

            stats.Update(state);
            agentCountSliders.Update(state.Agents.Count);
            consumption.Update(state.TotalMagicJuice);
        }
    }
}
