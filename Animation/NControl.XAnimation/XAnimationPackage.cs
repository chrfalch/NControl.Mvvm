using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NControl.XAnimation
{
	/// <summary>
	/// Implements the abstract part of a native animation
	/// </summary>
	public class XAnimationPackage
	{
		#region Static Properties

		/// <summary>
		/// Set to true to speed down animations
		/// </summary>
		public static bool SlowAnimations { get; set; }

		#endregion

		#region Private Members

		/// <summary>
		/// List of elements we are animating
		/// </summary>
		readonly List<WeakReference<VisualElement>> _elements = new List<WeakReference<VisualElement>>();

		/// <summary>
		/// List of animation information
		/// </summary>
		readonly List<XAnimationInfo> _animationInfos = new List<XAnimationInfo>();

		/// <summary>
		/// State list
		/// </summary>
		readonly Stack<Dictionary<WeakReference<VisualElement>, XAnimationInfo>> _states =
			new Stack<Dictionary<WeakReference<VisualElement>, XAnimationInfo>>();

		/// <summary>
		/// Platform specific animation provider
		/// </summary>
		IXAnimationProvider _animationProvider;

		/// <summary>
		/// The current info.
		/// </summary>
		XAnimationInfo _currentInfo;
		XAnimationInfo _previousInfo;

		/// <summary>
		/// State of running/not running.
		/// </summary>
		bool _runningState = false;

		/// <summary>
		/// The interpolation start.
		/// </summary>
		Dictionary<WeakReference<VisualElement>, XAnimationInfo> _interpolationStart;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new xanimation instance
		/// </summary>
		public XAnimationPackage(params VisualElement[] elements)
		{
			_elements.AddRange(elements.Select(el => new WeakReference<VisualElement>(el)));
		}
		#endregion

		/// <summary>
		/// Rotates the view around the z axis
		/// </summary>
		public XAnimationPackage Rotate(double rotation)
		{
			GetCurrentAnimation().Rotate = rotation;
			return this;
		}

		public XAnimationPackage Easing(EasingFunction easing)
		{
			GetCurrentAnimation().Easing = easing;
			return this;
		}

		/// <summary>
		/// Fade to the specified opacity (between 0.0 -> 1.0 where 1.0 is non-transparent).
		/// </summary>
		/// <param name="opacity">Opacity.</param>
		public XAnimationPackage Opacity(double opacity)
		{
			GetCurrentAnimation().Opacity = opacity;
			return this;
		}

		/// <summary>
		/// Sets the scale factor
		/// </summary>
		/// <returns>The scale.</returns>
		/// <param name="scale">Scale.</param>
		public XAnimationPackage Scale(double scale)
		{
			GetCurrentAnimation().Scale = scale;
			return this;
		}

		/// <summary>
		/// Translate the view x and y pixels
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public XAnimationPackage Translate(double x, double y)
		{
			GetCurrentAnimation().TranslationX = x;
			GetCurrentAnimation().TranslationY = y;
			return this;
		}

		/// <summary>
		/// Duration the specified milliseconds.
		/// </summary>
		/// <param name="milliseconds">Milliseconds.</param>
		public XAnimationPackage Duration(long milliseconds)
		{
			GetCurrentAnimation().Duration = milliseconds;
			return this;
		}

		/// <summary>
		/// Resets the transformation
		/// </summary>
		public XAnimationPackage Reset()
		{
			_previousInfo = null;
			_currentInfo = null;
			GetCurrentAnimation().Reset();
			return this;
		}

		/// <summary>
		/// Transform according to the animation
		/// </summary>
		public XAnimationPackage Set()
		{
			_previousInfo = _animationInfos.LastOrDefault();
			_currentInfo = null;
			if (_previousInfo != null)
				_previousInfo.OnlyTransform = true;

			return this;
		}

		public XAnimationPackage Animate()
		{
			_previousInfo = _animationInfos.LastOrDefault();
			_currentInfo = null;
			return this;
		}

		public XAnimationPackage Color(Color color)
		{
			GetCurrentAnimation().AnimateColor = true;
			GetCurrentAnimation().Color = color;
			return this;
		}

		//public XAnimationPackage Frame(Rectangle rect)
		//{
		//	GetCurrentAnimation().AnimateRectangle = true;
		//	GetCurrentAnimation().Rectangle = rect;
		//	return this;
		//}

		/// <summary>
		/// Creates a custom easing curve. See more here: http://cubic-bezier.com
		/// </summary>
		public XAnimationPackage Easing(Point start, Point end)
		{
			GetCurrentAnimation().Easing = EasingFunction.Custom;
			GetCurrentAnimation().EasingBezier = new EasingFunctionBezier(start, end);
			return this;
		}

		/// <summary>
		/// Creates a custom easing curve. See more here: http://cubic-bezier.com
		/// </summary>
		public XAnimationPackage Easing(double startX, double startY, double endX, double endY)
		{
			return Easing(new Point(startX, startY), new Point(endX, endY));
		}

		/// <summary>
		/// Creates a custom easing curve. See more here: http://cubic-bezier.com
		/// </summary>
		public XAnimationPackage Easing(EasingFunctionBezier easingFunction)
		{
			return Easing(easingFunction.Start, easingFunction.End);
		}

		public XAnimationPackage Then()
		{
			return this;
		}

		/// <summary>
		/// Saves state for the current animation 
		/// </summary>
		public void Save()
		{
			Dictionary<WeakReference<VisualElement>, XAnimationInfo> currentState = null;

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
			foreach (var elementRef in _elements)
			{
				VisualElement el;
				if (!elementRef.TryGetTarget(out el))
					continue;

				if (currentState.ContainsKey(elementRef))
					SetElementFromAnimationInfo(el, currentState[elementRef]);
			}
		}

		/// <summary>
		/// Runs this animation
		/// </summary>
		public void Run(Action completed = null)
		{
			if (_runningState)
				return;

			if (_animationInfos.Count == 0)
				return;

			_runningState = true;

			RunAnimation(_animationInfos.First(), completed);
		}

		/// <summary>
		/// Interpolate the animation along the values from 0.0 -> 1.0
		/// </summary>
		public void Interpolate(double value)
		{
			// Save state if no state is found
			if (_interpolationStart == null)
				_interpolationStart = GetCurrentStateAsDict(null);

			// Total time
			var animationTotalTime = _animationInfos.Sum((arg) => arg.Duration);

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

			//System.Diagnostics.Debug.WriteLine(
			//	$"{curValue:F2} == ({value:F2} - {startValue:F2}) * ({animationInfo.Duration:F2} " +
			//    $" / ({endValue:F2} - {startValue:F2})) / {animationInfo.Duration:F2} (index: {index})");


			// 5) Enumerate and apply, start by getting previous animation
			Dictionary<WeakReference<VisualElement>, XAnimationInfo> previousAnimations = _interpolationStart;

			for (var i = 0; i <= index && i <_animationInfos.Count; i++)
			{
				var nextPreviousList = new Dictionary<WeakReference<VisualElement>, XAnimationInfo>();
				var currentAnimation = _animationInfos.ElementAt(i);

				// Get starting point
				foreach (var elementRef in _elements)
				{
					// Get element
					VisualElement element;
					if (!elementRef.TryGetTarget(out element))
						continue;

					// Set animation values
					if (i < index)
					{
						Provider.Set(element, currentAnimation);
					}
					else
					{
						// Get start point 
						XAnimationInfo startPoint = null;
						if(previousAnimations != null)
							startPoint = previousAnimations[elementRef];

						// Get interpolated point
						var interpolatedPoint = GetInterpolatedPoint(
							startPoint, currentAnimation, curValue);

						Provider.Set(element, interpolatedPoint);
					}

					// Save current state 
					nextPreviousList.Add(elementRef, _animationInfos.ElementAt(i));
				}

				// Swap out previous
				previousAnimations = nextPreviousList;
			}
		}

		/// <summary>
		/// Runs the animations async
		/// </summary>
		public Task RunAsync()
		{
			var tcs = new TaskCompletionSource<bool>();
			Run(() => tcs.TrySetResult(true));
			return tcs.Task;
		}

		/// <summary>
		/// Run multiple animations and return with completed action when all are done.
		/// </summary>
		public static void RunAll(IEnumerable<XAnimationPackage> animations, Action completed = null)
		{
			if (animations == null || animations.Count() == 0)
			{
				if (completed != null)
					completed();

				return;
			}

			var counter = 0;
			foreach (var animation in animations)
			{
				animation.Run(() =>
				{
					counter++;
					if (counter == animations.Count() && completed != null)
						completed();
				});
			}
		}

		/// <summary>
		/// Run multiple animations async
		/// </summary>
		public static Task RunAllAsync(IEnumerable<XAnimationPackage> animations)
		{
			var tcs = new TaskCompletionSource<bool>();
			RunAll(animations, () => tcs.TrySetResult(true));
			return tcs.Task;
		}

		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		/// <value>The tag.</value>
		public int Tag { get; set; }

		/// <summary>
		/// Returns the animation info list
		/// </summary>
		public IEnumerable<XAnimationInfo> AnimationInfos { get { return _animationInfos; } }

		#region Private Members

		/// <summary>
		/// Enumerate elements
		/// </summary>
		void EnumerateElements(Action<VisualElement> callback)
		{
			foreach (var elementRef in _elements)
			{
				// Get element
				VisualElement element;
				if (!elementRef.TryGetTarget(out element))
					continue;

				callback(element);
			}
		}

		/// <summary>
		/// Gets the current state as dict. Used to save state
		/// </summary>
		Dictionary<WeakReference<VisualElement>, XAnimationInfo> GetCurrentStateAsDict(
			Dictionary<WeakReference<VisualElement>, XAnimationInfo> currentState)
		{
			var stateDict = new Dictionary<WeakReference<VisualElement>, XAnimationInfo>();

			foreach (var elementRef in _elements)
			{
				VisualElement el;
				if (!elementRef.TryGetTarget(out el))
					continue;

				XAnimationInfo curInfo = null;
				if (currentState != null && currentState.ContainsKey(elementRef))
					curInfo = currentState[elementRef];

				// Get state 
				var info = GetAnimationInfoFromElement(el, curInfo);
				stateDict.Add(elementRef, info);
			}

			return stateDict;
		}

		/// <summary>
		/// Runs the animation from startAnimation to endAnimation
		/// </summary>
		void RunAnimation(XAnimationInfo currentAnimation, Action completed)
		{
			// If no views are live we can just return without calling completed,
			// user interface is no longer available
			if (!Provider.GetHasViewsToAnimate(currentAnimation))
				return;
			
			// Save state if no state is found
			if (_interpolationStart == null)
				_interpolationStart = GetCurrentStateAsDict(null);

			DoLog(() => currentAnimation.ToString());

			if (currentAnimation.OnlyTransform)
			{				
				// Set transformation directly
				Provider.Set(currentAnimation);
				HandleRunCompleted(currentAnimation, completed);
			}
			else
			{
				// Run transformation
                Provider.Animate(currentAnimation, ()=> 
                	HandleRunCompleted(currentAnimation, completed));
			}
		}

		/// <summary>
		/// Animation Run completed
		/// </summary>
		void HandleRunCompleted(XAnimationInfo currentAnimation, Action completed)
		{
			// Start next
			if (currentAnimation != _animationInfos.Last() && 			    
			    _runningState)
			{
				var index = _animationInfos.IndexOf(currentAnimation);
				currentAnimation = _animationInfos.ElementAt(index + 1);
				RunAnimation(currentAnimation, completed);
			}
			else
			{
				_runningState = false;

				if (completed != null)
					completed();
			}
		}

		/// <summary>
		/// Gets the current animation.
		/// </summary>
		XAnimationInfo GetCurrentAnimation()
		{
			if (_animationInfos.Count == 0 || _currentInfo == null)
			{
				_currentInfo = new XAnimationInfo(_previousInfo);
				_animationInfos.Add(_currentInfo);
				_previousInfo = null;
			}

			return _animationInfos.Last();
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

		#region Helpers

		/// <summary>
		/// Returns the animation info from time (0.0 -> 1.0) 
		/// </summary>
		XAnimationInfo GetAnimationInfoFromTime(double time)
		{
			if (_animationInfos.Count > 1)
			{
				var currentTime = 0L;
				var animationTotalTime = _animationInfos.Sum((arg) => arg.Duration);
				var timeInMilliseconds = animationTotalTime * time;

				foreach (var info in _animationInfos)
				{
					if (timeInMilliseconds >= currentTime &&
						timeInMilliseconds <= currentTime + info.Duration)					
						return info;

					currentTime += info.Duration;
				}
			}
			else
			{
				return _animationInfos.FirstOrDefault();
			}

			return null;
		}

		/// <summary>
		/// Returns the starttime for a given animation info in the list of infos
		/// </summary>
		long GetStartTimeForAnimationInfo(XAnimationInfo animationInfo)
		{
			if (_animationInfos.Count > 1)
			{
				var currentTime = 0L;

				foreach (var info in _animationInfos)
				{
					if (info == animationInfo)
						return currentTime;
					
					currentTime += info.Duration;
				}

				return 0;
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Create animation info from an element
		/// </summary>
		XAnimationInfo GetAnimationInfoFromElement(VisualElement element, XAnimationInfo animationInfo)
		{
			return new XAnimationInfo
			{
				Duration = animationInfo != null ? animationInfo.Duration : 0,
				Delay = animationInfo != null ? animationInfo.Delay : 0,
				Easing = animationInfo != null ? animationInfo.Easing : EasingFunction.Linear,
				EasingBezier = animationInfo != null ? animationInfo.EasingBezier : null,
				OnlyTransform = animationInfo != null ? animationInfo.OnlyTransform : true,
				Rotate = element.Rotation,
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

		/// <summary>
		/// Sets the element's values from animation info.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="animationInfo">Animation info.</param>
		void SetElementFromAnimationInfo(VisualElement element, XAnimationInfo animationInfo)
		{
			element.Rotation = animationInfo.Rotate;
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
		XAnimationInfo GetInterpolatedPoint(XAnimationInfo fromAnimation, XAnimationInfo toAnimation,
        	double time)
		{
			if (fromAnimation == null)
				return new XAnimationInfo
				{
					Rotate = toAnimation.Rotate* time,
					Opacity = toAnimation.Opacity * time,
					TranslationX = toAnimation.TranslationX * time,
					TranslationY = toAnimation.TranslationY * time,
				};

			return new XAnimationInfo
			{
				Rotate = fromAnimation.Rotate +
					   ((toAnimation.Rotate - fromAnimation.Rotate) * time),

				Opacity = fromAnimation.Opacity +
					   ((toAnimation.Opacity - fromAnimation.Opacity) * time),

				TranslationX = fromAnimation.TranslationX +
					 ((toAnimation.TranslationX - fromAnimation.TranslationX) * time),

				TranslationY = fromAnimation.TranslationY +
					 ((toAnimation.TranslationY - fromAnimation.TranslationY) * time),
			};
		}
		#endregion

		#region Properties

		/// <summary>
		/// Returns the initialized animation provider
		/// </summary>
		IXAnimationProvider Provider
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
		/// Returns the number of elements
		/// </summary>
		/// <value>The element count.</value>
		public int ElementCount
		{
			get
			{
				return _elements.Count();
			}
		}

		/// <summary>
		/// Returns the elements that should be animated
		/// </summary>
		public VisualElement GetElement(int index)
		{ 
			VisualElement el2 = null;
			if (_elements.ElementAt(index).TryGetTarget(out el2))
				return el2;

			return null;
		} 

		#endregion
	}

	
}
