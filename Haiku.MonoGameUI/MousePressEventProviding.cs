using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Haiku.MonoGameUI
{
    public interface MousePressEventProviding
    {
        Tuple<MouseState, MouseState> MouseStates { get; }

        void Initialize(Game game);
        void OnNewFrame(bool clipCursor);
    }
}
