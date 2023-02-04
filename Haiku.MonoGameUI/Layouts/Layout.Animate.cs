using Haiku.MathExtensions;
using Microsoft.Xna.Framework;
using System;

namespace Haiku.MonoGameUI.Layouts
{
    public partial class Layout
    {
        public void AnimateOnScreenFromTheBottom()
        {
            AnimateOnScreenFromTheBottom(Frame);
        }

        public Animation.Builder Animate()
        {
            return new Animation.Builder(this);
        }

        bool isAnimating = false;
        Animation fadeAnimation;
        float targetFadeAlpha;

        public void FadeIn(Action completion = null, double duration = 0.1)
        {
            AlphaTo(1f, duration, completion);
        }

        public void FadeOut(double duration = 0.1)
        {
            AlphaTo(0f, duration, null);
        }

        void AlphaTo(float targetAlpha, double duration, Action completion)
        {
            if (fadeAnimation == null
                || targetFadeAlpha != targetAlpha)
            {
                IsVisible = true;
                targetFadeAlpha = targetAlpha;
                ParentWindow?.CancelAnimation(fadeAnimation);
                fadeAnimation = 
                    Animate()
                    .Over(duration)
                    .Curving(RCurve.QuadEaseInOut)
                    .Opacity(Alpha, targetFadeAlpha)
                    .CompletingWith(_ => {
                            fadeAnimation = null;
                            if (targetFadeAlpha == 0f)
                            {
                                IsVisible = false;
                            }
                            else
                            {
                                IsVisible = true;
                            }
                            completion?.Invoke();
                        })
                    .Build();
                ParentWindow?.Add(fadeAnimation);
            }
        }

        public void AnimateOnScreenFromTheBottom(Rectangle triggerZone)
        {
            if (isAnimating)
            {
                return;
            }
            var bottom = new Point(triggerZone.X, triggerZone.Bottom);
            var top = new Point(triggerZone.X, bottom.Y - triggerZone.Height);
            var hideAnimation = Animate().Hide();
            var positionAnimation = Animate()
                .After(1 / 60)
                .Position(from: bottom, to: top)
                .Over(0.46)
                .Curving(RCurve.QuadEaseInOut);
            var initalAlpha = Animate().Opacity(to: 0);
            var alphaAnimation = Animate()
                .After(0.4)
                .Opacity(from: 0, to: 1f)
                .Over(0.46)
                .Curving(RCurve.Linear)
                .CompletingWith((animation) => { isAnimating = false; });
            var showAnimation = Animate().After(1/60).Show();

            ParentWindow?.Add(hideAnimation);
            ParentWindow?.Add(initalAlpha);
            ParentWindow?.Add(positionAnimation);
            ParentWindow?.Add(alphaAnimation);
            ParentWindow?.Add(showAnimation);
            isAnimating = true;
        }
    }
}
