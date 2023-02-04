using Microsoft.Xna.Framework;

namespace Haiku.MonoGameUI.Layouts
{
    public class DraggableGroupButton : GroupButton
    {
        public DraggableGroupButton(ButtonTriggerHandler action, Rectangle frame, ButtonGrouping buttonGroup) 
            : base(frame, action, buttonGroup)
        {
            IsDraggingEnabled = true;
        }

        public override void OnDragStart(Point point, Rectangle container)
        {
            if (State != ControlState.Selected)
            {
                State = ControlState.Normal;
            }
            Parent?.OnDragStart(point, container);
        }

        public override bool OnCursorDrag(Point point, Rectangle container)
        {
            Tip?.UpdatePosition(point);
            return Parent?.OnCursorDrag(point, container) ?? false;
        }

        public override bool OnCursorDragOutside(Point point)
        {
            Tip?.UpdatePosition(point);
            return Parent?.OnCursorDragOutside(point) ?? false;
        }

        public override void OnDragEnd(Point point, Rectangle container)
        {
            Parent?.OnDragEnd(point, container);
        }

        protected override void OnCursorExited(Point point, Rectangle container)
        {
            if (IsPressed
                && !IsDragging)
            {
                State = ControlState.Normal;
                if (Parent != null
                    && Parent.Parent != null
                    && container.Contains(point))
                {
                    var parentContainer = Parent.Parent.CalculateWorldFrame();

                    Parent.DoOnPress(point, parentContainer);
                }
            }
            base.OnCursorExited(point, container);
        }
    }
}
