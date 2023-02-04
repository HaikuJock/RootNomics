using Microsoft.Xna.Framework;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class ToolbarButton : GroupButton
    {
        GroupButton button;
        readonly Toolbar subToolbar;
        readonly Button accessoryButton;
        public override int Tag {
            get { return button.Tag; }
            set { button.Tag = value; }
        }
        public override SpriteFrame ForegroundImage { get { return button.ForegroundImage; } }
        public override ControlState State
        {
            get
            {
                return button.State;
            }
            set
            {
                button.State = value;
                button.OnNewState();
            }
        }

        public ToolbarButton(
            GroupButton button,
            Button accessoryButton,
            Toolbar subToolbar,
            Rectangle frame)
            : base(frame, _ => { })
        {
            this.subToolbar = subToolbar;
            this.button = button;
            this.accessoryButton = accessoryButton;
            accessoryButton.Action = ToggleSubGroup;
            AddChild(accessoryButton);
            AddChild(button);
            subToolbar.IsVisible = false;
            SubToolbarDidChange();
        }

        public void SubToolbarDidChange()
        {
            accessoryButton.IsVisible = subToolbar.ButtonCount() > 0;
        }

        public void SwapToSubtoolbarButton(int tag)
        {
            foreach (var groupButton in subToolbar.Buttons())
            {
                if (groupButton.Tag == tag)
                {
                    SwapToSubtoolbarButton(groupButton);
                    break;
                }
            }
        }

        public void SwapToSubtoolbarButton(GroupButton newButton)
        {
            if (subToolbar.Buttons().Contains(newButton))
            {
                var frameInSubtoolbar = newButton.Frame;
                var oldButton = button;

                newButton.Frame = button.Frame;
                newButton.RemoveFromParent();
                oldButton.Frame = frameInSubtoolbar;
                oldButton.RemoveFromParent();
                subToolbar.Add(oldButton);
                button = newButton;
                AddChild(newButton);
                SendChildToBack(newButton);
                subToolbar.DeselectAll();
                SendChildToBack(accessoryButton);
            }
        }

        public void HideSubToolbar()
        {
            subToolbar.IsVisible = false;
        }

        internal bool SelectSubToolbarButton(int tag)
        {
            foreach (var button in subToolbar.Buttons())
            {
                if (button.Tag == tag)
                {
                    subToolbar.SelectButton(tag);
                    return true;
                }
            }

            return false;
        }

        protected override bool OnCursorMove(Point point, Rectangle container)
        {
            return false;
        }

        protected override bool OnPress(Point point, Rectangle container)
        {
            return false;
        }

        protected override bool OnRelease(Point point, Rectangle container)
        {
            return false;
        }

        protected override bool OnAltPress(Point point, Rectangle container)
        {
            return false;
        }

        protected override bool OnAltRelease(Point point, Rectangle container)
        {
            return false;
        }

        void ToggleSubGroup(object obj)
        {
            subToolbar.IsVisible = !subToolbar.IsVisible;
        }
    }
}
