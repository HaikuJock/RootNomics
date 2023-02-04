using Haiku.MonoGameUI;
using TextCopy;

namespace RootNomicsGame
{
    class TextClipboard : TextClipboarding
    {
        public string GetText()
        {
            return Clipboard.GetText();
        }

        public void SetText(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
