using Microsoft.Xna.Framework;

namespace Haiku.MonoGameUI
{
    public static class Points
    {
        public static Point Make((int, int) pt) => new Point(pt.Item1, pt.Item2);
    }
}
