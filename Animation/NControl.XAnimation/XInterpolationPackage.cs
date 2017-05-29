using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;

namespace NControl.XAnimation
{
    public class XInterpolationPackage : XTransformationContainer
    {
		#region Private Members

		/// <summary>
		/// State list
		/// </summary>
		readonly Stack<Dictionary<WeakReference<VisualElement>, XTransform>> _states =
			new Stack<Dictionary<WeakReference<VisualElement>, XTransform>>();

		/// <summary>
		/// The interpolation start.
		/// </summary>
		Dictionary<WeakReference<VisualElement>, XTransform> _interpolationStart;

		/// <summary>
		/// Platform specific animation provider
		/// </summary>
		IXAnimationProvider _animationProvider;

		#endregion

        #region Constructors

        /// <summary>
        /// Construtor
        /// </summary>
        public XInterpolationPackage(params VisualElement[] elements) : base(elements)
        {
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Interpolate the animation along the values from 0.0 -> 1.0
        /// </summary>
        public void Interpolate(double value)
        {
            // Save state if no state is found
            if (!HasInterpolationStart)
                SaveInterpolationStart();

            // Total time
            var animationTotalTime = _animationInfos.Sum((arg) => arg.Duration) +
                                     _animationInfos.Sum((arg) => arg.Delay);

            // 1) Find start animation as a function of time and start time
            var animationInfo = GetAnimationInfoFromTime(value);
            var animationInfoStartTime = GetStartTimeForAnimationInfo(animationInfo);

            // 2) Find start/end on 0.0 -> 1.0 
            var startValue = animationInfoStartTime * (1.0 / animationTotalTime);
            var endValue = (animationInfoStartTime + animationInfo.Duration) * (1.0 / animationTotalTime);

            // 3) Find factor to multiply time value with to get from 0 - duration in current 
            //    animation.
            var curValue = (value - startValue) * (animationInfo.Duration / (endValue - startValue)) /
                animationInfo.Duration;

            // 4) Save index of animation for ease of lookup
            var index = _animationInfos.IndexOf(animationInfo);

            // 5) Enumerate and apply, start by getting previous animation
            Dictionary<WeakReference<VisualElement>, XTransform> previousAnimations = GetInterpolationStart();

            for (var i = 0; i <= index && i < _animationInfos.Count; i++)
            {
                var nextPreviousList = new Dictionary<WeakReference<VisualElement>, XTransform>();
                var currentTransform = _animationInfos.ElementAt(i);

                // Get starting point
                EnumerateElements((elementRef) =>
                {
                    // Get element
                    VisualElement element;
                    if (!elementRef.TryGetTarget(out element))
                        return;

                    // Set animation values
                    if (i < index)
                    {
                        Provider.Set(element, currentTransform);
                    }
                    else
                    {
                        // Get start point 
                        XTransform startPoint = null;
                        if (previousAnimations != null)
                            startPoint = previousAnimations[elementRef];

                        if (currentTransform.OnlyTransform)
                        {
							// Just set transform
							Provider.Set(element, currentTransform);
                        }
                        else
                        {
                            // Get interpolated point
                            var interpolatedPoint = GetInterpolatedPoint(
                                startPoint, currentTransform, curValue);

                            Provider.Set(element, interpolatedPoint);
                        }
                    }

                    // Save current state 
                    nextPreviousList.Add(elementRef, _animationInfos.ElementAt(i));
                });

                // Swap out previous
                previousAnimations = nextPreviousList;
            }
        }

		/// <summary>
		/// Saves state for the current animation 
		/// </summary>
		public void Save()
		{
			Dictionary<WeakReference<VisualElement>, XTransform> currentState = null;

			if (_states.Count > 0)
				currentState = _states.Peek();

			_states.Push(GetCurrentStateAsDict(currentState));
		}

		/// <summary>
		/// Pops the current state and restores
		/// </summary>
		public void Restore()
		{
			if (_states.Count == 0)
				return;

			var currentState = _states.Pop();
			EnumerateElements((elementRef) =>
			{
				VisualElement el;
				if (!elementRef.TryGetTarget(out el))
					return;

				if (currentState.ContainsKey(elementRef))
					SetElementFromAnimationInfo(el, currentState[elementRef]);
			});
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

		/// <summary>
		/// Gets a value indicating whether this XTranslationPackage has
		/// interpolation start.
		/// </summary>
		protected bool HasInterpolationStart { get { return _interpolationStart != null; } }

		/// <summary>
		/// Saves the interpoliation starting point
		/// </summary>
		protected void SaveInterpolationStart()
		{
			_interpolationStart = GetCurrentStateAsDict(null);
		}

		/// <summary>
		/// Returns the interpolation starting point if it has been set.
		/// </summary>
		protected Dictionary<WeakReference<VisualElement>, XTransform> GetInterpolationStart()
		{
			return _interpolationStart;
		}

		/// <summary>
		/// Returns the total animation time
		/// </summary>
		/// <returns>The total animation time.</returns>
		protected long GetTotalAnimationTime()
		{
			return _animationInfos.Sum((arg) => arg.Duration) +
				_animationInfos.Sum((arg) => arg.Delay);
		}

		/// <summary>
		/// Returns the animation info from time (0.0 -> 1.0) 
		/// </summary>
		protected XTransform GetAnimationInfoFromTime(double time)
		{
			if (_animationInfos.Count > 1)
			{
				var currentTime = 0L;
				var animationTotalTime = GetTotalAnimationTime();

				var timeInMilliseconds = animationTotalTime * time;

				foreach (var info in _animationInfos)
				{
					if (timeInMilliseconds >= currentTime &&
						timeInMilliseconds <= currentTime + info.Duration)
						return info;

					currentTime += info.Duration + info.Delay;
				}
			}
			else
			{
				return _animationInfos.FirstOrDefault();
			}

			return null;
		}

		#endregion

		#region Private Members

		/// <summary>
		/// Sets the element's values from animation info.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="animationInfo">Animation info.</param>
		void SetElementFromAnimationInfo(VisualElement element, XTransform animationInfo)
		{
			element.Rotation = animationInfo.Rotation;
			element.TranslationX = animationInfo.TranslationX;
			element.TranslationY = animationInfo.TranslationY;
			element.Scale = animationInfo.Scale;
			element.Opacity = animationInfo.Opacity;

			if (animationInfo.AnimateRectangle)
				element.Layout(animationInfo.Rectangle);

			if (animationInfo.AnimateColor)
				element.BackgroundColor = animationInfo.Color;
		}

		/// <summary>
		/// Creates a new interpolated set of points from the two animations where
		/// time is between 0.0 and 1.0.
		/// </summary>
		XTransform GetInterpolatedPoint(XTransform fromAnimation, XTransform toAnimation,
			double time)
		{
			if (fromAnimation == null)
				return new XTransform
				{
					Rotation = toAnimation.Rotation * time,
					Opacity = toAnimation.Opacity * time,
					Scale = toAnimation.Scale * time,
					TranslationX = toAnimation.TranslationX * time,
					TranslationY = toAnimation.TranslationY * time,
					Rectangle = new Rectangle(toAnimation.Rectangle.X * time,
											  toAnimation.Rectangle.Y * time,
											  toAnimation.Rectangle.Width * time,
											  toAnimation.Rectangle.Height * time),
					AnimateRectangle = toAnimation.AnimateRectangle,
					Color = toAnimation.Color,
					AnimateColor = toAnimation.AnimateColor
				};

			return new XTransform
			{
				Rotation = fromAnimation.Rotation +
					   ((toAnimation.Rotation - fromAnimation.Rotation) * time),

				Opacity = fromAnimation.Opacity +
					   ((toAnimation.Opacity - fromAnimation.Opacity) * time),

				TranslationX = fromAnimation.TranslationX +
					 ((toAnimation.TranslationX - fromAnimation.TranslationX) * time),

				TranslationY = fromAnimation.TranslationY +
					 ((toAnimation.TranslationY - fromAnimation.TranslationY) * time),

				Scale = fromAnimation.Scale +
					 ((toAnimation.Scale - fromAnimation.Scale) * time),

				Rectangle = fromAnimation.AnimateRectangle ?
					 new Rectangle(
						fromAnimation.Rectangle.X +
						((toAnimation.Rectangle.X - fromAnimation.Rectangle.X) * time),
						fromAnimation.Rectangle.Y +
						((toAnimation.Rectangle.Y - fromAnimation.Rectangle.Y) * time),
						fromAnimation.Rectangle.Width +
						((toAnimation.Rectangle.Width - fromAnimation.Rectangle.Width) * time),
						fromAnimation.Rectangle.Height +
						((toAnimation.Rectangle.Height - fromAnimation.Rectangle.Height) * time)) :
					 new Rectangle(toAnimation.Rectangle.X * time,
						  toAnimation.Rectangle.Y * time,
						  toAnimation.Rectangle.Width * time,
						  toAnimation.Rectangle.Height * time),

				AnimateRectangle = toAnimation.AnimateRectangle,

				Color = Xamarin.Forms.Color.FromRgba(toAnimation.Color.R +
					 ((toAnimation.Color.R - fromAnimation.Color.R) * time),
					 toAnimation.Color.G +
					 ((toAnimation.Color.G - fromAnimation.Color.G) * time),
					 toAnimation.Color.B +
					 ((toAnimation.Color.B - fromAnimation.Color.B) * time),
					 toAnimation.Color.A +
					 ((toAnimation.Color.A - fromAnimation.Color.A) * time)),

				AnimateColor = toAnimation.AnimateColor,
			};
		}

		/// <summary>
		/// Returns the starttime for a given animation info in the list of infos
		/// </summary>
		protected long GetStartTimeForAnimationInfo(XTransform animationInfo)
		{
			if (_animationInfos.Count > 1)
			{
				var currentTime = 0L;

				foreach (var info in _animationInfos)
				{
					if (info == animationInfo)
						return currentTime;

					currentTime += info.Duration + info.Delay;
				}

				return 0;
			}

			return 0;
		}

		/// <summary>
		/// Gets the current state as dict. Used to save state
		/// </summary>
		Dictionary<WeakReference<VisualElement>, XTransform> GetCurrentStateAsDict(
			Dictionary<WeakReference<VisualElement>, XTransform> currentState)
		{
			var stateDict = new Dictionary<WeakReference<VisualElement>, XTransform>();

			EnumerateElements((elementRef) =>
			{
				VisualElement el;
				if (!elementRef.TryGetTarget(out el))
					return;

				XTransform curInfo = null;
				if (currentState != null && currentState.ContainsKey(elementRef))
					curInfo = currentState[elementRef];

				// Get state 
				var info = GetAnimationInfoFromElement(el, curInfo);
				stateDict.Add(elementRef, info);
			});

			return stateDict;
		}

		/// <summary>
		/// Create animation info from an element
		/// </summary>
		XTransform GetAnimationInfoFromElement(VisualElement element, XTransform animationInfo)
		{
			return new XTransform
			{
				Duration = animationInfo != null ? animationInfo.Duration : 0,
				Delay = animationInfo != null ? animationInfo.Delay : 0,
				//Easing = animationInfo != null ? animationInfo.Easing : EasingFunction.Linear,
				//EasingBezier = animationInfo != null ? animationInfo.EasingBezier : null,
				OnlyTransform = animationInfo != null ? animationInfo.OnlyTransform : true,
				Rotation = element.Rotation,
				TranslationX = element.TranslationX,
				TranslationY = element.TranslationY,
				Scale = element.Scale,
				Opacity = element.Opacity,
				AnimateColor = animationInfo != null ? animationInfo.AnimateColor : false,
				Color = element.BackgroundColor,
				AnimateRectangle = animationInfo != null ? animationInfo.AnimateRectangle : false,
				Rectangle = new Rectangle(element.X, element.Y, element.Width, element.Height),
			};
		}
		#endregion
    }
}
