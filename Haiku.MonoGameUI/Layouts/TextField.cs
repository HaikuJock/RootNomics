using Haiku.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public delegate void TextChangedDelegate(string text);

    public class TextField : Control
    {
        const double IBarFlashOnDuration = 0.76;
        const double IBarFlashOffDuration = 0.47;
        const int LabelInset = 9;
        const int DefaultMaxLength = 255;
        const int NothingSelected = -1;
        const int NoSelectionEndpoint = -1;
        const double DragOutsideRate = 0.8;
        public static SpriteFrame Background;
        public static SpriteFrame IBar;

        public string Text { get; protected set; } = "";
        public int MaxLength { get; set; } = DefaultMaxLength;
        public string AcceptedCharacters { get; set; }
        public TextChangedDelegate OnTextChanged;
        readonly Action<string> onDone;
        readonly Panel labelContainer;
        readonly Label label;
        readonly Panel selectionHighlight;
        protected readonly Panel iBar;
        int startSelectionIndex;
        int endSelectionIndex;
        int textSelectionIndex;
        protected double iBarFlashTimer;
        double dragOutsideSpeed;
        double dragOutsideMovement;
        protected int textInsertionIndex;

        public TextField(Point size, Action<string> onDone = null, bool clipsChildren = true)
            : this(new Rectangle(Point.Zero, size), onDone, clipsChildren)
        {
        }

        public TextField(Rectangle frame, Action<string> onDone = null, bool clipsChildren = true)
            : base(frame)
        {
            this.onDone = onDone ?? DefaultDone;
            IsFocusable = true;
            IsDraggingEnabled = true;
            BackgroundColor = Color.Transparent;
            var background = new Panel(frame)
            {
                Texture = Background,
                IsInteractionEnabled = false,
            };

            var iBarOffset = IBar.SourceRectangle.Width / 2;
            iBar = new Panel(LabelInset - iBarOffset, 0, IBar.SourceRectangle.Width, IBar.SourceRectangle.Height)
            {
                Texture = IBar,
                IsVisible = false,
                IsInteractionEnabled = false
            };

            var maxLabelWidth = frame.Width - 2 * LabelInset;
            labelContainer = new Panel(LabelInset, 0, maxLabelWidth, frame.Height)
            {
                IsClippingChildren = clipsChildren,
                IsInteractionEnabled = false,
                BackgroundColor = Color.Transparent
            };
            label = new Label
            {
                IsInteractionEnabled = false
            };
            if (frame.Height >= 55)
            {
                label.Font = TitleFont;
            }
            else if (frame.Height >= 44)
            {
                label.Font = MainFont;
            }
            else if (frame.Height >= 33)
            {
                label.Font = BodyFont;
            }
            else
            {
                label.Font = LabelFont;
            }
            var containerHeight = (int)label.Font.MeasureString("X").Y;
            labelContainer.Frame = new Rectangle(labelContainer.Frame.X, labelContainer.Frame.Y, labelContainer.Frame.Width, containerHeight);
            label.Frame = new Rectangle(0, 0, maxLabelWidth, labelContainer.Frame.Height);
            selectionHighlight = new Panel(0, 0, 0, labelContainer.Frame.Height)
            {
                BackgroundColor = Color.CornflowerBlue,
                IsInteractionEnabled = false
            };

            labelContainer.AddChild(selectionHighlight);
            labelContainer.AddChild(label);
            AddChild(background);
            AddChild(labelContainer);
            labelContainer.CenterYInParent();
            AddChild(iBar);
            iBar.CenterYInParent();
            startSelectionIndex = NothingSelected;
            textSelectionIndex = NoSelectionEndpoint;
        }

        public void SetText(string text)
        {
            if (text.Length > MaxLength)
            {
                Text = text.Substring(0, MaxLength);
            }
            else
            {
                Text = text;
            }
            SetLabelText();
            textInsertionIndex = text.Length;
            startSelectionIndex = NothingSelected;
            UpdateChildrenPositions();
        }

        public void SelectAll()
        {
            SelectAllInternal();
            UpdateSelectionHighlight();
        }

        public override bool OnCursorDrag(Point point, Rectangle container)
        {
            if (startSelectionIndex == NothingSelected)
            {
                startSelectionIndex = textInsertionIndex;
            }
            if (IsWithinLabelContainer(point))
            {
                var worldFrame = WorldFrame(container);
                MoveIBarAndEndSelection(point, worldFrame);
            }
            else
            {
                UpdateDragOutsideSpeed(point);
            }
            return true;
        }

        public override bool OnCursorDragOutside(Point point)
        {
            if (IsWithinLabelContainer(point))
            {
                var worldFrame = CalculateWorldFrame();
                MoveIBarAndEndSelection(point, worldFrame);
            }
            else
            {
                UpdateDragOutsideSpeed(point);
            }
            return true;
        }

        public override void OnDragEnd(Point point, Rectangle container)
        {
            dragOutsideSpeed = 0;
            dragOutsideMovement = 0;
        }

        internal override bool HandleKeyPress(Keys key, IEnumerable<Keys> activeModifierKeys, TextClipboarding clipboard)
        {
            var previousText = Text;

            if (key.IsSelectAll(activeModifierKeys))
            {
                SelectAllInternal();
            }
            else if (key.IsHome(activeModifierKeys))
            {
                MoveHome(activeModifierKeys);
            }
            else if (key.IsEnd(activeModifierKeys))
            {
                MoveEnd(activeModifierKeys);
            }
            else if (key == Keys.Left)
            {
                MoveLeft(activeModifierKeys);
            }
            else if (key == Keys.Right)
            {
                MoveRight(activeModifierKeys);
            }
            else if (key.IsPaste(activeModifierKeys))
            {
                Paste(clipboard);
            }
            else if (key.IsCopy(activeModifierKeys))
            {
                Copy(clipboard);
            }
            else if (key.IsCut(activeModifierKeys))
            {
                Cut(clipboard);
            }

            HandleTextChanging(previousText);

            return IsHandledKeyPress(key, activeModifierKeys);
        }

        internal override bool HandleTextInput(Keys key, string keyAsString, IEnumerable<Keys> activeModifierKeys)
        {
            var previousText = Text;

            keyAsString = AcceptedText(keyAsString);

            if (key == Keys.Back)
            {
                DeleteBack();
            }
            else if (key == Keys.Delete)
            {
                DeleteInPlace();
            }
            else if (key == Keys.Enter)
            {
                onDone(Text);
            }
            else if (keyAsString.Length == 1
                     && !char.IsControl(keyAsString[0]))
            {
                InsertText(keyAsString);
            }

            HandleTextChanging(previousText);

            return IsHandledTextInput(key, keyAsString);
        }

        internal override void UpdateFocused(double deltaSeconds)
        {
            iBarFlashTimer += deltaSeconds;
            if (startSelectionIndex == NothingSelected)
            {
                if (iBar.IsVisible
                    && iBarFlashTimer > IBarFlashOnDuration)
                {
                    iBar.IsVisible = false;
                    iBarFlashTimer = 0;
                }
                else if (!iBar.IsVisible
                         && iBarFlashTimer > IBarFlashOffDuration)
                {
                    iBar.IsVisible = true;
                    iBarFlashTimer = 0;
                }
            }
            else
            {
                iBar.IsVisible = false;
            }
            if (dragOutsideSpeed != 0)
            {
                dragOutsideMovement += dragOutsideSpeed * deltaSeconds;
                if (Math.Abs(dragOutsideMovement) >= 1)
                {
                    var dragDirection = dragOutsideMovement > 0 ? 1 : -1;

                    dragOutsideMovement = 0;
                    endSelectionIndex = MathHelper.Clamp(endSelectionIndex + dragDirection, 0, Text.Length);
                    MoveIBarToIndex(endSelectionIndex);
                    iBar.IsVisible = false;
                }
            }
        }

        internal override void OnLoseFocus()
        {
            iBar.IsVisible = false;
            iBarFlashTimer = 0;
            dragOutsideMovement = 0;
            dragOutsideSpeed = 0;
        }

        internal override void OnGainFocus()
        {
            iBar.IsVisible = true;
            iBarFlashTimer = 0;
        }

        protected virtual void SetLabelText(string text)
        {
            label.Text = text;
        }

        protected virtual void Copy(TextClipboarding clipboard)
        {
            if (startSelectionIndex == NothingSelected)
            {
                clipboard.SetText(Text);
            }
            else
            {
                var startIndex = Math.Min(startSelectionIndex, endSelectionIndex);
                var endIndex = Math.Max(startSelectionIndex, endSelectionIndex);
                var selectionLength = endIndex - startIndex;
                var selectedString = Text.Substring(startIndex, selectionLength);

                clipboard.SetText(selectedString);
            }
        }

        protected virtual void Cut(TextClipboarding clipboard)
        {
            if (startSelectionIndex != NothingSelected)
            {
                var startIndex = Math.Min(startSelectionIndex, endSelectionIndex);
                var endIndex = Math.Max(startSelectionIndex, endSelectionIndex);
                var selectionLength = endIndex - startIndex;
                var selectedString = Text.Substring(startIndex, selectionLength);

                clipboard.SetText(selectedString);
                Text = Text.Remove(startIndex, selectionLength);
                startSelectionIndex = NothingSelected;
                textSelectionIndex = NoSelectionEndpoint;
                textInsertionIndex = startIndex;
            }
        }

        protected override bool OnPress(Point point, Rectangle container)
        {
            startSelectionIndex = NothingSelected;
            textSelectionIndex = NoSelectionEndpoint;
            MoveIBar(point, WorldFrame(container));
            ParentWindow?.GiveFocusTo(this);
            return base.OnPress(point, container);
        }

        protected void SetLabelText()
        {
            SetLabelText(Text);
        }

        protected void UpdateChildrenPositions()
        {
            label.SizeToFit();
            var textInsertionX = CalculateTextInsertionX();
            var fieldX = LabelInset + label.Frame.X + textInsertionX;

            if (fieldX > labelContainer.Frame.Right)
            {
                var distanceBeyondContainer = labelContainer.Frame.Right - fieldX;
                var x = label.Frame.X + distanceBeyondContainer;
                label.Frame = new Rectangle(x, label.Frame.Y, label.Frame.Width, label.Frame.Height);
                fieldX = labelContainer.Frame.Right;
            }
            else if (fieldX < LabelInset)
            {
                var distanceBeforeContainer = LabelInset - fieldX;
                var x = label.Frame.X + distanceBeforeContainer;
                label.Frame = new Rectangle(x, label.Frame.Y, label.Frame.Width, label.Frame.Height);
                fieldX = LabelInset;
            }

            var iBarOffset = IBar.SourceRectangle.Width / 2;
            iBar.Frame = new Rectangle(fieldX - iBarOffset, iBar.Frame.Y, iBar.Frame.Width, iBar.Frame.Height);

            UpdateSelectionHighlight();
        }

        protected void HandleTextChanging(string previousText)
        {
            SetLabelText();
            if (previousText != Text)
            {
                iBar.IsVisible = true;
                iBarFlashTimer = 0;
                OnTextChanged?.Invoke(Text);
                dragOutsideMovement = 0;
                dragOutsideSpeed = 0;
                startSelectionIndex = NothingSelected;
                textSelectionIndex = NoSelectionEndpoint;
            }
            if (startSelectionIndex != NothingSelected
                && startSelectionIndex == endSelectionIndex)
            {
                textInsertionIndex = startSelectionIndex;
                startSelectionIndex = NothingSelected;
                textSelectionIndex = NoSelectionEndpoint;
            }
            UpdateChildrenPositions();
        }

        void DefaultDone(string text)
        {
            ParentWindow?.AdvanceFocus();
        }

        bool IsWithinLabelContainer(Point point)
        {
            var worldFrame = labelContainer.CalculateWorldFrame();

            return point.X >= worldFrame.Left
                && point.X <= worldFrame.Right;
        }

        void MoveIBarAndEndSelection(Point point, Rectangle worldFrame)
        {
            endSelectionIndex = PointToInsertionIndex(point, worldFrame);
            if (endSelectionIndex == Text.Length - 1
                && IsToRightOfLabelContainer(point))
            {
                endSelectionIndex = Text.Length;
            }
            MoveIBarToIndex(endSelectionIndex);
            if (startSelectionIndex != NothingSelected
                && startSelectionIndex == endSelectionIndex)
            {
                startSelectionIndex = NothingSelected;
                textSelectionIndex = NoSelectionEndpoint;
                iBar.IsVisible = true;
            }
            else
            {
                iBar.IsVisible = false;
            }
            dragOutsideSpeed = 0;
            dragOutsideMovement = 0;
        }

        void UpdateDragOutsideSpeed(Point point)
        {
            var worldFrame = labelContainer.CalculateWorldFrame();

            if (point.X < worldFrame.Left)
            {
                dragOutsideSpeed = (point.X - worldFrame.Left) * DragOutsideRate;
            }
            else if (point.X > worldFrame.Right)
            {
                dragOutsideSpeed = (point.X - worldFrame.Right) * DragOutsideRate;
            }
            else
            {
                dragOutsideSpeed = 0;
                dragOutsideMovement = 0;
            }
        }

        void SelectAllInternal()
        {
            if (Text.Length > 0)
            {
                startSelectionIndex = 0;
                endSelectionIndex = Text.Length;
                textInsertionIndex = endSelectionIndex;
            }
        }

        void MoveHome(IEnumerable<Keys> activeModifierKeys)
        {
            if (KeyExtensions.IsShifted(activeModifierKeys))
            {
                if (startSelectionIndex == NothingSelected)
                {
                    if (textInsertionIndex != 0)
                    {
                        startSelectionIndex = 0;
                        endSelectionIndex = textInsertionIndex;
                    }
                }
                else
                {
                    endSelectionIndex = Math.Max(startSelectionIndex, endSelectionIndex);
                    startSelectionIndex = 0;
                }
            }
            else
            {
                startSelectionIndex = NothingSelected;
            }
            textInsertionIndex = 0;
            textSelectionIndex = NoSelectionEndpoint;
        }

        void MoveEnd(IEnumerable<Keys> activeModifierKeys)
        {
            if (KeyExtensions.IsShifted(activeModifierKeys))
            {
                if (startSelectionIndex == NothingSelected)
                {
                    if (textInsertionIndex != Text.Length)
                    {
                        startSelectionIndex = textInsertionIndex;
                        endSelectionIndex = Text.Length;
                    }
                }
                else
                {
                    startSelectionIndex = Math.Min(startSelectionIndex, endSelectionIndex);
                    endSelectionIndex = Text.Length;
                }
            }
            else
            {
                startSelectionIndex = NothingSelected;
            }
            textInsertionIndex = Text.Length;
            textSelectionIndex = NoSelectionEndpoint;
        }

        void MoveLeft(IEnumerable<Keys> activeModifierKeys)
        {
            if (KeyExtensions.IsShifted(activeModifierKeys))
            {
                if (startSelectionIndex == NothingSelected)
                {
                    if (textInsertionIndex > 0)
                    {
                        startSelectionIndex = textInsertionIndex;
                        endSelectionIndex = startSelectionIndex - 1;
                        textInsertionIndex--;
                        textSelectionIndex = textInsertionIndex;
                    }
                }
                else
                {
                    if (textSelectionIndex == NoSelectionEndpoint)
                    {
                        var start = Math.Min(startSelectionIndex, endSelectionIndex);
                        var end = Math.Max(startSelectionIndex, endSelectionIndex);

                        if (start > 0)
                        {
                            --start;
                            textSelectionIndex = start;
                        }
                        startSelectionIndex = start;
                        endSelectionIndex = end;
                        textInsertionIndex = start;
                    }
                    else
                    {
                        if (textSelectionIndex > 0)
                        {
                            if (textSelectionIndex == startSelectionIndex)
                            {
                                --textSelectionIndex;
                                --startSelectionIndex;
                            }
                            else if (textSelectionIndex == endSelectionIndex)
                            {
                                --textSelectionIndex;
                                --endSelectionIndex;
                            }
                        }
                    }
                }
            }
            else
            {
                if (startSelectionIndex == NothingSelected)
                {
                    if (textInsertionIndex > 0)
                    {
                        textInsertionIndex--;
                    }
                }
                else
                {
                    textInsertionIndex = Math.Min(startSelectionIndex, endSelectionIndex);
                    startSelectionIndex = NothingSelected;
                    textSelectionIndex = NoSelectionEndpoint;
                }
            }
        }

        void MoveRight(IEnumerable<Keys> activeModifierKeys)
        {
            if (KeyExtensions.IsShifted(activeModifierKeys))
            {
                if (startSelectionIndex == NothingSelected)
                {
                    if (textInsertionIndex < Text.Length)
                    {
                        startSelectionIndex = textInsertionIndex;
                        endSelectionIndex = startSelectionIndex + 1;
                        textInsertionIndex++;
                        textSelectionIndex = textInsertionIndex;
                    }
                }
                else
                {
                    if (textSelectionIndex == NoSelectionEndpoint)
                    {
                        var start = Math.Min(startSelectionIndex, endSelectionIndex);
                        var end = Math.Max(startSelectionIndex, endSelectionIndex);

                        if (end < Text.Length)
                        {
                            ++end;
                            textSelectionIndex = end;
                        }
                        startSelectionIndex = start;
                        endSelectionIndex = end;
                        textInsertionIndex = end;
                    }
                    else
                    {
                        if (textSelectionIndex < Text.Length)
                        {
                            if (textSelectionIndex == startSelectionIndex)
                            {
                                ++textSelectionIndex;
                                ++startSelectionIndex;
                            }
                            else if (textSelectionIndex == endSelectionIndex)
                            {
                                ++textSelectionIndex;
                                ++endSelectionIndex;
                            }
                        }
                    }
                }
            }
            else
            {
                if (startSelectionIndex == NothingSelected)
                {
                    if (textInsertionIndex < Text.Length)
                    {
                        textInsertionIndex++;
                    }
                }
                else
                {
                    textInsertionIndex = Math.Max(startSelectionIndex, endSelectionIndex);
                    startSelectionIndex = NothingSelected;
                    textSelectionIndex = NoSelectionEndpoint;
                }
            }
        }

        void Paste(TextClipboarding clipboard)
        {
            var clipboardText = AcceptedText(clipboard.GetText());

            if (!string.IsNullOrEmpty(clipboardText))
            {
                if (startSelectionIndex == NothingSelected)
                {
                    if (clipboardText.Length + Text.Length < MaxLength)
                    {
                        Text = Text.Insert(textInsertionIndex, clipboardText);
                        textInsertionIndex += clipboardText.Length;
                    }
                }
                else
                {
                    var startIndex = Math.Min(startSelectionIndex, endSelectionIndex);
                    var endIndex = Math.Max(startSelectionIndex, endSelectionIndex);
                    var selectionLength = endIndex - startIndex;

                    if (clipboardText.Length - selectionLength + Text.Length < MaxLength)
                    {
                        Text = Text.Remove(startIndex, selectionLength);
                        Text = Text.Insert(startIndex, clipboardText);
                        textInsertionIndex = startIndex + clipboardText.Length;
                        startSelectionIndex = NothingSelected;
                        textSelectionIndex = NoSelectionEndpoint;
                    }
                }
            }
        }

        string AcceptedText(string text)
        {
            if (AcceptedCharacters == null)
            {
                return text;
            }
            else
            {
                var stringBuilder = new StringBuilder(text.Length);

                foreach (var character in text)
                {
                    if (AcceptedCharacters.Contains(character))
                    {
                        stringBuilder.Append(character);
                    }
                }

                return stringBuilder.ToString();
            }
        }

        bool IsHandledKeyPress(Keys key, IEnumerable<Keys> activeModifierKeys)
        {
            return key == Keys.Left
                || key == Keys.Right
                || key.IsCut(activeModifierKeys)
                || key.IsCopy(activeModifierKeys)
                || key.IsPaste(activeModifierKeys)
                || key.IsEnd(activeModifierKeys)
                || key.IsHome(activeModifierKeys)
                || key.IsSelectAll(activeModifierKeys);
        }

        void DeleteBack()
        {
            if (startSelectionIndex == NothingSelected)
            {
                if (Text.Length > 0
                    && textInsertionIndex > 0)
                {
                    Text = Text.Remove(textInsertionIndex - 1, 1);
                    textInsertionIndex--;
                }
            }
            else
            {
                RemoveSelectedText();
            }
        }

        void DeleteInPlace()
        {
            if (startSelectionIndex == NothingSelected)
            {
                if (Text.Length > 0
                    && textInsertionIndex < Text.Length)
                {
                    Text = Text.Remove(textInsertionIndex, 1);
                }
            }
            else
            {
                RemoveSelectedText();
            }
        }

        void InsertText(string keyAsString)
        {
            if (startSelectionIndex == NothingSelected)
            {
                if (Text.Length < MaxLength)
                {
                    Text = Text.Insert(textInsertionIndex, keyAsString);
                    ++textInsertionIndex;
                }
            }
            else
            {
                var startIndex = Math.Min(startSelectionIndex, endSelectionIndex);
                var endIndex = Math.Max(startSelectionIndex, endSelectionIndex);
                var selectionLength = endIndex - startIndex;

                if (Text.Length - selectionLength + keyAsString.Length < MaxLength)
                {
                    Text = Text.Remove(startIndex, selectionLength);
                    textInsertionIndex = startIndex;
                    Text = Text.Insert(textInsertionIndex, keyAsString);
                    ++textInsertionIndex;
                    startSelectionIndex = NothingSelected;
                    textSelectionIndex = NoSelectionEndpoint;
                }
            }
        }

        bool IsHandledTextInput(Keys key, string keyAsString)
        {
            return key == Keys.Back
                || key == Keys.Delete
                || key == Keys.Enter
                || (keyAsString.Length == 1 && key != Keys.Escape);
        }

        void MoveIBarToIndex(int insertionIndex)
        {
            if (textInsertionIndex != insertionIndex)
            {
                textInsertionIndex = insertionIndex;
                UpdateChildrenPositions();
                iBar.IsVisible = true;
                iBarFlashTimer = 0;
            }
            else
            {
                UpdateSelectionHighlight();
            }
        }

        void MoveIBar(Point point, Rectangle container)
        {
            MoveIBarToIndex(PointToInsertionIndex(point, container));
        }

        int CalculateTextInsertionX()
        {
            return IndexToX(textInsertionIndex);
        }

        void UpdateSelectionHighlight()
        {
            if (startSelectionIndex == NothingSelected)
            {
                selectionHighlight.IsVisible = false;
            }
            else
            {
                var startSelectionX = IndexToX(startSelectionIndex);
                var endSelectionX = IndexToX(endSelectionIndex);
                var minX = Math.Min(startSelectionX, endSelectionX);
                var maxX = Math.Max(startSelectionX, endSelectionX);

                if (minX == maxX)
                {
                    selectionHighlight.IsVisible = false;
                }
                else
                {
                    selectionHighlight.IsVisible = true;
                    selectionHighlight.Frame = new Rectangle(
                        minX + label.Frame.X, 0,
                        maxX - minX, selectionHighlight.Frame.Height
                        );
                }
            }
        }

        int PointToInsertionIndex(Point point, Rectangle worldFrame)
        {
            var visibleTextStart = -label.Frame.X;
            var localX = (point - worldFrame.Location).X - LabelInset;

            localX = MathHelper.Clamp(localX, 0, labelContainer.Frame.Width);
            var pointInText = visibleTextStart + localX;

            var previousIndex = 0;
            var index = 0;
            var exit =
                localX < labelContainer.Frame.Width / 2
                ? 1
                : 0;

            while (pointInText >= exit)
            {
                if (index >= label.Text.Length)
                {
                    previousIndex = label.Text.Length;
                    break;
                }
                var character = label.Text.Substring(index, 1);
                var characterWidth = (int)label.Font.MeasureString(character).X;

                pointInText -= characterWidth;
                visibleTextStart -= characterWidth;

                previousIndex = index;
                ++index;
            }
            if (visibleTextStart >= pointInText)
            {
                return index;
            }
            else
            {
                return previousIndex;
            }
        }

        bool IsToRightOfLabelContainer(Point point)
        {
            var worldFrame = labelContainer.CalculateWorldFrame();
            return point.X >= worldFrame.Right;
        }

        void RemoveSelectedText()
        {
            var startIndex = Math.Min(startSelectionIndex, endSelectionIndex);
            var endIndex = Math.Max(startSelectionIndex, endSelectionIndex);
            var selectionLength = endIndex - startIndex;

            Text = Text.Remove(startIndex, selectionLength);
            textInsertionIndex = startIndex;
            startSelectionIndex = NothingSelected;
            textSelectionIndex = NoSelectionEndpoint;
        }

        int IndexToX(int index)
        {
            var x = 0;

            if (label.Text.Length > 0)
            {
                var textBefore = label.Text.Substring(0, index);
                var textWidth = label.Font.MeasureString(textBefore).X;

                x = (int)Math.Ceiling(textWidth);
            }

            return x;
        }
    }
}
