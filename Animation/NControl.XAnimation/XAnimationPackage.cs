﻿using System;
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
			GetCurrentAnimation().Color = color;
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

		public XAnimationPackage Then()
		{
			return this;
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

			if (_animationProvider == null)
			{
				_animationProvider = DependencyService.Get<IXAnimationProvider>(DependencyFetchTarget.NewInstance);
				_animationProvider.Initialize(this);
			}

			if (animationInfo.OnlyTransform)
			{
				DoLog("SetTransformation({0})", animationInfo);
				_animationProvider.Set(animationInfo);
				HandleCompletedAction();
			}
			else
			{
				DoLog("RunAnimation({0})", animationInfo);

				// Tell the animation provider to animate
				_animationProvider.Animate(animationInfo, () =>
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
		void DoLog(string message, params object[] args)
		{			
			var ticks = DateTime.Now;				
			System.Diagnostics.Debug.WriteLine(ticks.TimeOfDay + " - XAnimation: " + string.Format(message, args));
		}

		#endregion

		#region Properties

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
