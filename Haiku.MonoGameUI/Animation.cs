using Haiku.MathExtensions;
using Haiku.MonoGameUI.Layouts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Haiku.MonoGameUI
{
    public class Animation
    {
        internal class DelayedAction : IEquatable<DelayedAction>
        {
            internal double Delay;
            internal Action<Layout> Do;

            public DelayedAction(double delay = 0)
                : this(delay, (_) => { })
            {

            }

            public DelayedAction(double delay, Action<Layout> action)
            {
                Delay = delay;
                Do = action;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as DelayedAction);
            }

            public bool Equals(DelayedAction other)
            {
                return other != null &&
                       Delay == other.Delay &&
                       EqualityComparer<Action<Layout>>.Default.Equals(Do, other.Do);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Delay, Do);
            }
        }

        internal class ContinuousAction : IEquatable<ContinuousAction>
        {
            internal Action<double, Layout> Continuous;
            internal Action<Animation, Layout> Completion;

            public ContinuousAction(Action<double, Layout> continuous, Action<Animation, Layout> completion)
            {
                Continuous = continuous;
                Completion = completion;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as ContinuousAction);
            }

            public bool Equals(ContinuousAction other)
            {
                return other != null
                       && EqualityComparer<Action<Animation, Layout>>.Default.Equals(Completion, other.Completion)
                       && EqualityComparer<Action<double, Layout>>.Default.Equals(Continuous, other.Continuous);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Continuous, Completion);
            }
        }

        public class Builder
        {
            readonly Layout layout;
            readonly Animation animation;
            DelayedAction delayedUnderConstruction;
            Point positionFrom;
            Point positionTo;

            public Builder(Layout layout)
            {
                this.layout = layout;
                animation = new Animation(layout);
            }

            public Builder After(double delay)
            {
                if (delayedUnderConstruction == null)
                {
                    delayedUnderConstruction = new DelayedAction();
                }
                animation.Duration = Math.Max(delay, animation.Duration);
                delayedUnderConstruction.Delay = delay;
                return this;
            }

            public Builder Position(Point to)
            {
                return Position(layout.Frame.Location, to);
            }

            public Builder Rotation(float toAngleInRadians)
            {
                return Rotation(layout.LocalRotation, toAngleInRadians);
            }

            public Builder Rotation(float fromAngleInRadians, float toAngleInRadians)
            {
                void rotating(Layout _)
                {
                    animation.ContinuousActions.Add(
                        new ContinuousAction(
                            continuous: (portion, layout) =>
                            {
                                var animatedRotation = toAngleInRadians;

                                if (portion < 1)
                                {
                                    var journey = toAngleInRadians - fromAngleInRadians;

                                    var curvedContinuousRotation = journey * portion;
                                    animatedRotation = fromAngleInRadians + (float)curvedContinuousRotation;
                                }
                                layout.LocalRotation = animatedRotation;
                            },
                            completion: (_, layout) => layout.LocalRotation = toAngleInRadians
                        )
                    );
                }
                if (delayedUnderConstruction != null)
                {
                    delayedUnderConstruction.Do = rotating;
                    CompleteDelayedUnderConstruction();
                }
                else
                {
                    rotating(layout);
                }
                return this;
            }

            public Builder Position(Point from, Point to)
            {
                positionFrom = from;
                positionTo = to;
                void positioning(Layout _)
                {
                    animation.ContinuousActions.Add(
                        new ContinuousAction(
                            continuous: (portion, layout) =>
                            {
                                var animatedPoint = to;
                                var journey = to - from;

                                var curvedContinuousPoint = new Vector2((float)(journey.X * portion), (float)(journey.Y * portion));
                                animatedPoint = new Point(
                                    (int)(from.X + Math.Round(curvedContinuousPoint.X)),
                                    (int)(from.Y + Math.Round(curvedContinuousPoint.Y)));
                                layout.Frame = new Rectangle(animatedPoint, layout.Frame.Size);
                            },
                            completion: (_, layout) => layout.Frame = new Rectangle(to, layout.Frame.Size)
                        )
                    );
                }
                if (delayedUnderConstruction != null)
                {
                    delayedUnderConstruction.Do = positioning;
                    CompleteDelayedUnderConstruction();
                }
                else
                {
                    positioning(layout);
                }
                return this;
            }

            public Builder Function(Action<double, Layout> action, Action<Animation, Layout> completion)
            {
                void functioning(Layout _)
                {
                    animation.ContinuousActions.Add(
                        new ContinuousAction(
                            continuous: action,
                            completion: completion
                        )
                    );
                }
                if (delayedUnderConstruction != null)
                {
                    delayedUnderConstruction.Do = functioning;
                    CompleteDelayedUnderConstruction();
                }
                else
                {
                    functioning(layout);
                }
                return this;
            }

            public Builder Hide()
            {
                return Show(false);
            }

            public Builder Show()
            {
                return Show(true);
            }

            Builder Show(bool show)
            {
                if (delayedUnderConstruction == null)
                {
                    delayedUnderConstruction = new DelayedAction();
                }
                delayedUnderConstruction.Do = (Layout lay) => lay.IsVisible = show;
                CompleteDelayedUnderConstruction();
                return this;
            }

            public Builder AtSpeed(double speedInPointsPerSecond)
            {
                var distance = (positionTo - positionFrom).ToVector2().Length();
                var duration = distance / speedInPointsPerSecond;

                return Over(duration);
            }

            void CompleteDelayedUnderConstruction()
            { 
                animation.DelayedActions.Add(delayedUnderConstruction);
            }

            public Builder Opacity(float to)
            {
                if (delayedUnderConstruction == null)
                {
                    delayedUnderConstruction = new DelayedAction();
                }
                delayedUnderConstruction.Do = (Layout lay) => lay.Alpha = to;
                CompleteDelayedUnderConstruction();
                return this;
            }

            public Builder Opacity(float from, float to)
            {
                void transparency(Layout _)
                {
                    animation.ContinuousActions.Add(
                        new ContinuousAction(
                            continuous: (portion, layout) => layout.Alpha = from + ((float)portion * (to - from)),
                            completion: (_, layout) => layout.Alpha = to
                        )
                    );
                }
                if (delayedUnderConstruction != null)
                {
                    delayedUnderConstruction.Do = transparency;
                    CompleteDelayedUnderConstruction();
                }
                else
                {
                    transparency(layout);
                }
                return this;
            }

            public Builder Over(double duration)
            {
                duration = 
                    (delayedUnderConstruction != null)
                    ? duration + delayedUnderConstruction.Delay
                    : duration;
                animation.Duration = Math.Max(1 / 60, duration);
                return this;
            }

            public Builder Curving(RCurve curve)
            {
                animation.RCurve = curve;
                return this;
            }

            public Builder CompletingWith(Action<Animation> action)
            {
                animation.OnCompletion = action;
                return this;
            }

            public Builder RepeatForever()
            {
                animation.OnCompletion = (anim) => anim.Restart();
                return this;
            }

            public Animation Build()
            {
                return animation;
            }
        }

        internal double Duration {
            get {
                return duration;
            } set {
                duration = Math.Max(1 / 60, value);
            }
        }

        internal bool IsComplete => Lifetime > Duration;
        internal double Lifetime { get; private set; }
        internal List<DelayedAction> DelayedActions { get; }
        internal List<ContinuousAction> ContinuousActions { get; }

        internal RCurve RCurve;
        Action<Animation> OnCompletion;
        double duration;
        public readonly Layout Target;
        readonly List<DelayedAction> completedActions;

        public Animation(Layout subject)
        {
            Target = subject;
            Duration = 1 / 60;
            DelayedActions = new List<DelayedAction>();
            completedActions = new List<DelayedAction>();
            ContinuousActions = new List<ContinuousAction>();
        }

        public void Restart()
        {
            Lifetime = 0;
            DelayedActions.AddRange(completedActions);
            completedActions.Clear();
            Target.ParentWindow?.Add(this);
        }

        public void OnCompleted()
        {
            foreach (var continuous in ContinuousActions)
            {
                continuous.Completion(this, Target);
            }
            OnCompletion?.Invoke(this);
        }

        internal void Update(double deltaSeconds)
        {
            var portion = (Lifetime / Duration);
            var curved = RelationCurve.Fn[(int)RCurve].Invoke(portion);

            foreach (var delayed in DelayedActions)
            {
                if (Lifetime <= delayed.Delay
                    && Lifetime + deltaSeconds > delayed.Delay)
                {
                    delayed.Do(Target);
                    completedActions.Add(delayed);
                    Lifetime -= delayed.Delay;
                    Duration -= delayed.Delay;
                    portion = (Lifetime / Duration);
                    curved = RelationCurve.Fn[(int)RCurve].Invoke(portion);
                }
            }
            foreach (var completed in completedActions)
            {
                DelayedActions.Remove(completed);
            }

            foreach (var continuous in ContinuousActions)
            {
                continuous.Continuous(curved, Target);
            }

            Lifetime += deltaSeconds;
        }
    }
}
