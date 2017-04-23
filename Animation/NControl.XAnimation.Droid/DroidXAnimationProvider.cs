﻿using System;
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

		public bool GetHasViewsToAnimate(XAnimationInfo animationinfo)
		{
			var numberofLiveViews = 0;
			for (var i = 0; i < _animation.ElementCount; i++)
			{
				var element = _animation.GetElement(i);
				var viewGroup = GetViewGroup(element);
				if (viewGroup.Parent != null)
					numberofLiveViews++;
			}

			return numberofLiveViews > 0;
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

			for (var i = 0; i<_animation.ElementCount; i++)
			{
				var element = _animation.GetElement(i);
				var viewGroup = GetViewGroup(element);
				var nativeAnimation = viewGroup.Animate();
				nativeAnimation
					.SetDuration(GetTime(animationInfo.Duration))
					.SetStartDelay(GetTime(animationInfo.Delay))
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
					colorAnimator.SetDuration(GetTime(animationInfo.Duration));
					colorAnimator.SetInterpolator(GetInterpolator(animationInfo));
					colorAnimator.StartDelay = GetTime(animationInfo.Delay);
					colorAnimator.AddListener(new AnimatorListener(null, animationFinishedAction, null, null));

					animations.Add(colorAnimator);
				}

				if (animationInfo.AnimateRectangle)
				{
					animations.Add(GetRectangleAnimation(element, viewGroup, animationInfo, animationFinishedAction));
				}

				// Get children
				if (animationInfo.AnimateRectangle)
				{
					// Get animation info for target after layout has been updated
					var childHierarchyInfo = GetChildHierarchyInfo(element, animationInfo);

					foreach (var childElement in childHierarchyInfo.Keys)
					{
						// Get child details
						var childView = GetViewGroup(childElement);
						var childAnimation = GetRectangleAnimation(
							childElement, childView, childHierarchyInfo[childElement], animationFinishedAction);

						// Add animations
						animations.Add(childAnimation);
					}
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

		public void Set(VisualElement element, XAnimationInfo animationInfo)
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

		public void Set(XAnimationInfo animationInfo)
		{
			for (var i = 0; i < _animation.ElementCount; i++)
			{
				var element = _animation.GetElement(i);
				Set(element, animationInfo);
			}
		}

		#region Private Members

		Animator GetRectangleAnimation(VisualElement element, ViewGroup viewGroup, XAnimationInfo animationInfo, Action<Animator> animationFinishedAction)
		{
			var originalSize = new Rectangle(viewGroup.Left, viewGroup.Top, viewGroup.Width, viewGroup.Height);
			var newSize = new Rectangle(animationInfo.Rectangle.Left * _displayDensity,
										animationInfo.Rectangle.Top * _displayDensity,
										animationInfo.Rectangle.Width * _displayDensity,
										animationInfo.Rectangle.Height * _displayDensity);

			var resizeAnimation = ValueAnimator.OfFloat(0.0f, 1.0f);

			resizeAnimation.SetDuration(GetTime(animationInfo.Duration));
			resizeAnimation.SetInterpolator(GetInterpolator(animationInfo));
			resizeAnimation.StartDelay = GetTime(animationInfo.Delay);
			resizeAnimation.AddListener(new AnimatorListener(null, animationFinishedAction, null, null));
			resizeAnimation.AddUpdateListener(new UpdateListener((obj) =>
			{

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

			return resizeAnimation;
		}

		Dictionary<VisualElement, XAnimationInfo> GetChildHierarchyInfoInt(VisualElement element, XAnimationInfo animationInfo)
		{
			var retVal = new Dictionary<VisualElement, XAnimationInfo>();

			if (element is ContentView)
			{
				var child = (element as ContentView).Content;
				var transformsTo = GetAnimationInfoFromElement(child, animationInfo);
				retVal.Add(child, transformsTo);

				var childValues = GetChildHierarchyInfoInt(child as VisualElement, animationInfo);
				foreach (var key in childValues.Keys)
					retVal.Add(key, childValues[key]);
			}
			else if (element is ILayoutController)
			{
				foreach (var child in (element as ILayoutController).Children)
				{
					if (child is VisualElement)
					{
						var transformsTo = GetAnimationInfoFromElement(child as VisualElement, animationInfo);
						retVal.Add(child as VisualElement, transformsTo);

						var childValues = GetChildHierarchyInfoInt(child as VisualElement, animationInfo);
						foreach (var key in childValues.Keys)
							retVal.Add(key, childValues[key]);
					}
				}
			}

			return retVal;
		}

		Dictionary<VisualElement, XAnimationInfo> GetChildHierarchyInfo(VisualElement element, XAnimationInfo animationInfo)
		{
			var originalState = GetAnimationInfoFromElement(element, animationInfo);

			Set(animationInfo);
			var toValues = GetChildHierarchyInfoInt(element, animationInfo);
			Set(originalState);

			return toValues;
		}

		XAnimationInfo GetAnimationInfoFromElement(VisualElement element, XAnimationInfo animationInfo)
		{
			return new XAnimationInfo
			{
				Duration = animationInfo.Duration,
				Delay = animationInfo.Delay,
				Easing = animationInfo.Easing,
				EasingBezier = animationInfo.EasingBezier,
				OnlyTransform = animationInfo.OnlyTransform,
				Rotate = element.Rotation,
				TranslationX = element.TranslationX,
				TranslationY = element.TranslationY,
				Scale = element.Scale,
				Opacity = element.Opacity,
				AnimateColor = animationInfo.AnimateColor,
				Color = element.BackgroundColor,
				AnimateRectangle = animationInfo.AnimateRectangle,
				Rectangle = new Xamarin.Forms.Rectangle(element.X, element.Y, element.Width, element.Height),
			};
		}

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

		long GetTime(long time)
		{
			return time * (XAnimationPackage.SlowAnimations ? 5 : 1);
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
