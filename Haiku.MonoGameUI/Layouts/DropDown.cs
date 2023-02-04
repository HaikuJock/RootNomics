using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public delegate void DropDownChangedHandler(int previousIndex, int selectedIndex);

    public class DropDown : Control
    {
        public static SpriteFrame ButtonNormal;
        public static SpriteFrame ButtonActive;
        public static SpriteFrame ButtonSelected;
        public int SelectedIndex { get; private set; }
        public DropDownChangedHandler OnSelectionChanged;
        readonly int indent;
        readonly IReadOnlyList<string> items;
        readonly Layout dropDownContainer;
        readonly SpriteFont font;
        readonly List<DropDownCell> cells;
        readonly DropDownButton selectedItem;
        readonly InteractionListener pressCatcher;
        ScrollPanel cellsContainer;

        public DropDown(Rectangle frame, int indent, IReadOnlyList<string> items, Layout dropDownContainer)
            : this(frame, indent, items, dropDownContainer, BodyFont)
        {
        }

        public DropDown(Rectangle frame, int indent, IReadOnlyList<string> items, Layout dropDownContainer, SpriteFont font)
            : base(frame, new FormLayoutStrategy())
        {
            this.indent = indent;
            this.items = items;
            this.dropDownContainer = dropDownContainer;
            this.font = font;
            pressCatcher = new InteractionListener(new Rectangle(0, 0, dropDownContainer.Frame.Width, dropDownContainer.Frame.Height));
            pressCatcher.OnPressed += OnBackgroundInteraction;
            cells = new List<DropDownCell>(items.Count);
            BackgroundColor = Color.Transparent;
            SelectedIndex = 0;

            selectedItem = new DropDownButton(frame, indent)
            {
                Action = ToggleDropDown,
            };
            selectedItem.SetBackground(ButtonNormal, ButtonActive, ButtonSelected);
            SetItemText(selectedItem, items.Count > 0 ? items[0] : "-");

            if (items.Count <= 1)
            {
                selectedItem.Disable();
            }

            AddChild(selectedItem);
        }

        public void SetSelected(string title)
        {
            var indexOf = -1;

            for (int i = 0; i < items.Count; i++)
            {
                if (title == items[i])
                {
                    indexOf = i;
                    break;
                }
            }
            SetSelected(indexOf);
        }

        public void SetSelected(int index)
        {
            if (index >= 0
                && index < items.Count)
            {
                if (SelectedIndex < cells.Count)
                {
                    cells[SelectedIndex].SetSelected(false);
                }
                SelectedIndex = index;
                SetItemText(selectedItem, items[SelectedIndex]);
                HideDropDown();

                if (SelectedIndex < cells.Count)
                {
                    cells[SelectedIndex].SetSelected(true);
                }
            }
        }

        public void HideDropDown()
        {
            pressCatcher?.RemoveFromParent();
        }

        void OnBackgroundInteraction(Point point, Rectangle container)
        {
            var worldFrame = CalculateWorldFrame();
            var worldPoint = container.Location + point;

            if (!worldFrame.Contains(worldPoint))
            {
                HideDropDown();
            }
        }

        void SetItemText(Button button, string text)
        {
            var vsize = font.MeasureString(text);
            var size = new Point((int)vsize.X, (int)vsize.Y);
            var label = new Label(0, 0, size.X, size.Y)
            {
                Font = font,
                Text = text,
                TextColor = Color.Black,
                BackgroundColor = Color.Transparent
            };
            var x = ContentAlignment.Left.HorizontalPositionIn(button.Frame.Width, size.X) + indent;
            var y = button.Frame.Height / 2 - size.Y / 2;

            label.Frame = new Rectangle(x, y, size.X, size.Y);
            button.SetForegroundPanel(label);
        }

        void ToggleDropDown(object button)
        {
            if (pressCatcher?.Parent == null)
            {
                ShowDropDown();
            }
            else
            {
                HideDropDown();
            }
        }

        void SelectItem(object button)
        {
            var btn = button as DropDownCell;
            var previousIndex = SelectedIndex;

            SetSelected(btn.Tag);
            if (SelectedIndex != previousIndex
                && OnSelectionChanged != null)
            {
                OnSelectionChanged(previousIndex, SelectedIndex);
            }
        }

        void ShowDropDown()
        {
            var containerWorldFrame = dropDownContainer.CalculateWorldFrame();
            var worldFrame = CalculateWorldFrame();
            var availableHeightBelow = containerWorldFrame.Bottom - worldFrame.Bottom;
            var availableHeightAbove = worldFrame.Top;
            var itemsHeight = items.Count * Frame.Height;
            var availableHeight = availableHeightBelow;
            var containerHeight = itemsHeight;
            var positionBelow = new Point(worldFrame.X - containerWorldFrame.X, (worldFrame.Y - containerWorldFrame.Y) + worldFrame.Height);
            var showAbove = false;

            if (itemsHeight > availableHeightBelow
                && availableHeightAbove > availableHeightBelow)
            {
                availableHeight = availableHeightAbove;
                showAbove = true;
            }
            if (itemsHeight > availableHeight)
            {
                containerHeight = availableHeight;
            }

            var positionAbove = new Point(worldFrame.X - containerWorldFrame.X, worldFrame.Top - containerHeight);
            var position = showAbove ? positionAbove : positionBelow;

            if (cellsContainer == null
                || cellsContainer.Size.Y != containerHeight)
            {
                CreateDropDownCells(itemsHeight, containerHeight);
            }

            var dropDownFrame = new Rectangle(position, cellsContainer.Size);

            pressCatcher.AddChild(cellsContainer);
            dropDownContainer.AddChild(pressCatcher);
            cellsContainer.Frame = dropDownFrame;
        }

        void CreateDropDownCells(int itemsHeight, int containerHeight)
        {
            var scrollFrame = new Rectangle(0, 0, Frame.Width, containerHeight);

            cellsContainer = new ScrollPanel(
                scrollFrame, 
                Orientation.Vertical, 
                spacing: 0, 
                padding: 0,
                includeScrollBar: itemsHeight > containerHeight,
                wrapContent: false,
                showShadows: false);

            var itemFrame = new Rectangle(0, 0, Frame.Width, Frame.Height);
            var index = 0;

            cells.Clear();
            foreach (var item in items)
            {
                var itemCell = new DropDownCell(itemFrame, index == SelectedIndex)
                {
                    Action = SelectItem,
                    Tag = index,
                };
                SetItemText(itemCell, item);
                cellsContainer.AddContent(itemCell);
                cells.Add(itemCell);
                ++index;
            }
        }
    }
}
