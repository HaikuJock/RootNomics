using Microsoft.Xna.Framework;
using TexturePackerLoader;

namespace Haiku.MonoGameUI.Layouts
{
    public class DropDownCell : Button
    {
        public static SpriteFrame CellNormal;
        public static SpriteFrame CellActive;
        public static SpriteFrame CellSelected;
        bool isCurrentSelection;

        public DropDownCell(Rectangle frame, bool isCurrentSelection)
            : base(frame)
        {
            this.isCurrentSelection = isCurrentSelection;
            SetBackground(CellNormal, ControlState.Normal);
            SetBackground(CellActive, ControlState.Active);
            SetBackground(CellActive, ControlState.Highlighted);
            SetBackground(CellSelected, ControlState.Selected);
            State = isCurrentSelection ? ControlState.Selected : ControlState.Normal;
        }

        public void SetSelected(bool isSelected)
        {
            isCurrentSelection = isSelected;
            State = isCurrentSelection ? ControlState.Selected : ControlState.Normal;
        }

        public override void Trigger()
        {
            base.Trigger();
            Action(this);
        }

        protected override void OnCursorEntered(Point point, Rectangle container)
        {
            base.OnCursorEntered(point, container);
            if (State == ControlState.Active
                || State == ControlState.Selected)
            {
                State = ControlState.Highlighted;
            }
        }

        protected override void OnCursorExited(Point point, Rectangle container)
        {
            base.OnCursorExited(point, container);
            State = isCurrentSelection ? ControlState.Selected : ControlState.Normal;
        }
    }
}
