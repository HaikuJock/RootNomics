using Haiku.Audio;
using Haiku.MonoGameUI;
using Haiku.MonoGameUI.Layouts;
using Haiku.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RootNomicsGame.Simulation;
using RootNomicsGame.UI;
using System;
using TexturePackerLoader;
using TexturePackerMonoGameDefinitions;

namespace RootNomicsGame
{
    public class RootNomics : Game
    {
        private GraphicsDeviceManager graphicsManager;
        private SpriteBatch _spriteBatch;
        internal SpriteSheet UiSpriteSheet { get; private set; }
        UserInterface userInterface;
        HUD hud;
        Point screenSize;
        private readonly MousePressEventProviding mousePressEventProvider;
        private readonly BrowserOpening browserOpener;
        private readonly TextClipboarding clipboard;
        private readonly AudioPlaying audio;

        public RootNomics()
        {
        }

        public RootNomics(
            MousePressEventProviding mousePressEventProvider,
            BrowserOpening browserOpener,
            TextClipboarding clipboard,
            AudioPlaying audio)
        {
            this.mousePressEventProvider = mousePressEventProvider;
            this.browserOpener = browserOpener;
            this.clipboard = clipboard;
            this.audio = audio;
            graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            mousePressEventProvider.Initialize(this);
            userInterface = new UserInterface(this, mousePressEventProvider, browserOpener, clipboard, audio);
            Components.Add(userInterface);
            // Robb: Add your component here: e.g.
            // Components.Add(garden);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            int w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphicsManager.PreferredBackBufferWidth = w;   // 1768
            graphicsManager.PreferredBackBufferHeight = h; // 992
#if DEBUG
            graphicsManager.IsFullScreen = false;   // Otherwise when you hit a breakpoint, bad things happen
            graphicsManager.HardwareModeSwitch = false;
#else
            graphicsManager.IsFullScreen = true;
            graphicsManager.HardwareModeSwitch = true;
#endif
            graphicsManager.ApplyChanges();

            LoadUIContent();
            Window.KeyDown += OnKeyDown;
            Window.KeyUp += OnKeyUp;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            userInterface.Pointer = UiSpriteSheet.Sprite(UITextureAtlas.IconPointer);
            screenSize = GraphicsDevice.Viewport.Bounds.Size;
            hud = new HUD(new Rectangle(Point.Zero, screenSize), audio, new Simulator());
            userInterface.PushWindow(hud);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.R))
            {
                hud = new HUD(new Rectangle(Point.Zero, screenSize), audio, new Simulator());
                userInterface.PopAllWindows();
                userInterface.PushWindow(hud);
            }
            // TODO: Add your update logic here
            mousePressEventProvider.OnNewFrame(clipCursor: false);

            base.Update(gameTime);
            userInterface.PostUpdate();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            userInterface.PopAllWindows();
            base.OnExiting(sender, args);
        }

        void OnKeyDown(object sender, InputKeyEventArgs e)
        {
            var key = KeyExtensions.ReinterpretVisuallySimilarKey((Haiku.UI.Keys)e.Key);

            userInterface.OnKeyDown(key);
        }

        void OnKeyUp(object sender, InputKeyEventArgs e)
        {
            var key = KeyExtensions.ReinterpretVisuallySimilarKey((Haiku.UI.Keys)e.Key);

            userInterface.OnKeyUp(key);
        }

        void LoadUIContent()
        {
            Layout.TitleFont = Content.Load<SpriteFont>("Fonts/Title");
            Layout.HeadingFont = Content.Load<SpriteFont>("Fonts/Heading");
            Layout.MainFont = Content.Load<SpriteFont>("Fonts/Main");
            Layout.BodyFont = Content.Load<SpriteFont>("Fonts/Body");
            Layout.LabelFont = Content.Load<SpriteFont>("Fonts/Label");
            Layout.ItemFont = Content.Load<SpriteFont>("Fonts/Item");
            var spriteSheetLoader = new SpriteSheetLoader(Content);
            UiSpriteSheet = spriteSheetLoader.Load("UITextureAtlas", UITextureAtlas.MissingIcon);
            ScrollPanel.BottomShadow = UiSpriteSheet.Sprite(UITextureAtlas.ShadowBottom);
            ScrollPanel.TopShadow = UiSpriteSheet.NinePatch(UITextureAtlas.ShadowTop);
            ScrollPanel.LeftAndCornersShadow = UiSpriteSheet.VerticalPatch(UITextureAtlas.ShadowLeftAndCorners);
            ScrollPanel.RightAndCornersShadow = UiSpriteSheet.NinePatch(UITextureAtlas.ShadowRightAndCorners);
            ScrollBar.UpArrowTexture = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarUp);
            ScrollBar.UpArrowTextureActive = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarUpActive);
            ScrollBar.UpArrowTextureSelected = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarUpSelected);
            ScrollBar.DownArrowTexture = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarDown);
            ScrollBar.DownArrowTextureActive = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarDownActive);
            ScrollBar.DownArrowTextureSelected = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarDownSelected);
            ScrollBar.LeftArrowTexture = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarLeft);
            ScrollBar.LeftArrowTextureActive = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarLeftActive);
            ScrollBar.LeftArrowTextureSelected = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarLeftSelected);
            ScrollBar.RightArrowTexture = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarRight);
            ScrollBar.RightArrowTextureActive = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarRightActive);
            ScrollBar.RightArrowTextureSelected = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarRightSelected);
            ScrollBar.ScrollThumbTexture = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonNormal);
            ScrollBar.ScrollThumbTextureActive = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonActive);
            ScrollBar.ScrollThumbTextureSelected = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonSelected);
            ScrollBar.TroughTexture = UiSpriteSheet.NinePatch(UITextureAtlas.ScrollBarBackground);
            TextField.Background = UiSpriteSheet.NinePatch(UITextureAtlas.TextFieldBackground);
            TextField.IBar = UiSpriteSheet.Sprite(UITextureAtlas.TextFieldIBar);
            SecureTextField.ShowPasswordIcon = UiSpriteSheet.Sprite(UITextureAtlas.IconCheck);
            SecureTextField.HidePasswordIcon = UiSpriteSheet.Sprite(UITextureAtlas.IconMinus);
            SecureTextField.BackgroundNormal = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonNormal);
            SecureTextField.BackgroundActive = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonActive);
            SecureTextField.BackgroundHighlighted = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonSelected);
            SecureTextField.BackgroundSelected = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonSelected);
            CheckBox.BoxTexture = UiSpriteSheet.NinePatch(UITextureAtlas.TextFieldBackground);
            CheckBox.BoxTextureActive = UiSpriteSheet.Sprite(UITextureAtlas.ButtonCheckBoxActive);
            CheckBox.CheckTexture = UiSpriteSheet.Sprite(UITextureAtlas.IconCheck);
            Slider.IndicatorTextureNormal = UiSpriteSheet.Sprite(UITextureAtlas.ButtonNormal);
            Slider.IndicatorTextureActive = UiSpriteSheet.Sprite(UITextureAtlas.ButtonActive);
            Slider.IndicatorTextureSelected = UiSpriteSheet.Sprite(UITextureAtlas.ButtonSelected);
            Slider.TrackTextureLeft = UiSpriteSheet.NinePatch(UITextureAtlas.SliderTrackLeft);
            Slider.TrackTextureRight = UiSpriteSheet.NinePatch(UITextureAtlas.SliderTrackRight);
            DualSlider.TrackTextureLeft = UiSpriteSheet.NinePatch(UITextureAtlas.DualSliderTrackLeft);
            DualSlider.TrackTextureMiddle = UiSpriteSheet.NinePatch(UITextureAtlas.DualSliderTrackMiddle);
            DropDownButton.ArrowNormal = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarDown);
            DropDownButton.ArrowActive = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarDownActive);
            DropDownButton.ArrowSelected = UiSpriteSheet.Sprite(UITextureAtlas.IconScrollBarDownSelected);
            DropDown.ButtonNormal = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonNormal);
            DropDown.ButtonActive = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonActive);
            DropDown.ButtonSelected = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonSelected);
            DropDownCell.CellNormal = UiSpriteSheet.NinePatch(UITextureAtlas.CellNormal);
            DropDownCell.CellActive = UiSpriteSheet.NinePatch(UITextureAtlas.CellActive);
            DropDownCell.CellSelected = UiSpriteSheet.NinePatch(UITextureAtlas.CellSelected);
            TreeTextures.ButtonNormal = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonNormal);
            TreeTextures.ButtonActive = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonActive);
            TreeTextures.ButtonSelected = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonSelected);
            TreeTextures.Plus = UiSpriteSheet.Sprite(UITextureAtlas.IconPlus);
            TreeTextures.Minus = UiSpriteSheet.Sprite(UITextureAtlas.IconMinus);
            PressShrinkReleaseGrowBar.Background = UiSpriteSheet.NinePatch(UITextureAtlas.ButtonPullDown);
            TopBar.Background = UiSpriteSheet.NinePatch(UITextureAtlas.BackgroundTopBar);
        }
    }
}