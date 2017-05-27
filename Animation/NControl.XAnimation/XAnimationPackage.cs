using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace NControl.XAnimation
{
    public class XAnimationPackage : XInterpolationPackage
    {
        #region Private Members
        bool _runningState;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:NControl.XAnimation.XAnimations"/> class.
        /// </summary>
        public XAnimationPackage(params VisualElement[] elements) : base(elements)
        {
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Animates the contained transformations on the list of visual elements 
        /// </summary>
        public void Animate(Action completed = null, long duration = 250, EasingFunction easing = EasingFunction.Linear)
        {
            if (_runningState)
                return;

            if (_animationInfos.Count == 0)
                return;

            _runningState = true;

            RunAnimation(_animationInfos.First(), false, completed, duration, easing);
        }

        /// <summary>
        /// Runs the animations async
        /// </summary>
        public Task AnimateAsync(long duration = 250, EasingFunction easing = EasingFunction.Linear)
        {
            var tcs = new TaskCompletionSource<bool>();
            Animate(() => tcs.TrySetResult(true), duration, easing);
            return tcs.Task;
        }

        /// <summary>
        /// Runs the animation in reverse
        /// </summary>
        public void AnimateReverse(Action completed = null, long duration = 250, EasingFunction easing = EasingFunction.Linear)
        {
            if (_runningState)
                return;

            if (_animationInfos.Count == 0)
                return;

            _runningState = true;

            RunAnimation(_animationInfos.Last(), true, completed, duration, easing);
        }

		#endregion

		#region Private Members

		/// <summary>
		/// Runs the animation from startAnimation to endAnimation
		/// </summary>
        void RunAnimation(XTransform currentAnimation, bool reverse, 
                          Action completed, long duration, EasingFunction easing)
		{
			// If no views are live we can just return without calling completed,
			// user interface is no longer available
			if (!Provider.GetHasViewsToAnimate(currentAnimation))
				return;

			// Save state if no state is found
			if (!HasInterpolationStart)
			{
				// If we are reversing we should have called RunAnimation first which should
				// again have set the starting point. If not, we can just return, we're at the
				// beginning anyways.
				if (reverse)
					return;

				SaveInterpolationStart();
			}

			// Let's just write out the current animation to the log window
			DoLog(() => currentAnimation.ToString());

			if (reverse)
			{
				// We are working in reverse - this means the the state we're in is
				// the end of the currentAnimation object. Let's find the previous 
				// animation and animate to it! Check if we are at the beginning,
				// if so lets use the interpolation start instead.
				if (currentAnimation != _animationInfos.First())
				{
					var nextAnimation = _animationInfos.ElementAt(_animationInfos.IndexOf(currentAnimation) - 1);

					if (currentAnimation.OnlyTransform)
					{
						Provider.Set(nextAnimation);
						HandleRunCompleted(currentAnimation, reverse, completed, duration, easing);
					}
					else
					{
						// Merge animation info from current animation
						var clonedAnimation = XTransform.FromAnimationInfo(nextAnimation);
						clonedAnimation.OnlyTransform = currentAnimation.OnlyTransform;
						clonedAnimation.Duration = currentAnimation.Duration;
						clonedAnimation.Delay = currentAnimation.Delay;
						clonedAnimation.AnimateColor = currentAnimation.AnimateColor;
						clonedAnimation.AnimateRectangle = currentAnimation.AnimateRectangle;

						Provider.Animate(clonedAnimation, () => HandleRunCompleted(currentAnimation, reverse, 
                                                                                   completed, duration, easing));
					}
				}
				else
				{
					Action doneAction = () =>
					{
						// Handle last animation
						_runningState = false;

						if (completed != null)
							completed();
					};

					EnumerateElements((elementRef) =>
					{
						VisualElement element = null;
						if (elementRef.TryGetTarget(out element))
						{
							var animationInfo = GetInterpolationStart()[elementRef];
							if (currentAnimation.OnlyTransform)
							{
								Provider.Set(element, animationInfo);
							}
							else
							{
								// Merge animation info from current animation
								var clonedAnimation = XTransform.FromAnimationInfo(animationInfo);
								clonedAnimation.OnlyTransform = currentAnimation.OnlyTransform;
								clonedAnimation.Duration = currentAnimation.Duration;
								clonedAnimation.Delay = currentAnimation.Delay;
								clonedAnimation.AnimateColor = currentAnimation.AnimateColor;
								clonedAnimation.AnimateRectangle = currentAnimation.AnimateRectangle;

								Provider.Animate(clonedAnimation, doneAction);
							}
						}
					});

					if (currentAnimation.OnlyTransform)
						doneAction();
				}
			}
			else
			{
				if (currentAnimation.OnlyTransform)
				{
					// Set transformation directly
					Provider.Set(currentAnimation);
					HandleRunCompleted(currentAnimation, reverse, completed, duration, easing);
				}
				else
				{
					// Run transformation
					Provider.Animate(currentAnimation, () =>
						HandleRunCompleted(currentAnimation, reverse, completed, duration, easing));
				}
			}
		}

		/// <summary>
		/// Animation Run completed
		/// </summary>
		void HandleRunCompleted(XTransform currentAnimation, bool reverse, 
                                Action completed, long duration, EasingFunction easing)
		{
			if (reverse)
			{
				// Start previous
				if (currentAnimation != _animationInfos.First() &&
					_runningState)
				{
					var index = _animationInfos.IndexOf(currentAnimation);
					currentAnimation = _animationInfos.ElementAt(index - 1);
					RunAnimation(currentAnimation, reverse, completed, duration, easing);
				}
				else
				{
					_runningState = false;

					if (completed != null)
						completed();
				}
			}
			else
			{
				// Start next
				if (currentAnimation != _animationInfos.Last() &&
					_runningState)
				{
					var index = _animationInfos.IndexOf(currentAnimation);
					currentAnimation = _animationInfos.ElementAt(index + 1);
					RunAnimation(currentAnimation, reverse, completed, duration, easing);
				}
				else
				{
					_runningState = false;

					if (completed != null)
						completed();
				}
			}
		}

		/// <summary>
		/// Log
		/// </summary>
		void DoLog(Func<string> messageCallback)
		{
#if DEBUG
			System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay + " - XAnimation: " + messageCallback());
#endif
		}
        #endregion
    }
}
