using Haiku.Audio;
using Haiku.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Haiku.MonoGameUI.Layouts
{
    public class Window : Layout
    {
        public override Window ParentWindow { get { return this; } }
        internal bool IsHandlingDrag 
        { 
            get 
            { 
                return layoutUnderCursor != null &&
                    (layoutUnderCursor.IsDragging 
                     || (layoutUnderCursor.IsPressed 
                         && layoutUnderCursor.IsDraggingEnabled)); 
            }
        }
        public bool IsOpaque { get; set; }
        readonly HashSet<Animation> animations = new HashSet<Animation>();
        protected readonly AudioPlaying audio;
        readonly List<Control> focusableControls = new List<Control>();
        readonly HashSet<Animation> finishedAnimations = new HashSet<Animation>();
        Layout layoutUnderCursor;
        int focusIndex;

        public Window(Rectangle frame, AudioPlaying audio)
            : base(frame)
        {
            this.audio = audio;
        }

        public void Add(Animation.Builder builder) => Add(builder.Build());
        public void Add(Animation animation) => animations.Add(animation);

        public void CancelAnimation(Animation animation) => animations.Remove(animation);

        public void CancelAnimations(Layout layout)
        {
            animations.RemoveWhere(anim => anim.Target == layout);
        }
        
        public virtual void OnAppear()
        {
            ParentWindow.GiveFocusToFirstFocusableControl();
        }

        public virtual void OnDisappear()
        {
        }

        public virtual void OnPopped()
        {
        }

        public virtual void OnPushed()
        {
        }

        public virtual void Update(double deltaSeconds)
        {
            UpdateAnimations(deltaSeconds);
        }

        public void PlaySound(string name)
        {
            audio.PlaySound(name);
        }

        public override void AddChild(Layout child)
        {
            RecurseAddFocusableControls(child);
            base.AddChild(child);
        }

        public override void AddChildren(IEnumerable<Layout> children)
        {
            foreach (var child in children)
            {
                RecurseAddFocusableControls(child);
            }
            base.AddChildren(children);
        }

        protected override bool RemoveChild(Layout child)
        {
            RecurseRemoveFocusableControls(child);
            if (focusIndex >= focusableControls.Count)
            {
                ReverseFocus();
            }
            return base.RemoveChild(child);
        }

        internal bool HandleDraggingRelease(Point point, Rectangle container)
        {
            if (IsHandlingDrag)
            {
                layoutUnderCursor.DoOnRelease(point, container);
                return true;
            }
            return false;
        }

        internal bool HandleCursorDrag(Point point, Rectangle container)
        {
            if (layoutUnderCursor != null
                && !layoutUnderCursor.IsDragging)
            {
                layoutUnderCursor._StartDragging(point, container);
            }
            return layoutUnderCursor?.OnCursorDragOutside(point) == true;
        }

        internal void SetLayoutUnderCursor(Layout layout, Point point)
        {
            if (layoutUnderCursor != null)
            {
                Rectangle container;

                if (layoutUnderCursor.Parent == null)
                {
                    container = layoutUnderCursor.Frame;
                }
                else
                {
                    container = layoutUnderCursor.Parent.CalculateWorldFrame();
                }

                layoutUnderCursor.DoOnCursorExited(point, container);
            }
            layoutUnderCursor = layout;
        }

        public void GiveFocusTo(Control control)
        {
            if (control.IsFocusable 
                && control.IsInteractionEnabled
                && focusableControls.Contains(control))
            {
                SetFocusIndex(focusableControls.IndexOf(control));
            }
        }

        public bool IsFocussed(Control control)
        {
            return focusableControls.Count > 0
                && focusIndex < focusableControls.Count
                && focusableControls[focusIndex] == control;
        }

        public void AdvanceFocus()
        {
            if (focusableControls.Count > 0)
            {
                var nextIndex = (focusIndex + 1) % focusableControls.Count;
                while (nextIndex != focusIndex
                       && !focusableControls[nextIndex].IsInteractionEnabled)
                {
                    nextIndex = (nextIndex + 1) % focusableControls.Count;
                }
                SetFocusIndex(nextIndex);
            }
        }

        public void ReverseFocus()
        {
            if (focusableControls.Count > 0)
            {
                var previousIndex = focusIndex - 1;
                if (previousIndex < 0)
                {
                    previousIndex = focusableControls.Count - 1;
                }
                while (previousIndex != focusIndex
                       && !focusableControls[previousIndex].IsInteractionEnabled)
                {
                    previousIndex--;
                    if (previousIndex < 0)
                    {
                        previousIndex = focusableControls.Count - 1;
                    }
                }
                SetFocusIndex(previousIndex);
            }
        }

        public void RefreshFocusableControls()
        {
            ClearFocusableControls();
            RecurseAddFocusableControls(this);
        }

        public virtual void RecurseAddFocusableControls(Layout child)
        {
            if (!child.IsVisible)
            {
                return;
            }
            if (child is Control)
            {
                var control = child as Control;

                if (control.IsFocusable)
                {
                    focusableControls.Add(control);
                }
            }
            foreach (var grandChild in child.Children)
            {
                RecurseAddFocusableControls(grandChild);
            }
        }

        internal bool AnyRayTraceHitsChild(Point position)
        {
            return Children.Any(layout => layout.IsVisible && layout.IsInteractionEnabled && layout.Frame.Contains(position));
        }

        internal bool HandleTextInput(Keys key, string keyAsString, IEnumerable<Keys> activeModifierKeys)
        {
            if (focusableControls.Count > 0)
            {
                var focusedControl = focusableControls[focusIndex];

                if (focusedControl.HandleTextInput(key, keyAsString, activeModifierKeys) == true)
                {
                    return true;
                }
            }

            return false;
        }

        internal bool HandleKeyPress(Keys key, IEnumerable<Keys> activeModifierKeys, TextClipboarding clipboard)
        {
            if (focusableControls.Count > 0)
            {
                var focusedControl = focusableControls[focusIndex];

                if (focusedControl.HandleKeyPress(key, activeModifierKeys, clipboard) == true)
                {
                    return true;
                }
                else if (key == Keys.Tab)
                {
                    if (activeModifierKeys.Contains(Keys.LeftShift)
                        || activeModifierKeys.Contains(Keys.RightShift))
                    {
                        ReverseFocus();
                    }
                    else
                    {
                        AdvanceFocus();
                    }
                    return true;
                }
            }

            return false;
        }

        void ClearFocusableControls()
        {
            focusableControls.Clear();
            focusIndex = 0;
        }

        void UpdateAnimations(double deltaSeconds)
        {
            foreach (var animation in animations)
            {
                if (animation.IsComplete)
                {
                    finishedAnimations.Add(animation);
                }
                else
                {
                    animation.Update(deltaSeconds);
                }
            }
            animations.ExceptWith(finishedAnimations);
            foreach (var animation in finishedAnimations)
            {
                animation.OnCompleted();
            }
            finishedAnimations.Clear();
            var focusedControl = focusableControls.Count > 0 ? focusableControls[focusIndex] : null;
            focusedControl?.UpdateFocused(deltaSeconds);
        }

        void GiveFocusToFirstFocusableControl()
        {
            if (focusableControls.Count > 0)
            {
                var focusedControl = focusableControls[0];

                focusedControl.OnGainFocus();
                focusIndex = 0;
            }
        }

        void RecurseRemoveFocusableControls(Layout child)
        {
            if (child is Control)
            {
                var control = child as Control;

                focusableControls.Remove(control);
            }
            foreach (var grandChild in child.Children)
            {
                RecurseRemoveFocusableControls(grandChild);
            }
        }

        void SetFocusIndex(int index)
        {
            var previousFocusedControl = 
                focusableControls.Count > 0 && focusIndex < focusableControls.Count 
                ? focusableControls[focusIndex] 
                : null;
            var newFocusedControl =
                focusableControls.Count > 0 && index < focusableControls.Count
                ? focusableControls[index]
                : null;

            if (previousFocusedControl != newFocusedControl)
            {
                previousFocusedControl?.OnLoseFocus();
                newFocusedControl?.OnGainFocus();
            }
            focusIndex = index;
        }
    }
}
