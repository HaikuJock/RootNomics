using Haiku.MonoGameUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RootNomics.Win
{
    class MousePressEventProvider : MousePressEventProviding
    {
        [DllImport("user32.dll")]
        static extern void ClipCursor(ref Rectangle rect);

        public Tuple<MouseState, MouseState> MouseStates { get; private set; }
        MouseState previous = Mouse.GetState();
        Game game;
        bool wasClippingCursor;

        public void Initialize(Game game)
        {
            this.game = game;
        }

        public void OnNewFrame(bool clipCursor)
        {
            UpdateClipCursor(game.IsActive && clipCursor);

            var current = Mouse.GetState();
            MouseStates = new Tuple<MouseState, MouseState>(current, previous);
            previous = current;
        }

        void UpdateClipCursor(bool clipCursor)
        {
            if (wasClippingCursor != clipCursor)
            {
                if (clipCursor)
                {
                    Rectangle rect = game.Window.ClientBounds;
                    rect.Width += rect.X;
                    rect.Height += rect.Y;

                    rect.Y -= 31;   // Include the window bar so user can minimize and close the app

                    ClipCursor(ref rect);
                }
                else
                {
                    ClipCursor(ref Unsafe.NullRef<Rectangle>());
                }
                wasClippingCursor = clipCursor;
            }
        }
    }
}
