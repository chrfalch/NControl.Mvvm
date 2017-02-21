using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace NControl.XAnimation
{
	/// <summary>
	/// Implements the abstract part of a native animation
	/// </summary>
	public class XAnimation
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
		readonly IXAnimationProvider _animationProvider;

		/// <summary>
		/// The state of the running.
		/// </summary>
		bool _runningState = false;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new xanimation instance
		/// </summary>
		public XAnimation(params VisualElement[] elements)
		{
			_elements.AddRange(elements.Select(el => new WeakReference<VisualElement>(el)));

			_animationProvider = DependencyService.Get<IXAnimationProvider>(DependencyFetchTarget.NewInstance);
			_animationProvider.Initialize(this);

			DoLog("Created animation for {0}.", string.Join(", ", _elements.Select(el => el.GetType().Name)));
		}
		#endregion

		/// <summary>
		/// Rotates the view around the z axis
		/// </summary>
		public XAnimation Rotate(double rotation)
		{
			DoLog("Rotate({0})", rotation);
			GetCurrentAnimation().Rotate = rotation;
			return this;
		}

		/// <summary>
		/// Fade to the specified opacity (between 0.0 -> 1.0 where 1.0 is non-transparent).
		/// </summary>
		/// <param name="opacity">Opacity.</param>
		public XAnimation Opacity(double opacity)
		{
			DoLog("SetOpacity({0})", opacity);
			GetCurrentAnimation().Opacity = opacity;
			return this;
		}

		/// <summary>
		/// Sets the scale factor
		/// </summary>
		/// <returns>The scale.</returns>
		/// <param name="scale">Scale.</param>
		public XAnimation Scale(double scale)
		{
			DoLog("Scale({0})", scale);
			GetCurrentAnimation().Scale = scale;
			return this;
		}

		/// <summary>
		/// Translate the view x and y pixels
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public XAnimation Translate(double x, double y)
		{
			DoLog("Translate({0}, {1})", x, y );
			GetCurrentAnimation().TranslationX = x;
			GetCurrentAnimation().TranslationY = y;
			return this;
		}


		/// <summary>
		/// Duration the specified milliseconds.
		/// </summary>
		/// <param name="milliseconds">Milliseconds.</param>
		public XAnimation Duration(long milliseconds)
		{
			DoLog("SetDuration({0})", milliseconds);
			GetCurrentAnimation().Duration = milliseconds;
			return this;
		}

		/// <summary>
		/// Resets the transformation
		/// </summary>
		public XAnimation Reset()
		{
			DoLog("Reset()");
			GetCurrentAnimation().Reset();
			return this;
		}

		/// <summary>
		/// Transform according to the animation
		/// </summary>
		public XAnimation Set()
		{
			DoLog("Set()");
			GetCurrentAnimation().OnlyTransform = true;
			var previousAnimation = _animationInfos.LastOrDefault();
			_animationInfos.Add(new XAnimationInfo(previousAnimation));
			return this;
		}

		public XAnimation Animate()
		{
			DoLog("Animate()");
			var previousAnimation = _animationInfos.LastOrDefault();
			_animationInfos.Add(new XAnimationInfo(previousAnimation));

			return this;
		}

		public void Cancel()
		{
			DoLog("Cancel()");
			_runningState = false;
		}

		public XAnimation Then()
		{
			return this;
		}

		/// <summary>
		/// Runs this animation
		/// </summary>
		public void Run(Action completed = null)
		{
			DoLog("Run({0})", (completed == null ? "(null)" : "handler"));

			if (_runningState)
				return;

			if (_animationInfos.Count == 0)
				return;
			
			DoLog("Animate()");

			_runningState = true;

			// Run the first animation in the list of animation objects
			var animationInfo = _animationInfos.First();
			_animationInfos.Remove(animationInfo);
			RunAnimation(animationInfo, completed);
		}

		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		/// <value>The tag.</value>
		public int Tag { get; set; }

		#region Private Members

		/// <summary>
		/// Runs the animation.
		/// </summary>
		/// <param name="animationInfo">Animation info.</param>
		void RunAnimation(XAnimationInfo animationInfo, Action completed)
		{
			DoLog("RunAnimation({0})", animationInfo);

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
				_animationProvider.Set(animationInfo);
				HandleCompletedAction();
			}

			// Tell the animation provider to animate
			_animationProvider.Animate(animationInfo, () =>
			{
				DoLog("Animation Done Callback({0})", animationInfo);

				HandleCompletedAction();
			});
		}

		/// <summary>
		/// Gets the current animation.
		/// </summary>
		/// <returns>The current animation.</returns>
		XAnimationInfo GetCurrentAnimation()
		{
			if (_animationInfos.Count == 0)
				_animationInfos.Add(new XAnimationInfo());			

			return _animationInfos.Last();
		}

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
