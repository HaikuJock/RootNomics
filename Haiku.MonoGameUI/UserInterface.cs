using Haiku.Audio;
using Haiku.MonoGameUI.Layouts;
using Haiku.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using TexturePackerLoader;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;
using MouseState = Microsoft.Xna.Framework.Input.MouseState;

namespace Haiku.MonoGameUI
{
    public class UserInterface : DrawableGameComponent, BrowserOpening
    {
        const double KeyRepeatInitialDelay = 0.16;
        const double KeyRepeatDelay = 0.083335;
        public bool DidHandleMouseMove { get; private set; }
        public bool DidHandleMouseButtons { get; private set; }
        public bool DidHandleKeyboardInput { get; private set; }
        public bool IsUIEnabled
        {
            get { return _isUIEnabled; }
            set
            {
                _isUIEnabled = value;
                if (_isUIEnabled)
                {
                    AlphaTo(1f);
                }
            }
        }

        public SpriteFrame Pointer;
        public Dictionary<Keys, Func<IEnumerable<Keys>, bool>> KeyBindings = new Dictionary<Keys, Func<IEnumerable<Keys>, bool>>();

        readonly MousePressEventProviding mousePressEventProvider;
        readonly BrowserOpening browserOpener;
        readonly TextClipboarding clipboard;
        readonly AudioPlaying audio;
        readonly Stack<Window> windowStack;
        readonly Stack<Window> drawStack;
        readonly Dictionary<Keys, double> keyRepeatTimers = new Dictionary<Keys, double>();
        readonly List<Keys> activeModifierKeys = new List<Keys>();
        SpriteBatch spriteBatch;
        MouseState previousMouseState;
        Rectangle pointerFrame;
        bool _isUIEnabled = true;
        bool didHandleTextInput;
        Animation fadeAnimation;
        float targetFadeAlpha;

        public UserInterface(
            Game game, 
            MousePressEventProviding mousePressEventProvider, 
            BrowserOpening browserOpener,
            TextClipboarding clipboard,
            AudioPlaying audio)
            : base(game)
        {
            this.mousePressEventProvider = mousePressEventProvider;
            this.browserOpener = browserOpener;
            this.clipboard = clipboard;
            this.audio = audio;
            game.Window.TextInput += TextInputHandler;
            windowStack = new Stack<Window>();
            drawStack = new Stack<Window>();
        }

        public void PushWindow(Window window)
        {
            if (windowStack.Count > 0)
            {
                windowStack.Peek().OnDisappear();
            }
            windowStack.Push(window);
            window.OnPushed();
            window.OnAppear();
            UpdateDrawStack();
        }

        public Window TopWindow()
        {
            return windowStack.Count > 0 ? windowStack.Peek() : null;
        }

        public bool PopWindow()
        {
            if (windowStack.Count > 1)
            {
                PopTopOfStack();
                if (windowStack?.Peek() != null)
                {
                    windowStack.Peek().OnAppear();
                }
                UpdateDrawStack();
                return true;
            }
            return false;
        }

        public bool PopToWindow(Window window)
        {
            while (windowStack.Count > 0
                && windowStack?.Peek() != window)
            {
                PopTopOfStack();
                if (windowStack.Count > 0)
                {
                    windowStack.Peek().OnAppear();
                }
            }
            UpdateDrawStack();
            if (windowStack.Count > 0
                && windowStack?.Peek() == window)
            {
                return true;
            }
            return false;
        }

        public void PopAllWindows()
        {
            while (windowStack.Count > 0)
            {
                PopTopOfStack();
                if (windowStack.Count > 0)
                {
                    windowStack.Peek().OnAppear();
                }
            }
            UpdateDrawStack();
        }

        public override void Initialize()
        {
            previousMouseState = Mouse.GetState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Panel.Graphics = GraphicsDevice;
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (IsActive())
            {
                var deltaSeconds = gameTime.ElapsedGameTime.TotalSeconds;
                var currentMouseState = Mouse.GetState();

                if (IsUIEnabled)
                {
                    UpdateKeyboardInput(deltaSeconds);
                    UpdateMouseMovement(currentMouseState, previousMouseState);
                    Tuple<MouseState, MouseState> mouseStates = mousePressEventProvider.MouseStates;
                    if (IsActive()
                        && mouseStates != null)
                    {
                        var current = mouseStates.Item1;
                        var previous = mouseStates.Item2;
                        UpdateMouseButtonPresses(current, previous);
                    }
                    previousMouseState = currentMouseState;
                }
                else
                {
                    DidHandleMouseMove = false;
                    if (IsActive()
                        && windowStack?.Peek() != null
                        && windowStack.Peek().AnyRayTraceHitsChild(currentMouseState.Position)
                        && windowStack.Peek().Alpha == 1f)
                    {
                        AlphaTo(0.2f);
                    }
                    else if (IsActive()
                             && windowStack?.Peek() != null
                             && !windowStack.Peek().AnyRayTraceHitsChild(currentMouseState.Position)
                             && windowStack.Peek().Alpha < 1f)
                    {
                        AlphaTo(1f);
                    }
                }
                if (IsActive())
                {
                    windowStack.Peek()?.Update(deltaSeconds);
                }
            }
            base.Update(gameTime);
        }

        public void OnKeyDown(Keys key)
        {
            var window = windowStack.Peek();

            AddActiveModifierKeys(new List<Keys> { key });

            DidHandleKeyboardInput = window?.HandleKeyPress(key, activeModifierKeys, clipboard) == true || DidHandleKeyboardInput;
            if (!DidHandleKeyboardInput
                && KeyBindings.ContainsKey(key))
            {
                DidHandleKeyboardInput = DidHandleKeyboardInput || KeyBindings[key](activeModifierKeys);
            }
        }

        public void OnKeyUp(Keys key)
        {
            keyRepeatTimers.Remove(key);
        }

        public void PostUpdate()
        {
            DidHandleKeyboardInput = false;
            didHandleTextInput = false;
        }

        public void OpenUrl(Uri url)
        {
            browserOpener.OpenUrl(url);
        }

        void PopTopOfStack()
        {
            windowStack.Peek().OnDisappear();
            windowStack.Pop().OnPopped();
        }

        void AlphaTo(float targetAlpha)
        {
            var window = windowStack.Peek();

            if (window != null
                && (fadeAnimation == null
                    || targetFadeAlpha != targetAlpha))
            {
                targetFadeAlpha = targetAlpha;
                window.CancelAnimation(fadeAnimation);
                fadeAnimation = window
                    .Animate()
                    .Over(0.2)
                    .Curving(MathExtensions.RCurve.QuadEaseInOut)
                    .Opacity(window.Alpha, targetFadeAlpha)
                    .CompletingWith(_ => fadeAnimation = null)
                    .Build();
                window.Add(fadeAnimation);
            }
        }

        bool IsActive()
        {
            return Game.IsActive
                && windowStack.Count > 0;
        }

        void TextInputHandler(object sender, TextInputEventArgs args)
        {
            if (windowStack.Count > 0)
            {
                var window = windowStack.Peek();

                didHandleTextInput = window?.HandleTextInput((Keys)args.Key, args.Character.ToString(), activeModifierKeys) == true;
            }
            else
            {
                didHandleTextInput = false;
            }
        }

        void RefreshActiveModifierKeys(IEnumerable<Keys> currentKeys)
        {
            activeModifierKeys.Clear();
            AddActiveModifierKeys(currentKeys);
        }

        void AddActiveModifierKeys(IEnumerable<Keys> currentKeys)
        {
            const int MaxModifierKeys = 2;

            foreach (var modifier in KeyExtensions.ModifierKeys)
            {
                if (currentKeys.Contains(modifier))
                {
                    if (activeModifierKeys.Count >= MaxModifierKeys)
                    {
                        break;
                    }
                    activeModifierKeys.Add(modifier);
                }
            }
        }

        void UpdateKeyboardInput(double deltaSeconds)
        {
            var currentKeyboardState = Keyboard.GetState();
            var currentKeys = KeyExtensions.ReinterpretVisuallySimilarKeys(currentKeyboardState.GetPressedKeys().Cast<Keys>());
            var window = windowStack.Peek();

            RefreshActiveModifierKeys(currentKeys);

            if (!DidHandleKeyboardInput)
            {
                foreach (var key in currentKeys)
                {
                    if (ShouldRepeatKey(key, deltaSeconds))
                    {
                        DidHandleKeyboardInput = window?.HandleKeyPress(key, activeModifierKeys, clipboard) == true || DidHandleKeyboardInput;
                    }
                }
            }

            DidHandleKeyboardInput = DidHandleKeyboardInput || didHandleTextInput;
        }

        void UpdateMouseMovement(MouseState current, MouseState previous)
        {
            Point location = current.Position;
            Rectangle screen = GraphicsDevice.Viewport.Bounds;

            if (location != previous.Position)
            {
                var edgeScrollingScreen = screen;

                edgeScrollingScreen.Inflate(-1, -1);

                if (edgeScrollingScreen.Contains(location))
                {
                    var window = windowStack.Peek();
                    DidHandleMouseMove = window?.HandleCursorMove(location, screen) == true;
                }
                else
                {
                    DidHandleMouseMove = false;
                }
            }

            pointerFrame = new Rectangle(location, Pointer.SourceRectangle.Size);
        }

        void UpdateMouseButtonPresses(MouseState current, MouseState previous)
        {
            Point location = current.Position;
            Rectangle screen = GraphicsDevice.Viewport.Bounds;
            var window = windowStack.Peek();

            DidHandleMouseButtons = false;
            if (window == null)
            {
                return;
            }

            if (current.ScrollWheelValue != previous.ScrollWheelValue)
            {
                int scrollDistance = previous.ScrollWheelValue - current.ScrollWheelValue;
                DidHandleMouseButtons |= window.HandleScroll(scrollDistance, location, screen);
            }

            if (previous.LeftButton == ButtonState.Released
                && current.LeftButton == ButtonState.Pressed)
            {
                DidHandleMouseButtons |= window.HandlePress(location, screen);
            }
            else if (previous.LeftButton == ButtonState.Pressed
                     && current.LeftButton == ButtonState.Released)
            {
                DidHandleMouseButtons |= window.HandleRelease(location, screen);
            }

            if (previous.RightButton == ButtonState.Released
                && current.RightButton == ButtonState.Pressed)
            {
                DidHandleMouseButtons |= window.HandleAltPress(location, screen);
            }
            else if (previous.RightButton == ButtonState.Pressed
                     && current.RightButton == ButtonState.Released)
            {
                DidHandleMouseButtons |= window.HandleAltRelease(location, screen);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            foreach (var window in drawStack)
            {
                window.Draw(spriteBatch, GraphicsDevice.Viewport.Bounds);
            }
 
            if (DidHandleMouseMove)
            {
                spriteBatch.Draw(Pointer.Texture, pointerFrame, Pointer.SourceRectangle, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void UpdateDrawStack()
        {
            drawStack.Clear();

            foreach (var window in windowStack)
            {
                drawStack.Push(window);
                if (window.IsOpaque)
                {
                    break;
                }
            }
        }

        bool ShouldRepeatKey(Keys key, double deltaSeconds)
        {
            var repeat = false;

            if (IsRepeatableKey(key))
            {
                if (!keyRepeatTimers.TryGetValue(key, out double keyRepeatTimer))
                {
                    keyRepeatTimers.Add(key, 0);
                }
                keyRepeatTimer += deltaSeconds;
                while (keyRepeatTimer > KeyRepeatDelay + KeyRepeatInitialDelay)
                {
                    keyRepeatTimer -= KeyRepeatDelay;
                    repeat = true;
                }
                keyRepeatTimers[key] = keyRepeatTimer;
            }

            return repeat;
        }

        bool IsRepeatableKey(Keys key)
        {
            return !KeyExtensions.ModifierKeys.Contains(key);
        }
    }
}
