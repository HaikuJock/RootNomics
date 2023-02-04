using Haiku.MonoGameUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace RootNomics.Mac
{
    class MousePressEventProvider : MousePressEventProviding
    {
        public Tuple<MouseState, MouseState> MouseStates { get; private set; }
        MouseState previous = Mouse.GetState();

        public void Initialize(Game game) { }

        public void OnNewFrame(bool clipCursor)
        {
            var current = Mouse.GetState();
            MouseStates = new Tuple<MouseState, MouseState>(current, previous);
            previous = current;
        }
    }
}
