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
			Duration = 250;
        }

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		public long Duration{ get;set; }

		public XAnimationPackage SetDuration(long duration)
		{
			Duration = duration;
			return this;
		}

		public bool IsRunning => _runningState;

		#endregion

		#region Public Static Members

		/// <summary>
		/// Runs all animation packages in paralell and returns when they
		/// are done.
		/// </summary>
		public static void RunAll(IEnumerable<XAnimationPackage> packages, Action completed)
		{
			if (!packages.Any())
			{
				completed?.Invoke();
				return;
			}

			var animationCount = packages.Count();

			foreach (var package in packages)
			{
				package.Animate(() => {
					animationCount--;
					if (animationCount == 0)
						completed?.Invoke();
					
				});
			}
		}

		#endregion

        #region Public Members

        /// <summary>
        /// Animates the contained transformations on the list of visual elements 
        /// </summary>
        public void Animate(Action completed = null)
        {
            if (_runningState)
                return;

            if (_transforms.Count == 0)
                return;

            _runningState = true;

			RunAnimation(_transforms.First(), false, completed);
        }

        /// <summary>
        /// Runs the animations async
        /// </summary>
        public Task AnimateAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            Animate(() => tcs.TrySetResult(true));
            return tcs.Task;
        }

        /// <summary>
        /// Runs the animation in reverse
        /// </summary>
        public void AnimateReverse(Action completed = null, long duration = 250)
        {
            if (_runningState)
                return;

            if (_transforms.Count == 0)
                return;

            _runningState = true;

			RunAnimation(_transforms.Last(), true, completed);
        }

		public bool HasViewsToAnimate => Provider.GetHasViewsToAnimate();

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

		#region Static Members

		public static void RunAll()
		{
		}
		#endregion

		#region Private Members

		/// <summary>
		/// Runs the animation from startAnimation to endAnimation
		/// </summary>
        void RunAnimation(XTransform currentTransform, bool reverse, 
      		Action completed)
		{
			// If no views are live we can just return without calling completed,
			// user interface is no longer available
			if (!Provider.GetHasViewsToAnimate())
			{
				_runningState = false;
				return;
			}

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
			var calculatedDuration = GetCalculatedDuration(currentTransform, Duration);

			if (reverse)
			{
				// We are working in reverse - this means the the state we're in is
				// the end of the currentAnimation object. Let's find the previous 
				// animation and animate to it! Check if we are at the beginning,
				// if so lets use the interpolation start instead.
				if (currentTransform != _transforms.First())
				{
					var nextAnimation = _transforms.ElementAt(
						_transforms.IndexOf(currentTransform) - 1);

					if (currentTransform.OnlyTransform)
					{
						Provider.Set(nextAnimation);
						HandleRunCompleted(currentTransform, reverse, completed);
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
                       		completed), calculatedDuration);
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

								Provider.Animate(clonedAnimation, doneAction, calculatedDuration);
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
					HandleRunCompleted(currentTransform, reverse, completed);
				}
				else
				{					
					// Run transformation
					Provider.Animate(currentTransform, () =>
						HandleRunCompleted(currentTransform, reverse, completed),
		                calculatedDuration);
				}
			}
		}

		/// <summary>
		/// Animation Run completed
		/// </summary>
		void HandleRunCompleted(XTransform currentAnimation, bool reverse, 
        	Action completed)
		{
			if (reverse)
			{
				// Start previous
				if (currentAnimation != _transforms.First() &&
					_runningState)
				{
					var index = _transforms.IndexOf(currentAnimation);
					currentAnimation = _transforms.ElementAt(index - 1);
					RunAnimation(currentAnimation, reverse, completed);
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
				if (currentAnimation != _transforms.Last() &&
					_runningState)
				{
					var index = _transforms.IndexOf(currentAnimation);
					currentAnimation = _transforms.ElementAt(index + 1);
					RunAnimation(currentAnimation, reverse, completed);
				}
				else
				{
					_runningState = false;

					if (completed != null)
						completed();
				}
			}
		}

		long GetCalculatedDuration(XTransform currentTransform, long duration)
		{
			if (currentTransform.OnlyTransform)
				return 0;
			
			var totalLength = GetTotalAnimationTime();
			var thisFactorOfTotal = totalLength / currentTransform.Duration;
			return duration / thisFactorOfTotal;
		}

		static double Normalize(double value, double min, double max)
		{
			return (value - min) / (max - min);
		}

		/// <summary>
		/// Log
		/// </summary>
		void DoLog(Func<string> messageCallback)
		{
			return;
#if DEBUG
			System.Diagnostics.Debug.WriteLine(
				DateTime.Now.TimeOfDay + " - XAnimation: " + messageCallback());
#endif
		}
        #endregion
    }
}
