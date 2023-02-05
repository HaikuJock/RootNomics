using Haiku.Audio;
using Haiku.MathExtensions;
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
using TexturePackerMonoGameDefinitions;

namespace RootNomicsGame.UI
{
    internal class HUD : Window
    {
        internal static HUD Instance;
        LinkedSliders agentCountSliders;
        StatsPanel stats;
        ConsumptionPanel consumption;
        PlayerPanel playerPanel;
        readonly Simulator simulator;
        Layout topContent;
        double damageMin;
        double damageMax;
        internal int DamageMin => (int)Math.Round(damageMin);
        internal int DamageMax => (int)Math.Round(damageMax);
        Random random;
        UserInterface ui;
        SpriteSheet uiTextureAtlas;
        private readonly Action restart;
        private readonly Action quit;

        public HUD(
            Rectangle frame,
            UserInterface ui,
            AudioPlaying audio,
            Simulator simulator,
            SpriteSheet uiTextureAtlas,
            Action restart,
            Action quit) 
            : base(frame, audio)
        {
            Instance = this;
            this.simulator = simulator;
            this.ui = ui;
            this.uiTextureAtlas = uiTextureAtlas;
            this.restart = restart;
            this.quit = quit;
            damageMin = 0.1;
            damageMax = 0.2;
            random = new Random();

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

            var state = simulator.Initialize(Configuration.InitialAgentTypeCount);
            AddChild(topContent);

            var playerFrame = new Rectangle(0, frame.Height - 60 - 32, 520, 60);
            playerPanel = new PlayerPanel(playerFrame, uiTextureAtlas);
            playerPanel.GrowButton.Action += EndTurn;
            AddChild(playerPanel);
            playerPanel.CenterXInParent();
            UpdateFromSimulationState(state);
        }

        void EndTurn(object button)
        {
            var agentValues = agentCountSliders.GetValues();
            var healing = consumption.GetValues();

            var state = simulator.Simulate(agentValues, healing[ConsumptionPanel.PlantHealingKey], healing[ConsumptionPanel.PlayerHealingKey]);

            UpdateFromSimulationState(state);

            var damage = damageMin + random.NextDouble() * (damageMax - damageMin);
            int actualDamage = (int)Math.Round(damage);
            actualDamage -= healing[ConsumptionPanel.PlayerHealingKey];
            actualDamage = actualDamage.Clamp(-100, 100);

            damageMin *= 0.95 + random.NextDouble();
            damageMax *= 0.95 + random.NextDouble();

            playerPanel.Update(actualDamage, DamageMin, DamageMax);

            if (playerPanel.Health <= 0
                && state.TotalWealth <= 0)
            {
                ShowModalOptions(
    "Disaster! You passed on and all your plants are dead. Your family is left destitute, your remains thrown to the dogs and your scion sold to the poorhouse.",
    new ModalAction("Noooooo!!!", quit));
            }
            else if (playerPanel.Health <= 0)
            {
                ShowModalOptions(
                    "You passed on. Your remains decompose into the earth, sustaining the roots of the plants you so dearly love. Will your scion continue your legacy?",
                    new ModalAction("Yes", restart), new ModalAction("No", quit));
            }
            else if (state.TotalWealth <= 0)
            {
                ShowModalOptions(
                    "All your plants have died. Try again?",
                    new ModalAction("Yes", restart), new ModalAction("No", quit));
            }
        }

        private void UpdateFromSimulationState(SimulationState state)
        {
            stats.Update(state);
            agentCountSliders.Update(state.AgentTypeCounts, state.Agents.Count);
            consumption.Update(state.TotalMagicJuice);
        }

        const int ModalWidth = 344;
        const int ModalPanelSpacing = 16;
        const int ModalPanelPadding = 8;
        const int ModalInset = 8;
        const int ButtonHeight = 44;

        public void ShowModalOptions(string message, params ModalAction[] modalActions)
        {
            LinearLayout dialog = CreateModalOptions(message, modalActions);

            ShowDialog(dialog);
        }

        public LinearLayout CreateModalOptions(string message, params ModalAction[] modalActions)
        {
            LinearLayout dialog = new LinearLayout(Rectangle.Empty, Orientation.Vertical, ModalPanelSpacing, ModalPanelPadding);

            AddMessage(dialog, message);
            AddOptionButtons(dialog, modalActions, (ModalAction option) => option.Action());

            return dialog;
        }

        void AddMessage(Layout dialog, string message)
        {
            var messageLayout = LinearLayout.ForText(message, BodyFont, ModalWidth);

            dialog.AddChild(messageLayout);
            messageLayout.CenterXInParent();
        }

        Layout AddOptionButtons(Layout dialog, ModalAction[] modalActions, Action<ModalAction> onButtonPress)
        {
            LinearLayout optionsLayout = new LinearLayout(Rectangle.Empty, Orientation.Horizontal, ModalPanelSpacing, ModalPanelPadding);
            var availableOptionWidth = ModalWidth - ModalPanelSpacing * (modalActions.Length - 1) - ModalPanelPadding * 2;
            var optionWidth = availableOptionWidth / modalActions.Length;

            foreach (var option in modalActions)
            {
                Button button = new Button(new Rectangle(0, 0, optionWidth, ButtonHeight), (_) =>
                {
                    ui.PopWindow();
                    onButtonPress(option);
                }, option.Title)
                {
                    IsFocusable = true
                };
                button.SetBackground(
                    uiTextureAtlas.NinePatch(UITextureAtlas.ButtonNormal),
                    uiTextureAtlas.NinePatch(UITextureAtlas.ButtonActive),
                    uiTextureAtlas.NinePatch(UITextureAtlas.ButtonSelected));
                optionsLayout.AddChild(button);
            }

            dialog.AddChild(optionsLayout);

            return optionsLayout;
        }

        public void ShowDialog(LinearLayout dialog)
        {
            ShowDialog(dialog, UITextureAtlas.ModalBackgroundDestructive);
        }

        void ShowDialog(LinearLayout dialog, string spriteName)
        {
            Panel overlay = new Panel(Frame);
            overlay.BackgroundColor = Color.Black * 0.75f;
            Panel dialogBackground = new Panel()
            {
                Texture = uiTextureAtlas.TiledObliqueNinePatch(spriteName, 22, 14),
            };

            dialogBackground.Frame = new Rectangle(
                dialog.Frame.X - ModalInset,
                dialog.Frame.Y - ModalInset,
                SpriteSheet.SizeForObliqueTiled(dialog.Frame.Width + ModalInset * 2, 22, 14),
                SpriteSheet.SizeForObliqueTiled(dialog.Frame.Height + ModalInset * 2, 22, 14));
            dialog.Frame = new Rectangle(ModalInset, ModalInset, dialog.Frame.Width, dialog.Frame.Height);

            dialogBackground.AddChild(dialog);
            dialog.CenterInParent();
            overlay.AddChild(dialogBackground);
            dialogBackground.CenterXInParent();
            dialogBackground.CenterYInParent();

            var window = new Window(Frame, audio);

            window.AddChild(overlay);
            ui.PushWindow(window);
        }
    }
}
