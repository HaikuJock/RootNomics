using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Haiku.MonoGameUI.Layouts
{
    public interface ButtonGrouping
    {
        GroupButton SelectButton(int tag);
        void DeselectAll();
    }

    public class ButtonGroup : Control, ButtonGrouping
    {
        public int SelectedButtonTag => Buttons().FirstOrDefault(button => button.State == ControlState.Selected)?.Tag ?? 0;

        public ButtonGroup(Rectangle frame) 
            : base(frame)
        {
        }

        public ButtonGroup(Rectangle frame, LayoutStrategy layoutStrategy)
            : base(frame, layoutStrategy)
        {
        }

        public List<GroupButton> Buttons()
        {
            return Children.Select(child => child).OfType<GroupButton>().ToList();
        }

        public GroupButton SelectButton(int tag)
        {
            DeselectAll();
            foreach (var child in Children)
            {
                if (child is GroupButton button
                    && button.Tag == tag)
                {
                    button.State = ControlState.Selected;
                    return button;
                }
            }
            return null;
        }

        public void DeselectAll()
        {
            foreach (var child in Children)
            {
                if (child is GroupButton button)
                {
                    button.State = ControlState.Normal;
                }
            }
        }
    }
}
