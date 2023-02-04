using Haiku.Audio;
using Haiku.MonoGameUI;
using Haiku.MonoGameUI.Layouts;
using Haiku.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        MousePressEventProviding mousePressEventProvider;
        HUD hud;
        AudioPlaying audio;

        public RootNomics()
        {
            graphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            mousePressEventProvider = new MousePressEventProvider();
            mousePressEventProvider.Initialize(this);
            audio = new NullAudioPlayer();
            userInterface = new UserInterface(this, mousePressEventProvider, new BrowserOpener(), new TextClipboard(), audio);
            Components.Add(userInterface);
            // Robb: Add your component here: e.g.
            // Components.Add(garden);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphicsManager.PreferredBackBufferWidth = 1768;
            graphicsManager.PreferredBackBufferHeight = 992;
            graphicsManager.IsFullScreen = false;
            graphicsManager.HardwareModeSwitch = false;
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
            var screenSize = GraphicsDevice.Viewport.Bounds.Size;
            hud = new HUD(new Rectangle(Point.Zero, screenSize), audio);
            userInterface.PushWindow(hud);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Exit();

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