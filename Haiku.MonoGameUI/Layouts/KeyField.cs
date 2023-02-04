using Haiku.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Haiku.MonoGameUI.Layouts
{
    public delegate void KeyChangedDelegate(List<Keys> keys, string text);

    public class KeyField : TextField
    {
        public KeyChangedDelegate OnKeyChanged;
        readonly List<Keys> keys = new List<Keys>();
        readonly List<string> keyStrings = new List<string>();

        public KeyField(Rectangle frame) 
            : base(frame, null, false)
        {
        }

        internal override bool HandleKeyPress(Keys key, IEnumerable<Keys> activeModifierKeys, TextClipboarding clipboard)
        {
            var previousKeys = new List<Keys>(keys);
            var previousText = Text;

            if (key == Keys.None)
            {
                return true;
            }

            Text = "";
            keys.Clear();
            keyStrings.Clear();
            AddModifierKeys(activeModifierKeys);

            if (!key.IsModifier())
            {
                keys.Add(key);
                keyStrings.Add(key.DisplayString());
            }
            Text = string.Join("+", keyStrings);

            HandleKeyChanging(previousText, previousKeys);

            return true;
        }

        internal override bool HandleTextInput(Keys key, string keyAsString, IEnumerable<Keys> activeModifierKeys)
        {
            return false;
        }

        void AddModifierKeys(IEnumerable<Keys> activeModifierKeys)
        {
            if (activeModifierKeys.Count() > 0)
            {
                foreach (var modifier in KeyExtensions.ModifierKeys)
                {
                    if (activeModifierKeys.Contains(modifier))
                    {
                        keys.Add(modifier);
                        keyStrings.Add(modifier.DisplayString());
                    }
                }
            }
        }

        bool HandleKeyChanging(string previousText, List<Keys> previousKeys)
        {
            textInsertionIndex = Text.Length;
            HandleTextChanging(previousText);
            if (previousText != Text
                || !previousKeys.SequenceEqual(keys))
            {
                OnKeyChanged?.Invoke(keys, Text);
                iBar.IsVisible = false;
                iBarFlashTimer = 0;
                return true;
            }
            return false;
        }
    }
}
