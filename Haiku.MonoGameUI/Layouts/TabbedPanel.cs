using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Haiku.MonoGameUI.Layouts
{
    public class TabbedPanel : Panel
    {
        public int SelectedTabIndex => toolbar.Buttons().FirstOrDefault(btn => btn.State == ControlState.Selected)?.Tag ?? -1;
        readonly Toolbar toolbar;
        readonly Layout tabsContainer;
        int selectedIndex;

        public TabbedPanel(Rectangle frame)
            : this(frame, 32, Orientation.Horizontal, isBarAtTop: true)
        {
        }

        public TabbedPanel(Rectangle frame, int tabButtonSize, Orientation orientation, bool isBarAtTop) 
            : base(frame)
        {
            var barY = isBarAtTop ? 0 : frame.Height - tabButtonSize;
            var tabY = isBarAtTop ? tabButtonSize : 0;
            var tabsFrame = new Rectangle(0, tabY, frame.Width, frame.Height - tabButtonSize);
            var barFrame = new Rectangle(0, barY, tabButtonSize, tabButtonSize);

            tabsContainer = new Layout(tabsFrame);
            AddChild(tabsContainer);
            toolbar = new Toolbar(barFrame, new LinearLayoutStrategy(orientation))
            {
                BackgroundColor = Color.Transparent
            };
            AddChild(toolbar);
        }

        public void Add(GroupButton tabButton, Layout tabPanel)
        {
            bool isFirst = tabsContainer.Children.Count == 0;

            tabButton.Tag = tabsContainer.Children.Count;
            tabButton.State = isFirst ? ControlState.Selected : ControlState.Normal;
            toolbar.Add(tabButton);
            tabButton.Action += OnTabButton;

            tabPanel.IsVisible = isFirst;
            tabPanel.Alpha = isFirst ? 1f : 0f;
            tabsContainer.AddChild(tabPanel);
        }

        public void SelectTab(int index)
        {
            toolbar.SelectButton(index);
            SelectTabInternal(index);
        }

        public void SelectTabContaining(Layout layout)
        {
            int tabTag = 0;

            foreach (var tabTuple in tabsContainer.Children.Zip(toolbar.Children, (panel, button) => Tuple.Create(panel, button)))
            {
                var tabPanel = tabTuple.Item1;
                
                if (layout.IsDescendantOf(tabPanel))
                {
                    if (tabTuple.Item2 is GroupButton button)
                    {
                        button.Trigger();
                        break;
                    }
                }
                ++tabTag;
            }
        }

        void OnTabButton(object obj)
        {
            var tabButton = obj as GroupButton;
            var index = tabButton.Tag;
            SelectTabInternal(index);
        }

        void SelectTabInternal(int index)
        {
            if (selectedIndex != index)
            {
                void completion() => ParentWindow?.RefreshFocusableControls();
                tabsContainer.Children[selectedIndex].FadeOut(0.16);
                tabsContainer.Children[index].FadeIn(completion, 0.16);
            }
            selectedIndex = index;
        }
    }
}
