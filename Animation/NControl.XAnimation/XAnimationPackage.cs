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

		public XAnimationPackage Frame(Rectangle rect)
		{
			GetCurrentAnimation().AnimateRectangle = true;
			GetCurrentAnimation().Rectangle = rect;
			return this;
		}

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
			var stateDict = new Dictionary<WeakReference<VisualElement>, XAnimationInfo>();

			Dictionary<WeakReference<VisualElement>, XAnimationInfo> currentState = null;
			if (_states.Count > 0)
				currentState = _states.Peek();

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

			_states.Push(stateDict);
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
		/// Set to true to speed down animations
		/// </summary>
		public static bool SlowAnimations { get; set; }

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

			// Run the first animation in the list of animation objects
			var animationInfo = _animationInfos.First();
			_animationInfos.Remove(animationInfo);
			RunAnimation(animationInfo, completed);
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
		/// Interpolate the animation along the values from 0.0 -> 1.0
		/// </summary>
		public void Interpolate(double value)
		{
			// State
			if (_states.Count == 0)
				Save();

			var currentState = _states.Peek();

			// Find animations
			var animationInfo = _animationInfos.Last();
			if (_animationInfos.Count > 1)
			{
				var animationStartTime = _animationInfos.Sum((arg) => arg.Duration) * value;
				var timeCounter = 0.0;
				foreach (var info in _animationInfos)
				{
					if (animationStartTime >= timeCounter &&
						animationStartTime <= timeCounter + info.Duration)
					{
						animationInfo = info;
						break;
					}

					timeCounter += info.Duration;
				}
			}

			var index = _animationInfos.IndexOf(animationInfo);
			System.Diagnostics.Debug.WriteLine(index + " - " + animationInfo);

			// Get starting point
			foreach (var elementRef in _elements)
			{
				// Get element
				VisualElement element;
				if (!elementRef.TryGetTarget(out element))
					continue;

				// Get state for element
				if (!currentState.ContainsKey(elementRef))
					continue;

				XAnimationInfo startPoint = currentState[elementRef];
				if (index > 0)
					startPoint = _animationInfos.ElementAt(index - 1);

				// Create interpolated point
				var interpolatedPoint = new XAnimationInfo
				{

					Rotate = startPoint.Rotate +
						   ((animationInfo.Rotate - startPoint.Rotate) * value),

					Opacity = startPoint.Opacity +
						   ((animationInfo.Opacity - startPoint.Opacity) * value),

					TranslationX = startPoint.TranslationX +
						 ((animationInfo.TranslationX - startPoint.TranslationX) * value),

					TranslationY = startPoint.TranslationY +
						 ((animationInfo.TranslationY - startPoint.TranslationY) * value),
				};

				// Interpolate with previous elements

				Provider.Set(element, interpolatedPoint);
			}
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
		/// Runs the animation.
		/// </summary>
		/// <param name="animationInfo">Animation info.</param>
		void RunAnimation(XAnimationInfo animationInfo, Action completed)
		{
			Action HandleCompletedAction = () =>
			{
				// Start next
				if (_animationInfos.Count > 0 && _runningState)
				{
					animationInfo = _animationInfos.First();
					_animationInfos.Remove(animationInfo);
					RunAnimation(animationInfo, completed);
				}
				else
				{
					_runningState = false;
					if (completed != null)
						completed();
				}
			};

			if (animationInfo.OnlyTransform)
			{
				DoLog(()=> animationInfo.ToString());
				Provider.Set(animationInfo);
				HandleCompletedAction();
			}
			else
			{
                DoLog(() => animationInfo.ToString());

				// Tell the animation provider to animate
				Provider.Animate(animationInfo, () =>
				{
					// DoLog("Animation Done Callback({0})", animationInfo);
					HandleCompletedAction();
				});
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
		/// Returns the elements that should be animated
		/// </summary>
		public IEnumerable<VisualElement> Elements 
		{ 
			get
			{
				return _elements.Select(el =>
				{
					VisualElement el2 = null;
					if (el.TryGetTarget(out el2))
						return el2;

					return null;
				});
			}				                       
		} 

		#endregion
	}

	
}
