using Haiku;
using Haiku.Audio;
using Haiku.MathExtensions;
using Haiku.MonoGameUI;
using Haiku.MonoGameUI.Layouts;
using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using RootNomicsGame.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TexturePackerLoader;
using TexturePackerMonoGameDefinitions;
using static System.Net.Mime.MediaTypeNames;

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
        int turnCount;
        Random random;
        UserInterface ui;
        SpriteSheet uiTextureAtlas;
        private readonly Action restart;
        private readonly Action quit;
        internal bool IsSimulating => simulationUpdateCount >= 0;
        private int simulationUpdateCount = -1;

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

        public override void Update(double deltaSeconds)
        {
            if (IsSimulating)
            {
                var agentValues = agentCountSliders.GetValues();
                var healing = consumption.GetValues();

                var state = simulator.Simulate(agentValues, healing[ConsumptionPanel.PlantHealingKey], healing[ConsumptionPanel.PlayerHealingKey]);

                UpdateFromSimulationState(state);
                ++simulationUpdateCount;

                if (simulationUpdateCount >= 60)
                {
                    CompleteTurn(state, healing);
                }
            }
            base.Update(deltaSeconds);
        }

        void EndTurn(object button)
        {
            simulationUpdateCount = 0;
            IsInteractionEnabled = false;
            Add(Animate().Opacity(from: 1f, to: 0.5f).Over(0.4).Curving(RCurve.QuadEaseOut));
        }

        void CompleteTurn(SimulationState state, Dictionary<string, int> healing)
        {
            var damage = damageMin + random.NextDouble() * (damageMax - damageMin);
            int actualDamage = (int)Math.Round(damage);
            actualDamage -= healing[ConsumptionPanel.PlayerHealingKey];
            actualDamage = actualDamage.Clamp(-100, 100);

            damageMin *= 0.95 + random.NextDouble();
            damageMax *= 0.95 + random.NextDouble();
            if (damageMin > damageMax)
            {
                var swap = damageMax;
                damageMax = damageMin;
                damageMin = swap;
            }

            var min = Math.Max(0, DamageMin - LinkedSlider.PlayerHealSlider.Value);
            var max = Math.Max(0, DamageMax - LinkedSlider.PlayerHealSlider.Value);

            playerPanel.Update(actualDamage, min, max);

            simulationUpdateCount = -1;
            IsInteractionEnabled = true;
            Add(Animate().Opacity(from: 0.5f, to: 1f).Over(0.4).Curving(RCurve.QuadEaseIn));

            if (playerPanel.Health <= 0
                && state.TotalWealth <= 0)
            {
                ShowBadModalOptions(
    "Disaster!\n\n You passed on and all your plants are dead. Your family is left destitute, your remains thrown to the dogs and your scion sold to the poorhouse.\n\n",
    new ModalAction("Noooooo!!!", quit));
            }
            else if (playerPanel.Health <= 0)
            {
                ShowBadModalOptions(
                    "You passed on.\n\n Your remains decompose into the earth, sustaining the roots of the plants you so dearly love.\n\n Will your scion continue your legacy?\n\n",
                    new ModalAction("Yes", restart), new ModalAction("No", quit));
            }
            else if (state.TotalWealth <= 0)
            {
                ShowBadModalOptions(
                    "All your plants have died.\n\n Try again?\n\n",
                    new ModalAction("Yes", restart), new ModalAction("No", quit));
            }
            else
            {
                ++turnCount;
                Log.Debug("Turn Count: " + turnCount.ToString());
                if (turnCount > 16)
                {
                    ShowModalOptions(
    "Congratulations!\n\n You cheated death and your garden is the talk of the town!\n\n Relive the glory?\n\n",
    new ModalAction("Yes", restart), new ModalAction("No", quit));
                }
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

            var containerFrame = new Rectangle(0, 0, 1080, 768);
            var topLeftFrame = new Rectangle(0, 0, 327, 356);
            var bottomRightFrame = new Rectangle(1080 - 327, 768 - 356, 327, 356);

            var container = new Layout(containerFrame);
            var topLeft = new Panel(topLeftFrame)
            {
                Texture = uiTextureAtlas.Sprite(UITextureAtlas.IconWreathTopLeft),
            };
            var bottomRight = new Panel(bottomRightFrame)
            {
                Texture = uiTextureAtlas.Sprite(UITextureAtlas.IconWreathBottomRight),
            };

            container.AddChild(topLeft);
            container.AddChild(bottomRight);
            container.AddChild(dialog);
            dialog.CenterInParent();
            ShowDialog(container, UITextureAtlas.ModalBackground);
        }

        public void ShowBadModalOptions(string message, params ModalAction[] modalActions)
        {
            LinearLayout dialog = CreateModalOptions(message, modalActions);

            ShowDialog(dialog, UITextureAtlas.ModalBackgroundDestructive);
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
            var messageLayout = LinearLayout.ForText(message, HeadingFont, ModalWidth);

            dialog.AddChild(messageLayout);
            messageLayout.CenterXInParent();
        }

        Layout AddOptionButtons(Layout dialog, ModalAction[] modalActions, Action<ModalAction> onButtonPress)
        {
            LinearLayout optionsLayout = new LinearLayout(Rectangle.Empty, Orientation.Horizontal, ModalPanelSpacing, ModalPanelPadding);
            var availableOptionWidth = ModalWidth - ModalPanelSpacing * (modalActions.Length - 1) - ModalPanelPadding * 2;
            var optionWidth = availableOptionWidth / modalActions.Length;
            var first = true;

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
                if (first)
                {
                    var foreground = new Panel(new Rectangle(), new LinearLayoutStrategy(Orientation.Horizontal, 16))
                    {
                        BackgroundColor = Color.Transparent,
                    };
                    var leaf = new Panel(new Rectangle(0, 0, 22, 28))
                    {
                        Texture = uiTextureAtlas.Sprite(UITextureAtlas.IconLeafLightBackground),
                    };
                    var stringSize = MainFont.MeasureString(option.Title);
                    var width = (int)stringSize.X;
                    var height = (int)stringSize.Y;
                    var label = new Label(0, 0, (int)width, (int)height)
                    {
                        Font = MainFont,
                        Text = option.Title,
                        TextColor = Color.Black,
                        BackgroundColor = Color.Transparent,
                    };

                    foreground.AddChildren(new Layout[] {leaf, label});
                    label.CenterYInParent();
                    button.SetForegroundPanel(foreground);
                    foreground.CenterInParent();

                    first = false;
                }
                button.SetBackground(
                    uiTextureAtlas.NinePatch(UITextureAtlas.ButtonNormal),
                    uiTextureAtlas.NinePatch(UITextureAtlas.ButtonActive),
                    uiTextureAtlas.NinePatch(UITextureAtlas.ButtonSelected));
                optionsLayout.AddChild(button);
            }

            dialog.AddChild(optionsLayout);

            return optionsLayout;
        }

        void ShowDialog(Layout dialog, string spriteName)
        {
            Panel overlay = new Panel(Frame);
            overlay.BackgroundColor = Color.Black * 0.85f;
            Panel dialogBackground = new Panel()
            {
                Texture = uiTextureAtlas.Sprite(spriteName),
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
