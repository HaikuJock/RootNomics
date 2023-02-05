using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Haiku.MonoGameUI.Layouts
{
    public class LinearLayout : Layout
    {
        public LinearLayout(Orientation orientation, int spacing = 0, int padding = 0)
            : this(Rectangle.Empty, orientation, spacing, padding)
        {
        }

        public LinearLayout(Rectangle frame, Orientation orientation, int spacing = 0, int padding = 0)
            : this(frame, orientation, spacing, padding, padding, Direction.Forward)
        {
        }

        public LinearLayout(Rectangle frame, Orientation orientation, int spacing, int startPadding, int endPadding, Direction direction)
            : base(frame, new LinearLayoutStrategy(orientation, spacing, startPadding, endPadding, direction))
        {
        }

        public static LinearLayout ForText(
            string text, 
            SpriteFont font, 
            int width, 
            ContentAlignment alignment = ContentAlignment.Centre, 
            int spacing = 2)
        {
            var textLayout = new LinearLayout(Rectangle.Empty, Orientation.Vertical, spacing);
            var layedOutText = LayedOutText(text, font, width, alignment);

            textLayout.AddChildren(layedOutText);

            if (alignment == ContentAlignment.Centre)
            {
                foreach (var child in layedOutText)
                {
                    child.CenterXInParent();
                }
            }

            return textLayout;
        }

        public static List<Layout> LayedOutText(
            string text,
            SpriteFont font,
            int width,
            ContentAlignment alignment = ContentAlignment.Centre)
        {
            return LayedOutText(text, font, width, () => new Label(), alignment);
        }

        public static List<Layout> LayedOutText(
            string text,
            SpriteFont font,
            int width,
            Func<Label> labelCreator,
            ContentAlignment alignment = ContentAlignment.Centre)
        {
            var result = new List<Layout>();
            var lines = text.Split('\n');
            var fontSize = font.MeasureString(" ");
            float spaceWidth = fontSize.X;

            foreach (var line in lines)
            {
                var words = line.Split(' ');
                var wordCount = words.Length;
                var displayLine = "";
                float displayLineWidth = 0f;

                if (wordCount == 0)
                {
                    result.Add(CreateLine(displayLine, font, width, alignment, labelCreator));
                    continue;
                }
                int wordIndex = 0;

                foreach (var word in words)
                {
                    var wordWidth = font.MeasureString(word).X;

                    if ((int)(displayLineWidth + wordWidth) >= width)
                    {
                        displayLine = displayLine.Trim();
                        if (wordIndex == wordCount - 1
                            && wordCount > 1)
                        {
                            var previousWord = words[wordIndex - 1];

                            displayLine = displayLine.Substring(0, displayLine.Length - previousWord.Length);
                            displayLine = displayLine.Trim();
                            result.Add(CreateLine(displayLine, font, width, alignment, labelCreator));
                            displayLine = previousWord + " " + word;
                            result.Add(CreateLine(displayLine, font, width, alignment, labelCreator));
                            displayLine = "";
                            displayLineWidth = 0f;
                            break;
                        }
                        result.Add(CreateLine(displayLine, font, width, alignment, labelCreator));
                        displayLine = "";
                        displayLineWidth = 0f;
                    }
                    ++wordIndex;
                    displayLine += word + " ";
                    displayLineWidth += wordWidth + spaceWidth;
                }
                displayLine = displayLine.Trim();
                if (displayLine.Length > 0)
                {
                    result.Add(CreateLine(displayLine, font, width, alignment, labelCreator));
                }
                else
                {
                    var layout = CreateLine(displayLine, font, width, alignment, labelCreator);
                    layout.Frame = new Rectangle(layout.Frame.X, layout.Frame.Y, layout.Frame.Width, (int)Math.Ceiling(fontSize.Y));
                    result.Add(layout);
                }
            }

            return result;
        }

        static Layout CreateLine(
            string displayLine, 
            SpriteFont font, 
            int width, 
            ContentAlignment alignment,
            Func<Label> labelCreator)
        {
            var label = labelCreator();
            label.Text = displayLine;
            var lineLabel = new TextPanel(new Rectangle(0, 0, width, 0), label, alignment)
            {
                Font = font,
                TextColor = new Color(0xF5F5DC),
            };
            lineLabel.SizeToFit();

            return lineLabel;
        }
    }
}
