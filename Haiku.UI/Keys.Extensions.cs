using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Haiku.UI
{
    public static class KeyExtensions
    {
        public static readonly Keys[] ModifierKeys = new[]
        {
            Keys.LeftControl,
            Keys.RightControl,
            Keys.LeftAlt,
            Keys.RightAlt,
            Keys.LeftShift,
            Keys.RightShift,
            Keys.LeftWindows,
            Keys.RightWindows,
            // Adding any more will require making modifier-bits larger than a ushort, see GetModifierKeyBits
        };
        static readonly Dictionary<Keys, string> DisplayStringOverrides = new Dictionary<Keys, string>
        {
            { Keys.D0, "0" },
            { Keys.D1, "1" },
            { Keys.D2, "2" },
            { Keys.D3, "3" },
            { Keys.D4, "4" },
            { Keys.D5, "5" },
            { Keys.D6, "6" },
            { Keys.D7, "7" },
            { Keys.D8, "8" },
            { Keys.D9, "9" },
            { Keys.LeftWindows, "Cmd" },
            { Keys.RightWindows, "Cmd" },
            { Keys.NumPad0, "NumPad0" },
            { Keys.NumPad1, "NumPad1" },
            { Keys.NumPad2, "NumPad2" },
            { Keys.NumPad3, "NumPad3" },
            { Keys.NumPad4, "NumPad4" },
            { Keys.NumPad5, "NumPad5" },
            { Keys.NumPad6, "NumPad6" },
            { Keys.NumPad7, "NumPad7" },
            { Keys.NumPad8, "NumPad8" },
            { Keys.NumPad9, "NumPad9" },
            { Keys.Multiply, "*" },
            { Keys.Add, "+" },
            { Keys.Separator, "Enter" },
            { Keys.Subtract, "-" },
            { Keys.Decimal, "." },
            { Keys.Divide, "/" },
            { Keys.LeftShift, "Shift" },
            { Keys.RightShift, "Shift" },
            { Keys.LeftControl, "Ctrl" },
            { Keys.RightControl, "Ctrl" },
            { Keys.LeftAlt, "Alt" },
            { Keys.RightAlt, "Alt" },
            { Keys.OemSemicolon, ";" },
            { Keys.OemPlus, "+" },
            { Keys.OemComma, "," },
            { Keys.OemMinus, "-" },
            { Keys.OemPeriod, "." },
            { Keys.OemQuestion, "/" },
            { Keys.OemTilde, "`" },
            { Keys.OemOpenBrackets, "[" },
            { Keys.OemPipe, "\\" },
            { Keys.OemCloseBrackets, "]" },
            { Keys.OemQuotes, "'" },
            { Keys.OemBackslash, "\\" },
        };

        static Dictionary<Keys, Keys> VisuallySimilarKeys = new Dictionary<Keys, Keys>
        {
            { Keys.Add, Keys.OemPlus },
            { Keys.Subtract, Keys.OemMinus },
            { Keys.Divide, Keys.OemQuestion },
            { Keys.RightAlt, Keys.LeftAlt },
            { Keys.RightControl, Keys.LeftControl },
            { Keys.RightShift, Keys.LeftShift },
            { Keys.RightWindows, Keys.LeftWindows },
            { Keys.OemPipe, Keys.OemBackslash },
            { Keys.Decimal, Keys.OemPeriod },
            { Keys.Separator, Keys.Enter },
        };

        public static Keys ReinterpretVisuallySimilarKey(Keys key)
        {
            if (VisuallySimilarKeys.TryGetValue(key, out Keys reInterpretedKey))
            {
                return reInterpretedKey;
            }
            return key;
        }

        public static IEnumerable<Keys> ReinterpretVisuallySimilarKeys(IEnumerable<Keys> keys)
        {
            return keys.Select(key => ReinterpretVisuallySimilarKey(key)).Distinct();
        }

        public static ushort GetModifierKeyBits(IEnumerable<Keys> currentKeys)
        {
            const int MaxModifierKeys = 2;
            ushort modifierBits = 0;
            int count = 0;

            for (int i = 0; i < ModifierKeys.Length; i++)
            {
                var modifier = ModifierKeys[i];

                if (currentKeys.Contains(modifier))
                {
                    modifierBits |= (ushort)(1 << (8 + i));
                    ++count;
                    if (count >= MaxModifierKeys)
                    {
                        break;
                    }
                }
            }

            return modifierBits;
        }

        public static ushort ModifierBits(this Keys key)
        {
            for (int i = 0; i < ModifierKeys.Length; ++i)
            {
                var modifier = ModifierKeys[i];

                if (modifier == key)
                {
                    return (ushort)(1 << (8 + i));
                }
            }
            return 0;
        }

        public static ushort Modify(this Keys modifierKey, Keys otherKey)
        {
            return (ushort)(modifierKey.ModifierBits() | (ushort)otherKey);
        }

        public static List<Keys> ModifiedKeyToList(ushort modified)
        {
            var result = new List<Keys>();
            var key = (Keys)(modified & 0x00FF);
            var modifiers = (ushort)((modified & 0xFF00) >> 8);

            for (int i = 0; i < ModifierKeys.Length; i++)
            {
                if ((modifiers & (1 << i)) != 0)
                {
                    result.Add(ModifierKeys[i]);
                }
            }

            if (key != Keys.None)
            {
                result.Add(key);
            }

            return result;
        }

        public static bool IsModifierPressed(Keys modifier, ushort pressedModifiers)
        {
            for (int i = 0; i < ModifierKeys.Length; i++)
            {
                if (ModifierKeys[i] == modifier)
                {
                    return (pressedModifiers & (1 << (8 + i))) != 0;
                }
            }
            return false;
        }

        public static bool IsSelectAll(this Keys key, IEnumerable<Keys> activeModifierKeys)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return key == Keys.A && IsCommanded(activeModifierKeys);
            }
            else
            {
                return key == Keys.A && IsControlled(activeModifierKeys);
            }
        }

        public static bool IsPaste(this Keys key, IEnumerable<Keys> activeModifierKeys)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return key == Keys.V && IsCommanded(activeModifierKeys);
            }
            else
            {
                return (key == Keys.V && IsControlled(activeModifierKeys))
                    || (key == Keys.Insert && IsShifted(activeModifierKeys));
            }
        }

        public static bool IsCopy(this Keys key, IEnumerable<Keys> activeModifierKeys)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return key == Keys.C && IsCommanded(activeModifierKeys);
            }
            else
            {
                return (key == Keys.C || key == Keys.Insert) && IsControlled(activeModifierKeys);
            }
        }

        public static bool IsCut(this Keys key, IEnumerable<Keys> activeModifierKeys)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return key == Keys.X && IsCommanded(activeModifierKeys);
            }
            else
            {
                return (key == Keys.X && IsControlled(activeModifierKeys))
                    || (key == Keys.Delete && IsShifted(activeModifierKeys));
            }
        }

        public static bool IsHome(this Keys key, IEnumerable<Keys> activeModifierKeys)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return key == Keys.Left && IsCommanded(activeModifierKeys);
            }
            return key == Keys.Home;
        }

        public static bool IsEnd(this Keys key, IEnumerable<Keys> activeModifierKeys)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return key == Keys.Right && IsCommanded(activeModifierKeys);
            }
            return key == Keys.End;
        }

        public static bool IsShifted(IEnumerable<Keys> activeModifierKeys)
        {
            return activeModifierKeys.Contains(Keys.LeftShift)
                || activeModifierKeys.Contains(Keys.RightShift);
        }

        public static bool IsControlled(IEnumerable<Keys> activeModifierKeys)
        {
            return activeModifierKeys.Contains(Keys.RightControl)
                || activeModifierKeys.Contains(Keys.LeftControl);
        }

        public static bool IsCommanded(IEnumerable<Keys> activeModifierKeys)
        {
            return activeModifierKeys.Contains(Keys.LeftWindows)
                || activeModifierKeys.Contains(Keys.RightWindows);
        }

        public static string DisplayString(this Keys key)
        {
            if (DisplayStringOverrides.TryGetValue(key, out string overrideString))
            {
                return overrideString;
            }
            else
            {
                var enumString = key.ToString();
                return Regex.Replace(enumString, "([A-Z])", "-$1", RegexOptions.Compiled).Trim('-');
            }
        }

        public static bool IsModifier(this Keys key)
        {
            return ModifierKeys.Contains(key);
        }
    }
}
