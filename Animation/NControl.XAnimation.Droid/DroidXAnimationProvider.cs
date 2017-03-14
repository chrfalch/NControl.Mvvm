using System;
using System.Collections.Generic;
using Android.Animation;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Views.Animations;
using NControl.XAnimation.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(DroidXAnimationProvider))]

namespace NControl.XAnimation.Droid
{
	/// <summary>
	/// Android implementation of the animation provider
	/// </summary>
	public class DroidXAnimationProvider: IXAnimationProvider
	{
		#region Private Members
		float _displayDensity = 1.0f;
		XAnimationPackage _animation;
		#endregion

		public void Initialize(XAnimationPackage animation)
		{
			_animation = animation;

			// Get display density to fix pixel scaling
			using (var metrics = Xamarin.Forms.Forms.Context.Resources.DisplayMetrics)
				_displayDensity = metrics.Density;			
		}

		public void Animate(XAnimationInfo animationInfo, Action completed)
		{
			var animations = new List<object>();
			var animationCount = 0;

			foreach (var element in _animation.Elements)
			{
				var viewGroup = GetViewGroup(element);
				var nativeAnimation = viewGroup.Animate();
				nativeAnimation
					.SetDuration(animationInfo.Duration)
					.SetStartDelay(animationInfo.Delay)
					.SetInterpolator(GetInterpolator(animationInfo))
					.SetListener(new AnimationListener(null, (obj) => 
				{
					animationCount--;
					if(animationCount == 0)
						completed?.Invoke();

				}, null, null));

				nativeAnimation.Alpha((float)animationInfo.Opacity);
				nativeAnimation.Rotation((float)animationInfo.Rotate);
				nativeAnimation.ScaleX((float)animationInfo.Scale);
				nativeAnimation.ScaleY((float)animationInfo.Scale);
				nativeAnimation.TranslationX((float)animationInfo.TranslationX * _displayDensity);
				nativeAnimation.TranslationY((float)animationInfo.TranslationY * _displayDensity);

				if (element.BackgroundColor != animationInfo.Color)
				{
					var fromColor = (viewGroup.Background as ColorDrawable)?.Color;
					var toColor = animationInfo.Color.ToAndroid();

					var colorAnimator = ObjectAnimator.OfInt(viewGroup, "backgroundColor", (int)fromColor, (int)toColor);
					colorAnimator.SetDuration(animationInfo.Duration);
					colorAnimator.SetTarget(viewGroup);
					colorAnimator.SetEvaluator(new ArgbEvaluator());
					colorAnimator.SetInterpolator(GetInterpolator(animationInfo));
					colorAnimator.StartDelay = animationInfo.Delay;
					colorAnimator.AddListener(new AnimationListener(null, (obj) =>
					{
						animationCount--;
						if (animationCount == 0)
							completed?.Invoke();

					}, null, null));

					animations.Add(colorAnimator);
				}

				animations.Add(nativeAnimation);
			}

			animationCount = animations.Count;
			foreach (var anim in animations)
			{
				if (anim is ViewPropertyAnimator)
					(anim as ViewPropertyAnimator).Start();
				else if (anim is ObjectAnimator)
					(anim as ObjectAnimator).Start();
			}
		}

		public void Set(XAnimationInfo animationInfo)
		{
			foreach (var element in _animation.Elements)
			{
				var viewGroup = GetViewGroup(element);

				viewGroup.Alpha = (float)animationInfo.Opacity;
				viewGroup.Rotation = (float)animationInfo.Rotate;
				viewGroup.ScaleX = (float)animationInfo.Scale;
				viewGroup.ScaleY = (float)animationInfo.Scale;
				viewGroup.TranslationX = (float)animationInfo.TranslationX * _displayDensity;
				viewGroup.TranslationY = (float)animationInfo.TranslationY * _displayDensity;
				viewGroup.SetBackgroundColor( animationInfo.Color.ToAndroid());
			}
		}

		#region Private Members

		/// <summary>
		/// Gets the view.
		/// </summary>
		/// <returns>The view.</returns>
		ViewGroup GetViewGroup(VisualElement element)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
			{
				renderer = Platform.CreateRenderer(element);
				Platform.SetRenderer(element, renderer);
			}

			return renderer.ViewGroup;
		
		}
		#endregion

		ITimeInterpolator GetInterpolator(XAnimationInfo animationInfo)
		{
			switch (animationInfo.Easing)
			{
				case EasingFunction.EaseIn:
					return new AccelerateInterpolator();					
					
				case EasingFunction.EaseOut:
					return new DecelerateInterpolator();					
					
				case EasingFunction.EaseInOut:
					return new AccelerateDecelerateInterpolator();
					
				case EasingFunction.Custom:
					return new DroidBezierInterpolator(
					(float)animationInfo.EasingBezier.Start.X, (float)animationInfo.EasingBezier.Start.Y,
					(float)animationInfo.EasingBezier.End.X, (float)animationInfo.EasingBezier.End.Y);

				default:
					return new LinearInterpolator();
			}
		}
	}

	class AnimationListener : Java.Lang.Object, Animator.IAnimatorListener
	{
		Action<Animator> _cancelAction;
		Action<Animator> _endAction;
		Action<Animator> _repeatAction;
		Action<Animator> _startAction;

		public AnimationListener(Action<Animator>  start = null, Action<Animator> end = null, 
		                         Action<Animator> cancel = null, Action<Animator> repeat = null)
		{
			_startAction = start;
			_endAction = end;
			_cancelAction = cancel;
			_repeatAction = repeat;
		}

		public void OnAnimationCancel(Animator animation)
		{
			if (_cancelAction != null)
				_cancelAction(animation);
		}

		public void OnAnimationEnd(Animator animation)
		{
			if (_endAction != null)
				_endAction(animation);
		}

		public void OnAnimationRepeat(Animator animation)
		{
			if (_repeatAction != null)
				_repeatAction(animation);
		}

		public void OnAnimationStart(Animator animation)
		{
			if (_startAction != null)
				_startAction(animation);
		}
	}
}
