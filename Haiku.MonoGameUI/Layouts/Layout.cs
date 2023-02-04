using Haiku.MonoGameUI.LayoutStrategies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haiku.MonoGameUI.Layouts
{
    public partial class Layout
    {
        public static Func<Rectangle, Rectangle> ScissorFunc = (rectangle) => rectangle;
        const int DragThresholdSquared = 25;
        public static SpriteFont TitleFont;   // Set me
        public static SpriteFont HeadingFont;   // Set me
        public static SpriteFont MainFont;   // Set me
        public static SpriteFont BodyFont;   // Set me
        public static SpriteFont LabelFont;   // Set me
        public static SpriteFont ItemFont;   // Set me
        
        public Rectangle Frame = Rectangle.Empty;
        public Point Size => Frame.Size;
        public List<Layout> Children = new List<Layout>();
        public Layout Parent;

        public float Alpha = 1;
        public float LocalRotation;
        public bool IsInteractionEnabled = true;
        public bool IsVisible = true;
        public bool IsClippingChildren;
        public bool IsDraggingEnabled;

        internal bool IsDragging { get { return isDragging; } }
        internal bool IsPressed { get { return isPressed; } }
        public virtual Window ParentWindow { get { return Parent?.ParentWindow; } }

        public LayoutStrategy LayoutStrategy
        {
            get
            {
                return _layoutStrategy;
            }
            set
            {
                if (_layoutStrategy != value)
                {
                    _layoutStrategy = value;
                    DoLayout();
                }
            }
        }
        LayoutStrategy _layoutStrategy;
        Point pressStartedAtPoint;
        bool isCursorInside;
        bool isPressed;
        bool isDragging;

        public Layout(int size)
            : this(size, size)
        {
        }

        public Layout((int, int) size)
            : this(size.Item1, size.Item2)
        {
        }

        public Layout((int, int) size, LayoutStrategy layoutStrategy)
            : this(new Rectangle(0, 0, size.Item1, size.Item2), layoutStrategy)
        {
        }

        public Layout(int width, int height)
            : this(0, 0, width, height)
        {
        }

        public Layout(float x, float y, float width, float height)
            : this((int)x, (int)y, (int)width, (int)height)
        {
        }

        public Layout(int x, int y, int width, int height)
            : this(new Rectangle(x, y, width, height))
        {
        }

        public Layout(Rectangle frame)
            : this(frame, new FrameLayoutStrategy())
        {
        }

        public Layout(Rectangle frame, LayoutStrategy layoutStrategy)
        {
            Frame = frame;
            _layoutStrategy = layoutStrategy;
        }

        public virtual void SetFrameWithSideEffects(Rectangle newFrame)
        {
            Frame = newFrame;
        }

        public virtual void AddChild(Layout child)
        {
            child.RemoveFromParent();
            child.Parent = this;
            Children.Add(child);
            DoLayout();
        }

        public virtual void AddChildren(IEnumerable<Layout> children)
        {
            foreach (var child in children)
            {
                child.RemoveFromParent();
                child.Parent = this;
                Children.Add(child);
            }
            DoLayout();
        }

        public void DoLayout()
        {
            _layoutStrategy.LayoutChildren(new Point(Frame.Width, Frame.Height), Children);
            Frame = new Rectangle(Frame.X, Frame.Y, _layoutStrategy.ParentSize.X, _layoutStrategy.ParentSize.Y);
        }

        public void RemoveFromParent()
        {
            Parent?.RemoveChild(this);
            Parent = null;
        }

        public void RemoveAllChildren()
        {
            while (Children.Count > 0)
            {
                Children[0].RemoveFromParent();
            }
        }

        public void BringChildToFront(Layout child)
        {
            if (RemoveChild(child))
            {
                AddChild(child);
            }
        }

        public void SendChildToBack(Layout child)
        {
            if (RemoveChild(child))
            {
                InsertChild(0, child);
            }
        }

        public void MoveChildInFrontOfSibling(Layout child, Layout sibling)
        {
            var initialIndex = Children.IndexOf(child);
            
            if (RemoveChild(child))
            {
                var index = Children.IndexOf(sibling);
                if (index >= 0)
                {
                    InsertChild(index + 1, child);
                }
                else
                {
                    InsertChild(initialIndex, child);
                }
            }
        }

        public bool IsDescendantOf(Layout layout)
        {
            if (layout == this)
            {
                return true;
            }
            else
            {
                foreach (var child in layout.Children)
                {
                    if (IsDescendantOf(child))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void CenterInParent()
        {
            if (Parent != null)
            {
                var x = Parent.Frame.Width / 2 - Frame.Width / 2;
                var y = Parent.Frame.Height / 2 - Frame.Height / 2;
                Frame = new Rectangle(x, y, Frame.Width, Frame.Height);
            }
        }

        public void CenterXInParent()
        {
            if (Parent != null)
            {
                var x = Parent.Frame.Width / 2 - Frame.Width / 2;
                Frame = new Rectangle(x, Frame.Y, Frame.Width, Frame.Height);
            }
        }

        public void AlignRightInParent()
        {
            if (Parent != null)
            {
                var x = Parent.Frame.Width - Frame.Width;
                Frame = new Rectangle(x, Frame.Y, Frame.Width, Frame.Height);
            }
        }

        public void CenterYInParent()
        {
            if (Parent != null)
            {
                var y = Parent.Frame.Height / 2 - Frame.Height / 2;
                Frame = new Rectangle(Frame.X, y, Frame.Width, Frame.Height);
            }
        }

        public bool HandleCursorMove(Point point, Rectangle container)
        {
            if (isPressed
                && IsDraggingEnabled
                && IsMouseMoveThresholdExceeded(pressStartedAtPoint, point))
            {
                DoOnCursorDrag(point, container);
                return true;
            }
            return HandleInput(point, container, HandleCursorMoveFor, DoOnCursorMove);
        }

        public bool HandlePress(Point point, Rectangle container)
        {
            return HandleInput(point, container, HandlePressFor, DoOnPress);
        }

        public bool HandleRelease(Point point, Rectangle container)
        {
            if (ParentWindow?.HandleDraggingRelease(point, container) == true)
            {
                return HandleInput(point, container, HandleCursorMoveFor, DoOnCursorMove);
            }
            return HandleInput(point, container, HandleReleaseFor, DoOnRelease);
        }

        public bool HandleAltPress(Point point, Rectangle container)
        {
            return HandleInput(point, container, HandleAltPressFor, OnAltPress);
        }

        public bool HandleAltRelease(Point point, Rectangle container)
        {
            return HandleInput(point, container, HandleAltReleaseFor, OnAltRelease);
        }

        public bool HandleScroll(int scrollDistance, Point point, Rectangle container)
        {
            if (!IsInteractable())
            {
                return false;
            }

            Rectangle worldFrame = WorldFrame(container);

            if (worldFrame.Contains(point))
            {
                foreach (var child in Children.Reverse<Layout>())
                {
                    if (child.HandleScroll(scrollDistance, point, worldFrame))
                    {
                        return true;
                    }
                }
                return OnScroll(scrollDistance, point, container);
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle container)
        {
            Draw(spriteBatch, container, Alpha);
        }

        public Rectangle CalculateWorldFrame()
        {
            Layout layout = this;
            var position = Point.Zero;

            while (layout.Parent != null)
            {
                position += layout.Frame.Location;
                layout = layout.Parent;
            }

            return new Rectangle(position, Frame.Size);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Rectangle container, float alpha)
        {
            if (!IsVisible)
            {
                return;
            }
            Rectangle destination = WorldFrame(container);

            if (destination.Intersects(container))
            {
                if (IsClippingChildren)
                {
                    DrawChildrenClipped(spriteBatch, destination, alpha);
                }
                else
                {
                    DrawChildren(spriteBatch, destination, alpha);
                }
            }
#if DEBUG
            else
            {
                if (Parent != null
                    && Parent.IsClippingChildren == false
                    && Size.X > 0
                    && Size.Y > 0
                    && destination.Intersects(ParentWindow.Frame))
                {
                    throw new Exception($"Layout: Child outside a non-clipper. Container: {container} Destination: {destination} this: {this} Parent: {Parent}");
                }
            }
#endif
        }

        protected void InsertChild(int index, Layout child)
        {
            Children.Insert(index, child);
            DoLayout();
        }

        protected virtual bool RemoveChild(Layout child)
        {
            if (Children.Remove(child))
            {
                DoLayout();
                return true;
            }
            return false;
        }

        protected virtual bool OnCursorMove(Point point, Rectangle container)
        {
            return false;
        }

        public virtual bool OnCursorDragOutside(Point point)
        {
            return false;
        }

        public virtual bool OnCursorDrag(Point point, Rectangle container)
        {
            return false;
        }

        protected virtual void OnCursorEntered(Point point, Rectangle container)
        {
        }

        protected virtual void OnCursorExited(Point point, Rectangle container)
        {
        }

        public virtual void OnDragStart(Point point, Rectangle container)
        {
        }

        public virtual void OnDragEnd(Point point, Rectangle container)
        {
        }

        protected virtual bool OnPress(Point point, Rectangle container)
        {
            return false;
        }

        protected virtual bool OnRelease(Point point, Rectangle container)
        {
            return false;
        }

        protected virtual bool OnAltPress(Point point, Rectangle container)
        {
            return false;
        }

        protected virtual bool OnAltRelease(Point point, Rectangle container)
        {
            return false;
        }

        protected virtual bool OnScroll(int scrollDistance, Point point, Rectangle container)
        {
            return false;
        }

        protected Rectangle WorldFrame(Rectangle container)
        {
            return new Rectangle(container.X + Frame.X,
                                 container.Y + Frame.Y,
                                 Frame.Width,
                                 Frame.Height);
        }

        protected bool IsInteractable()
        {
            return IsInteractionEnabled && IsVisible;
        }

        void DrawChildrenClipped(SpriteBatch spriteBatch, Rectangle destination, float alpha)
        {
            RasterizerState clipper = new RasterizerState();

            spriteBatch.End();
            clipper.ScissorTestEnable = true;
            spriteBatch.GraphicsDevice.RasterizerState = clipper;
            spriteBatch.GraphicsDevice.ScissorRectangle = ScissorFunc(destination);            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, clipper);
            DrawChildren(spriteBatch, destination, alpha);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
        }

        void DrawChildren(SpriteBatch spriteBatch, Rectangle destination, float alpha)
        {
            foreach (var child in Children)
            {
                child.Draw(spriteBatch, destination, alpha * child.Alpha);
            }
        }

        private bool HandleInput(
            Point point,
            Rectangle container,
            Func<Layout, Point, Rectangle, bool> childHandler,
            Func<Point, Rectangle, bool> thisHandler)
        {
            if (!IsInteractable())
            {
                return false;
            }

            Rectangle worldFrame = WorldFrame(container);

            if (worldFrame.Contains(point))
            {
                foreach (var child in Children.Reverse<Layout>())
                {
                    if (childHandler(child, point, worldFrame))
                    {
                        return true;
                    }
                }
                return thisHandler(point, container);
            }
            return false;
        }

        private bool HandleCursorMoveFor(Layout child, Point point, Rectangle container)
        {
            return child.HandleCursorMove(point, container);
        }

        private bool DoOnCursorMove(Point point, Rectangle container)
        {
            if (!isCursorInside)
            {
                if (ParentWindow?.IsHandlingDrag == true)
                {
                    return ParentWindow.HandleCursorDrag(point, container);
                }
                ParentWindow?.SetLayoutUnderCursor(this, point);
                DoOnCursorEntered(point, container);
            }
            return OnCursorMove(point, container);
        }

        internal bool DoOnPress(Point point, Rectangle container)
        {
            if (!isPressed)
            {
                pressStartedAtPoint = point;
            }
            isPressed = true;
            return OnPress(point, container);
        }

        private bool DoOnCursorDrag(Point point, Rectangle container)
        {
            if (!isDragging)
            {
                _StartDragging(point, container);
            }
            return OnCursorDrag(point, container);
        }

        internal bool DoOnRelease(Point point, Rectangle container)
        {
            isPressed = false;
            bool didHandle = OnRelease(point, container);
            if (isDragging)
            {
                _EndDragging(point, container);
            }
            return didHandle;
        }

        internal void StartDragging(Point point, Rectangle container)
        {
            DoOnCursorEntered(point, container);
            DoOnPress(point, container);
            ParentWindow?.SetLayoutUnderCursor(this, point);
            _StartDragging(point, container);
        }

        internal void EndDragging(Point point, Rectangle container)
        {
            DoOnCursorExited(point, container);
            DoOnRelease(point, container);
            _EndDragging(point, container);
        }

        internal void _StartDragging(Point point, Rectangle container)
        {
            isDragging = true;
            OnDragStart(point, container);
        }

        private void _EndDragging(Point point, Rectangle container)
        {
            isDragging = false;
            OnDragEnd(point, container);
        }

        private bool HandleReleaseFor(Layout child, Point point, Rectangle container)
        {
            return child.HandleRelease(point, container);
        }

        private bool HandlePressFor(Layout child, Point point, Rectangle container)
        {
            return child.HandlePress(point, container);
        }

        private bool HandleAltReleaseFor(Layout child, Point point, Rectangle container)
        {
            return child.HandleAltRelease(point, container);
        }

        private bool HandleAltPressFor(Layout child, Point point, Rectangle container)
        {
            return child.HandleAltPress(point, container);
        }

        internal void DoOnCursorEntered(Point point, Rectangle container)
        {
            isCursorInside = true;
            OnCursorEntered(point, container);
        }

        internal void DoOnCursorExited(Point point, Rectangle container)
        {
            isCursorInside = false;
            OnCursorExited(point, container);
            if (isPressed
                && !IsDragging)
            {
                isPressed = false;
            }
        }

        protected static bool IsMouseMoveThresholdExceeded(Point pressStartedAtPoint, Point point)
        {
            return (point.X - pressStartedAtPoint.X) * (point.X - pressStartedAtPoint.X)
                 + (point.Y - pressStartedAtPoint.Y) * (point.Y - pressStartedAtPoint.Y)
                  >= DragThresholdSquared;
        }
    }
}
