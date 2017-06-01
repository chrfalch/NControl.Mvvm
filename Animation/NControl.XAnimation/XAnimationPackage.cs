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

		/// <summary>
		/// Platform specific animation provider
		/// </summary>
		IXAnimationProvider _animationProvider;

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
        public void Animate(Action completed = null, long duration = 250, 
			EasingFunctionBezier easing = null)
        {
            if (_runningState)
                return;

            if (_animationInfos.Count == 0)
                return;

            _runningState = true;

			if (easing == null)
				easing = EasingFunctions.Linear;

            RunAnimation(_animationInfos.First(), false, completed, duration, easing);
        }

        /// <summary>
        /// Runs the animations async
        /// </summary>
        public Task AnimateAsync(long duration = 250, EasingFunctionBezier easing = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            Animate(() => tcs.TrySetResult(true), duration, easing);
            return tcs.Task;
        }

        /// <summary>
        /// Runs the animation in reverse
        /// </summary>
        public void AnimateReverse(Action completed = null, long duration = 250, EasingFunctionBezier easing = null)
        {
            if (_runningState)
                return;

            if (_animationInfos.Count == 0)
                return;

            _runningState = true;

			if (easing == null)
				easing = EasingFunctions.Linear;
			
            RunAnimation(_animationInfos.Last(), true, completed, duration, easing);
        }

		#endregion

		#region Protected Members

		/// <summary>
		/// Returns the initialized animation provider
		/// </summary>
		protected IXAnimationProvider Provider
		{
			get
			{
				if (_animationProvider == null)
				{
					_animationProvider = DependencyService.Get<IXAnimationProvider>(
						DependencyFetchTarget.NewInstance);

					_animationProvider.Initialize(this);
				}

				return _animationProvider;
			}
		}

		#endregion

		#region Private Members

		/// <summary>
		/// Runs the animation from startAnimation to endAnimation
		/// </summary>
        void RunAnimation(XTransform currentTransform, bool reverse, 
      		Action completed, long duration, EasingFunctionBezier easing)
		{
			// If no views are live we can just return without calling completed,
			// user interface is no longer available
			if (!Provider.GetHasViewsToAnimate(currentTransform))
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
			DoLog(() => currentTransform.ToString());

			// Find calculated duration
			var calculatedDuration = GetCalculatedDuration(currentTransform, duration);

			// Find part of easing function
			var easingFunction = GetEasingFunction(currentTransform, easing, duration, calculatedDuration);

			if (reverse)
			{
				// We are working in reverse - this means the the state we're in is
				// the end of the currentAnimation object. Let's find the previous 
				// animation and animate to it! Check if we are at the beginning,
				// if so lets use the interpolation start instead.
				if (currentTransform != _animationInfos.First())
				{
					var nextAnimation = _animationInfos.ElementAt(_animationInfos.IndexOf(currentTransform) - 1);

					if (currentTransform.OnlyTransform)
					{
						Provider.Set(nextAnimation);
						HandleRunCompleted(currentTransform, reverse, completed, duration, easing);
					}
					else
					{
						// Merge animation info from current animation
						var clonedAnimation = XTransform.FromAnimationInfo(nextAnimation);
						clonedAnimation.OnlyTransform = currentTransform.OnlyTransform;
						clonedAnimation.Duration = currentTransform.Duration;
						clonedAnimation.Delay = currentTransform.Delay;
						clonedAnimation.AnimateColor = currentTransform.AnimateColor;
						clonedAnimation.AnimateRectangle = currentTransform.AnimateRectangle;

						Provider.Animate(clonedAnimation, () => HandleRunCompleted(currentTransform, reverse, 
                       		completed, duration, easing), calculatedDuration, easingFunction);
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
							if (currentTransform.OnlyTransform)
							{
								Provider.Set(element, animationInfo);
							}
							else
							{
								// Merge animation info from current animation
								var clonedAnimation = XTransform.FromAnimationInfo(animationInfo);
								clonedAnimation.OnlyTransform = currentTransform.OnlyTransform;
								clonedAnimation.Duration = currentTransform.Duration;
								clonedAnimation.Delay = currentTransform.Delay;
								clonedAnimation.AnimateColor = currentTransform.AnimateColor;
								clonedAnimation.AnimateRectangle = currentTransform.AnimateRectangle;

								Provider.Animate(clonedAnimation, doneAction, calculatedDuration, easingFunction);
							}
						}
					});

					if (currentTransform.OnlyTransform)
						doneAction();
				}
			}
			else
			{
				if (currentTransform.OnlyTransform)
				{
					// Set transformation directly
					Provider.Set(currentTransform);
					HandleRunCompleted(currentTransform, reverse, completed, duration, easing);
				}
				else
				{
					// Run transformation
					Provider.Animate(currentTransform, () =>
						HandleRunCompleted(currentTransform, reverse, completed, duration, easing),
		                calculatedDuration, easingFunction);
				}
			}
		}

		/// <summary>
		/// Animation Run completed
		/// </summary>
		void HandleRunCompleted(XTransform currentAnimation, bool reverse, 
        	Action completed, long duration, EasingFunctionBezier easing)
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


		EasingFunctionBezier GetEasingFunction(XTransform currentTransform, EasingFunctionBezier easing, 
       		long totalDuration, long calculatedTransformDuration)
		{
			var transformStartTime = GetStartTimeForTranslation(currentTransform);
			if (transformStartTime > 0)
			{
				var totalLogicalLength = GetTotalAnimationTime();
				var thisFactorOfTotal = totalLogicalLength / transformStartTime;
				transformStartTime = totalDuration / thisFactorOfTotal;
			}

			var startValue = transformStartTime * (1.0 / totalDuration);
			var endValue = (transformStartTime + calculatedTransformDuration) * (1.0 / totalDuration);

			if (startValue.Equals(0.0))
				return easing.Split(endValue).Item1;

			if (endValue.Equals(1.0))
				return easing.Split(startValue).Item2;
			
			var easings = easing.Split(startValue);
			var newEasing = easings.Item2;
			var finalEasing = newEasing.Split(endValue).Item1;

			return finalEasing;
		}

		long GetCalculatedDuration(XTransform currentTransform, long duration)
		{
			var totalLength = GetTotalAnimationTime();
			var thisFactorOfTotal = totalLength / currentTransform.Duration;
			return duration / thisFactorOfTotal;
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
