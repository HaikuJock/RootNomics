using Microsoft.Xna.Framework;
using System;

namespace Haiku.MonoGameUI.Layouts
{
    public delegate void OnGrabbedDelegate();

    public class Grab : Button
    {
        public bool IsExtended => attachedTo.Frame.Location == togglePosition;
        public bool IsContracted => attachedTo.Frame.Location == defaultPosition;
        public Rectangle RemainWithin;
        readonly Layout attachedTo;
        readonly Point defaultPosition;
        readonly Point togglePosition;
        readonly Orientation constraint;
        readonly bool isConstrained;
        Point previousDragPoint;
        Vector2 velocity;
        public OnGrabbedDelegate OnGrabbed;

        public Grab(Rectangle frame, Layout attachedTo, Point togglePosition, Rectangle remainWithin, Orientation constraint)
            : this(frame, attachedTo)
        {
            this.togglePosition = togglePosition;
            RemainWithin = remainWithin;
            this.constraint = constraint;
            isConstrained = true;
            defaultPosition = remainWithin.Location;
            Action = (_) => ToggleExtended();
            velocity = Vector2.Zero;
        }

        public Grab(Rectangle frame, Layout attachedTo) 
            : base(frame)
        {
            this.attachedTo = attachedTo;
            IsDraggingEnabled = true;
        }

        public void Extend()
        {
            if (!IsExtended)
            {
                ToggleExtended();
            }
        }

        public void ToggleExtended()
        {
            ToggleExtended(() => { });
        }

        public void ToggleExtended(Action onCompletion)
        {
            if (!IsDragging)
            {
                OnGrabbed?.Invoke();
                var origin = attachedTo.Frame.Location;
                var destination = FurthestTogglePosition();
                var distance = (destination - origin).ToVector2().Length();

                AnimateTo(destination, distance * 0.001, onCompletion);
            }
        }

        public override void OnDragStart(Point point, Rectangle container)
        {
            previousDragPoint = point;
            OnGrabbed?.Invoke();
        }

        public override bool OnCursorDrag(Point point, Rectangle container)
        {
            DragTo(point);
            return true;
        }

        public override bool OnCursorDragOutside(Point point)
        {
            if (IsDragging)
            {
                DragTo(point);
            }
            return IsDragging;
        }

        public override void OnDragEnd(Point point, Rectangle container)
        {
            var speed = velocity.Length();

            if (speed >= 1.6)
            {
                var delta = (velocity * 6).ToPoint();
                Point destination = ConstrainedDestination(delta);
                var duration = speed * 0.008;

                AnimateTo(destination, duration);
            }
        }

        void DragTo(Point point)
        {
            var delta = point - previousDragPoint;
            velocity = delta.ToVector2();

            Point destination = ConstrainedDestination(delta);

            attachedTo.Frame = new Rectangle(destination, attachedTo.Frame.Size);
            previousDragPoint = point;
        }

        void AnimateTo(Point destination, double duration)
        {
            AnimateTo(destination, duration, () => { });
        }

        void AnimateTo(Point destination, double duration, Action onCompletion)
        {
            var animation = attachedTo
                .Animate()
                .Position(to: destination)
                .Over(duration)
                .Curving(MathExtensions.RCurve.QuadEaseOut)
                .CompletingWith((_) => 
                {
                    attachedTo.Frame = new Rectangle(destination, attachedTo.Frame.Size);
                    onCompletion();
                });
            attachedTo.ParentWindow?.Add(animation);
        }

        Point FurthestTogglePosition()
        {
            var distanceToToggle = (attachedTo.Frame.Location - togglePosition).ToVector2().LengthSquared();
            var distanceToDefault = (attachedTo.Frame.Location - defaultPosition).ToVector2().LengthSquared();

            return 
                distanceToToggle > distanceToDefault
                ? togglePosition 
                : defaultPosition;
        }

        Point ConstrainedDestination(Point delta)
        {
            if (isConstrained)
            {
                if (constraint == Orientation.Horizontal)
                {
                    delta = new Point(delta.X, 0);
                }
                else
                {
                    delta = new Point(0, delta.Y);
                }
            }
            var destination = attachedTo.Frame.Location + delta;

            if (isConstrained)
            {
                var x = destination.X;
                var y = destination.Y;

                if (x < RemainWithin.Left)
                {
                    x = RemainWithin.Left;
                }
                if (x + attachedTo.Frame.Width > RemainWithin.Right)
                {
                    x = RemainWithin.Right - attachedTo.Frame.Width;
                }
                if (y < RemainWithin.Top)
                {
                    y = RemainWithin.Top;
                }
                if (y + attachedTo.Frame.Height > RemainWithin.Bottom)
                {
                    y = RemainWithin.Bottom - attachedTo.Frame.Height;
                }
                destination = new Point(x, y);
            }

            return destination;
        }

        public void ExtendFromContracted()
        {
            if (IsContracted)
            {
                Extend();
            }
        }
    }
}
