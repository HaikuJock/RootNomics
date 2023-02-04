namespace Haiku.MonoGameUI
{
    public enum ContentAlignment
    {
        Left,
        Right,
        Centre
    }

    public static class ContentAlignmentMethods
    {
        public static int HorizontalPositionIn(this ContentAlignment alignment, int parentWidth, int width)
        {
            return alignment switch
            {
                ContentAlignment.Left => 0,
                ContentAlignment.Right => parentWidth - width,
                ContentAlignment.Centre => parentWidth / 2 - width / 2,
                _ => 0,
            };
        }
    }
}
