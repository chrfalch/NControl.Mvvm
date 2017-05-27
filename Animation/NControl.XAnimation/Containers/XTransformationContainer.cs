using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace NControl.XAnimation
{
    public abstract class XTransformationContainer : XElementContainer
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
        /// List of animation information
        /// </summary>
        protected readonly List<XTransform> _animationInfos = new List<XTransform>();

        /// <summary>
        /// Platform specific animation provider
        /// </summary>
        IXAnimationProvider _animationProvider;

        #endregion

        public XTransformationContainer(params VisualElement[] elements) : base(elements)
        {
        }

        #region Transformation Container Members

        /// <summary>
        /// Adds a new transform to the container
        /// </summary>
		public XTransformationContainer Add(Action<XTransform> transformSetup)
		{
			if (transformSetup == null)
				throw new ArgumentException(nameof(transformSetup));

			XTransform retVal = null;
			if (_animationInfos.Any())
				retVal = new XTransform(_animationInfos.Last(), true);
			else
				retVal = new XTransform();

			transformSetup(retVal);

			_animationInfos.Add(retVal);

			return this;
        }

		/// <summary>
		/// Add the specified transform.
		/// </summary>
		public XTransformationContainer Set(Action<XTransform> transformSetup)
		{
			if (transformSetup == null)
				throw new ArgumentException(nameof(transformSetup));

			XTransform retVal = null;
			if (_animationInfos.Any())
				retVal = new XTransform(_animationInfos.Last(), true);
			else
				retVal = new XTransform();

			retVal.SetOnlyTransform(true);
			transformSetup(retVal);

			_animationInfos.Add(retVal);

			return this;
		}

		/// <summary>
		/// Adds a reset transformation
		/// </summary>
		public XTransformationContainer Reset()
		{
			_animationInfos.Add(new XTransform(null, false));

			return this;
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
		        
		#endregion

		#region Protected Members

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
