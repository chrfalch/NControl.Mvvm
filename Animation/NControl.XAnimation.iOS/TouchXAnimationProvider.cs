using System;
using UIKit;
using CoreGraphics;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using NControl.XAnimation;
using CoreAnimation;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Foundation;
using NControl.XAnimation.iOS;

[assembly: Dependency(typeof(TouchXAnimationProvider))]
namespace NControl.XAnimation.iOS
{
	/// <summary>
	/// Implements the iOS XAnimation provider
	/// </summary>
	public class TouchXAnimationProvider : IXAnimationProvider
	{
		#region Private Members

		/// <summary>
		/// Parent animation
		/// </summary>
		XTransformationContainer _container;

		#endregion

		public void Initialize(XTransformationContainer container)
		{
			_container = container;
		}

		public bool GetHasViewsToAnimate()
		{
			var numberofLiveViews = 0;
			for (var i = 0; i < _container.ElementCount; i++)
			{
				var element = _container.GetElement(i);
				var view = GetView(element);
				if (view != null && view.Superview != null)
					numberofLiveViews++;
			}

			return numberofLiveViews > 0;
		}

		public void Animate(XTransform transform, Action completed, long duration)
		{
			if (transform.OnlyTransform)
			{
				Set(transform);
				return;
			}

			var viewAnimations = new Dictionary<UIView, IEnumerable<CAAnimation>>();

			for (var i = 0; i < _container.ElementCount; i++)
			{
				var element = _container.GetElement(i);
				var view = GetView(element);
				var animations = GetAnimationsForElement(element, view, transform);

				if (animations.Any())
				{
					// Update platform values for presentation model
					viewAnimations.Add(view, animations);

					if (transform.AnimateRectangle)
					{
						// Get animation info for target after layout has been updated
						var childHierarchyInfo = GetChildHierarchyInfo(element, transform);

						// Update platform values for presentation model
						SetPlatformElementFromAnimationInfo(element, transform);

						// Add animations for all
						foreach (var childElement in childHierarchyInfo.Keys)
						{
							// Get child details
							var childView = GetView(childElement);
							var childAnimations = GetRectangleAnimations(
								childElement, childView, childHierarchyInfo[childElement]);

							viewAnimations.Add(childView, childAnimations);
						}
					}
				}
			}

			if (!viewAnimations.Values.Any())
			{
				completed?.Invoke();
				return;
			}

			// Start animation with transaction
			CATransaction.Begin();
			CATransaction.DisableActions = true;
			CATransaction.AnimationTimingFunction = GetTimingFunction(transform.Easing);
			CATransaction.AnimationDuration = GetTime(duration);
			CATransaction.CompletionBlock = completed;

			// Add animations
			foreach (var view in viewAnimations.Keys)
				for (var i = 0; i < viewAnimations[view].Count(); i++)
					view.Layer.AddAnimation(viewAnimations[view].ElementAt(i), "animinfo-anims-" + i.ToString());

			SetInternal(transform, false);

			// Commit transaction
			CATransaction.Commit();
		}

		public void Set(XTransform animationInfo)
		{
			SetInternal(animationInfo);
		}

		public void Set(VisualElement element, XTransform animationInfo)
		{
            SetElementFromAnimationInfo(element, animationInfo);
		}

		#region Private Members

		void SetInternal(XTransform transform, bool waitIfSlowSet = true)
		{
			for (var i = 0; i < _container.ElementCount; i++)
			{
				var element = _container.GetElement(i);
				SetElementFromAnimationInfo(element, transform);
			}

			if (waitIfSlowSet && XElementContainer.ShowSetTransforms)
				NSThread.SleepFor(0.5);
		}

		Dictionary<VisualElement, XTransform> GetChildHierarchyInfoInt(VisualElement element, XTransform animationInfo)
		{
			var retVal = new Dictionary<VisualElement, XTransform>();

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

		Dictionary<VisualElement, XTransform> GetChildHierarchyInfo(VisualElement element, XTransform animationInfo)
		{
			var originalState = GetAnimationInfoFromElement(element, animationInfo);
		
			Set(animationInfo);
			var toValues = GetChildHierarchyInfoInt(element, animationInfo);
			Set(originalState);

			return toValues;
		}

		CAMediaTimingFunction GetTimingFunction(EasingFunctionBezier easingFunction)
		{
			return CAMediaTimingFunction.FromControlPoints(
						(float)easingFunction.Start.X, (float)easingFunction.Start.Y,
						(float)easingFunction.End.X, (float)easingFunction.End.Y);			           
		}

		float GetTime(long time)
		{
			return (float)(time * (XElementContainer.SlowAnimations ? 2.5 : 1) / 1000.0);
		}

		List<CAAnimation> GetAnimationsForElement(VisualElement element, UIView view, XTransform animationInfo)
		{
			var animations = new List<CAAnimation>();

			// Set up opacity
			if (!element.Opacity.Equals(animationInfo.Opacity))
			{
				var opacityAnimation = new CABasicAnimation();
				opacityAnimation.KeyPath = "opacity";
				opacityAnimation.From = new NSNumber(element.Opacity);
				opacityAnimation.To = new NSNumber(animationInfo.Opacity);
				animations.Add(opacityAnimation);
			}

			// Set up translation
			if (!element.TranslationX.Equals(animationInfo.TranslationX))
			{
				var translationAnimationX = new CABasicAnimation();
				translationAnimationX.KeyPath = "transform.translation.x";
				translationAnimationX.From = new NSNumber(element.TranslationX);
				translationAnimationX.To = new NSNumber(animationInfo.TranslationX);
				animations.Add(translationAnimationX);
			}

			if (!element.TranslationY.Equals(animationInfo.TranslationY))
			{
				var translationAnimationY = new CABasicAnimation();
				translationAnimationY.KeyPath = "transform.translation.y";
				translationAnimationY.From = new NSNumber(element.TranslationY);
				translationAnimationY.To = new NSNumber(animationInfo.TranslationY);
				animations.Add(translationAnimationY);
			}

			// Set up scale
			if (!element.Scale.Equals(animationInfo.Scale))
			{
				var scaleAnimation = new CABasicAnimation();
				scaleAnimation.KeyPath = "transform.scale";
				scaleAnimation.From = new NSNumber(element.Scale);
				scaleAnimation.To = new NSNumber(animationInfo.Scale);
				animations.Add(scaleAnimation);
			}

			// Set up rotation
			if (!element.Rotation.Equals(animationInfo.Rotation))
			{
				var rotateAnimation = new CABasicAnimation();
				rotateAnimation.KeyPath = "transform.rotation";
				rotateAnimation.From = new NSNumber((element.Rotation * Math.PI) / 180.0);
				rotateAnimation.To = new NSNumber((animationInfo.Rotation * Math.PI) / 180.0);
				animations.Add(rotateAnimation);
			}

			// Color
			if (animationInfo.AnimateColor)
			{
				var fromColor = element.BackgroundColor.ToUIColor();
				var toColor = animationInfo.Color.ToUIColor();

				var colorAnimation = new CABasicAnimation();
				colorAnimation.KeyPath = "backgroundColor";
				colorAnimation.From = fromColor;
				colorAnimation.To = toColor;
				animations.Add(colorAnimation);
			}

			// Frame
			if (animationInfo.AnimateRectangle)
			{
				animations.AddRange(GetRectangleAnimations(element, view, animationInfo));
			}

			return animations;
		}

		IEnumerable<CAAnimation> GetRectangleAnimations(VisualElement element, UIView view, XTransform animationInfo)
		{
			var animations = new List<CAAnimation>();

			var fromBounds = view.Layer.Bounds;
			var toBounds = view.Layer.Bounds;
			toBounds.Size = new CGSize(animationInfo.Rectangle.Width, animationInfo.Rectangle.Height);

			var boundsAnimation = new CABasicAnimation();
			boundsAnimation.KeyPath = "size";
			boundsAnimation.From = NSValue.FromCGSize(fromBounds.Size);
			boundsAnimation.To = NSValue.FromCGSize(toBounds.Size);

			animations.Add(boundsAnimation);

			if (animationInfo.Rectangle.X != double.MinValue &&
			   animationInfo.Rectangle.Y != double.MaxValue)
			{
				var toPos = new CGPoint(
					(nfloat)animationInfo.Rectangle.X + (view.Layer.AnchorPoint.X * toBounds.Width),
					(nfloat)animationInfo.Rectangle.Y + (view.Layer.AnchorPoint.Y * toBounds.Height));

				if (view.Layer.Position.X.Equals(toPos.X) && view.Layer.Position.Y.Equals(toPos.Y))
					return animations;

				var fromPos = view.Layer.ValueForKey(new NSString("position"));

				var posAnimation = new CABasicAnimation();
				posAnimation.KeyPath = "position";
				posAnimation.From = fromPos;
				posAnimation.To = NSValue.FromCGPoint(toPos);

				animations.Add(posAnimation);
			}

			return animations;
		}

		void SetPlatformElementFromAnimationInfo(VisualElement element, XTransform animationInfo)
		{
			var view = GetView(element);
			SetPlatformElementFromAnimationInfo(view, animationInfo);
		}

		void SetPlatformElementFromAnimationInfo(UIView view, XTransform animationInfo)
		{
			view.Layer.Transform = CATransform3D.Identity; 
			view.Layer.SetValueForKey(new NSNumber((float)animationInfo.Scale), new NSString("transform.scale"));
			view.Layer.SetValueForKey(new NSNumber((float)animationInfo.TranslationX), new NSString("transform.translation.x"));
			view.Layer.SetValueForKey(new NSNumber((float)animationInfo.TranslationY), new NSString("transform.translation.y"));
			view.Layer.SetValueForKey(new NSNumber((animationInfo.Rotation * Math.PI) / 180.0), new NSString("transform.rotate"));

			view.Layer.Opacity = (float)animationInfo.Opacity;

			if (animationInfo.AnimateRectangle)
			{
				var toBounds = view.Layer.Bounds;
				toBounds.Size = new CGSize(animationInfo.Rectangle.Width, animationInfo.Rectangle.Height);

				var toPos = new CGPoint(
					(nfloat)animationInfo.Rectangle.X + (view.Layer.AnchorPoint.X * toBounds.Width),
					(nfloat)animationInfo.Rectangle.Y + (view.Layer.AnchorPoint.Y * toBounds.Height));

				view.Layer.Position = toPos;

				if(animationInfo.Rectangle.X != double.MinValue &&
				   animationInfo.Rectangle.Y != double.MinValue)
					view.Layer.Bounds = toBounds;
			}

			if (animationInfo.AnimateColor)
			{
				view.Layer.BackgroundColor = animationInfo.Color.ToCGColor();
			}
		}

		XTransform GetAnimationInfoFromElement(VisualElement element, XTransform animationInfo)
		{
            return XTransform.FromAnimationInfoAndElement(element, animationInfo);
		}

		void SetElementFromAnimationInfo(VisualElement element, XTransform animationInfo)
		{
			element.Rotation = animationInfo.Rotation;
			element.TranslationX = animationInfo.TranslationX;
			element.TranslationY = animationInfo.TranslationY;
			element.Scale = animationInfo.Scale;
			element.Opacity = animationInfo.Opacity;

			if (animationInfo.AnimateRectangle)
			{
				if (animationInfo.Rectangle.X != double.MinValue &&
				   animationInfo.Rectangle.Y != double.MinValue)
					element.Layout(animationInfo.Rectangle);
				else
					element.Layout(new Xamarin.Forms.Rectangle(
						element.X, element.Y, animationInfo.Rectangle.Width, animationInfo.Rectangle.Height));
			}

			if (animationInfo.AnimateColor)
				element.BackgroundColor = animationInfo.Color;
		}

		float ConvertToRadians(double angle)
		{
			return (float)(angle * Math.PI / 180.0);
		}

		UIView GetView(VisualElement element)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
				return null;

			return renderer.NativeView;
		}

		#endregion
	}

}
