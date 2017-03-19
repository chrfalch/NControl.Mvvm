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

			Action<Animator> animationFinishedAction = _=>
			{
				animationCount--;
				if (animationCount == 0)
				{
					Set(animationInfo);
					completed?.Invoke();
				}
			};

			foreach (var element in _animation.Elements)
			{
				var viewGroup = GetViewGroup(element);
				var nativeAnimation = viewGroup.Animate();
				nativeAnimation
					.SetDuration(animationInfo.Duration)
					.SetStartDelay(animationInfo.Delay)
					.SetInterpolator(GetInterpolator(animationInfo))
					.SetListener(new AnimatorListener(null, animationFinishedAction, null, null));

				nativeAnimation.Alpha((float)animationInfo.Opacity);
				nativeAnimation.Rotation((float)animationInfo.Rotate);
				nativeAnimation.ScaleX((float)animationInfo.Scale);
				nativeAnimation.ScaleY((float)animationInfo.Scale);
				nativeAnimation.TranslationX((float)animationInfo.TranslationX * _displayDensity);
				nativeAnimation.TranslationY((float)animationInfo.TranslationY * _displayDensity);

				animations.Add(nativeAnimation);

				if (animationInfo.AnimateColor)
				{
					var fromColor = element.BackgroundColor.ToAndroid();
					var toColor = animationInfo.Color.ToAndroid();

					var colorAnimator = ObjectAnimator.OfInt(viewGroup, "backgroundColor", fromColor, toColor);
					colorAnimator.SetTarget(viewGroup);
					colorAnimator.SetEvaluator(new ArgbEvaluator());
					colorAnimator.SetDuration(animationInfo.Duration);
					colorAnimator.SetInterpolator(GetInterpolator(animationInfo));
					colorAnimator.StartDelay = animationInfo.Delay;
					colorAnimator.AddListener(new AnimatorListener(null, animationFinishedAction, null, null));

					animations.Add(colorAnimator);
				}

				if (animationInfo.AnimateRectangle)
				{
					var originalSize = new Rectangle(viewGroup.Left, viewGroup.Top, viewGroup.Width, viewGroup.Height);
					var newSize = new Rectangle(animationInfo.Rectangle.Left * _displayDensity,
												animationInfo.Rectangle.Top * _displayDensity,
												animationInfo.Rectangle.Width * _displayDensity,
												animationInfo.Rectangle.Height * _displayDensity);
					
					var resizeAnimation = ValueAnimator.OfFloat(0.0f, 1.0f);

					resizeAnimation.SetDuration(animationInfo.Duration);
					resizeAnimation.SetInterpolator(GetInterpolator(animationInfo));
					resizeAnimation.StartDelay = animationInfo.Delay;
					resizeAnimation.AddListener(new AnimatorListener(null, animationFinishedAction, null, null));
					resizeAnimation.AddUpdateListener(new UpdateListener((obj) => {

						var animValue = (float)obj.AnimatedValue;

						var newHeight = (int)(originalSize.Height + ((newSize.Height - originalSize.Height) * animValue));
						var newWidth = (int)(originalSize.Width + ((newSize.Width - originalSize.Width) * animValue));
						var newLeft = (int)(originalSize.Left + ((newSize.Left - originalSize.Left) * animValue));
						var newTop = (int)(originalSize.Top + ((newSize.Top - originalSize.Top) * animValue));

						// resize using formsviewgroup
						if (!((viewGroup is FormsViewGroup)))
							throw new ArgumentException("Needs a formsviewgroup. Wrong version?");
						   
						(viewGroup as FormsViewGroup).MeasureAndLayout(
							newWidth, newHeight, newLeft, newTop, newLeft + newWidth, newTop + newHeight);
						
					}));

					animations.Add(resizeAnimation);
				}

			}

			// Run all animations
			animationCount = animations.Count;
			foreach (var anim in animations)
			{
				if (anim is ViewPropertyAnimator)
					(anim as ViewPropertyAnimator).Start();
				else if (anim is Animator)
					(anim as Animator).Start();
			}
		}

		public void Set(XAnimationInfo animationInfo)
		{
			foreach (var element in _animation.Elements)
			{
				element.Opacity = (float)animationInfo.Opacity;
				element.Rotation = (float)animationInfo.Rotate;
				element.Scale = (float)animationInfo.Scale;
				element.TranslationX = (float)animationInfo.TranslationX;
				element.TranslationY = (float)animationInfo.TranslationY;

				if (animationInfo.AnimateRectangle)
					element.Layout(animationInfo.Rectangle);
				
				if(animationInfo.AnimateColor)
					element.BackgroundColor = animationInfo.Color;
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

		IInterpolator GetInterpolator(XAnimationInfo animationInfo)
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

	class UpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
	{
		readonly Action<ValueAnimator> _updateCallback;
		public UpdateListener(Action<ValueAnimator> updateCallback)
		{
			_updateCallback = updateCallback;
		}

		public void OnAnimationUpdate(ValueAnimator animation)
		{
			_updateCallback?.Invoke(animation);
		}
	}
	class AnimatorListener : Java.Lang.Object, Animator.IAnimatorListener
	{
		Action<Animator> _cancelAction;
		Action<Animator> _endAction;
		Action<Animator> _repeatAction;
		Action<Animator> _startAction;

		public AnimatorListener(Action<Animator>  start = null, Action<Animator> end = null, 
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
			_endAction?.Invoke(animation);
		}

		public void OnAnimationRepeat(Animator animation)
		{
			_repeatAction?.Invoke(animation);
		}

		public void OnAnimationStart(Animator animation)
		{
			_startAction?.Invoke(animation);
		}
	}
}
