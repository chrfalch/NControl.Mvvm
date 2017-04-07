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
		XAnimationPackage _animation;

		#endregion

		public void Initialize(XAnimationPackage animation)
		{
			_animation = animation;
		}


		public void Animate(XAnimationInfo animationInfo, Action completed)
		{
			if (animationInfo.OnlyTransform)
			{
				Set(animationInfo);
				return;
			}

			var viewAnimations = new Dictionary<UIView, IEnumerable<CAAnimation>>();

			foreach (var element in _animation.Elements)
			{
				var view = GetView(element);
				var animations = GetAnimationsForElement(element, view, animationInfo);

				if (animations.Any())
				{
					// Update platform values for presentation model
					viewAnimations.Add(view, animations);

					if (animationInfo.AnimateRectangle)
					{
						// Get animation info for target after layout has been updated
						var childHierarchyInfo = GetChildHierarchyInfo(element, animationInfo);

						// Update platform values for presentation model
						SetPlatformElementFromAnimationInfo(element, animationInfo);

						// Add animations for all
						foreach (var childElement in childHierarchyInfo.Keys)
						{
							// Get child details
							var childView = GetView(childElement);
							var childAnimations = GetRectangleAnimations(childElement, childView, childHierarchyInfo[childElement]);

							viewAnimations.Add(childView, childAnimations);
						}
					}
				}
			}

			// Start animation with transaction
			CATransaction.Begin();
			CATransaction.DisableActions = true;
			CATransaction.AnimationTimingFunction = GetTimingFunctionFromAnimationInfo(animationInfo);
			CATransaction.AnimationDuration = GetTime(animationInfo.Duration);
			CATransaction.CompletionBlock = completed;

			// Add animations
			foreach (var view in viewAnimations.Keys)
				for (var i = 0; i < viewAnimations[view].Count(); i++)
					view.Layer.AddAnimation(viewAnimations[view].ElementAt(i), "animinfo-anims-" + i.ToString());

			// Set end values
			Set(animationInfo);

			// Commit transaction
			CATransaction.Commit();
		}

		public void Set(XAnimationInfo animationInfo)
		{
			foreach (var element in _animation.Elements)
				SetElementFromAnimationInfo(element, animationInfo);
		}

		#region Private Members

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

		CAAnimationGroup GetAnimationGroup(IEnumerable<CAAnimation> animations, XAnimationInfo animationInfo)
		{
			// Create group of animations
			var group = new CAAnimationGroup();
			group.Duration = GetTime(animationInfo.Duration);
			group.BeginTime = CAAnimation.CurrentMediaTime() + (GetTime(animationInfo.Delay));
			group.Animations = animations.ToArray();
			group.TimingFunction = GetTimingFunctionFromAnimationInfo(animationInfo);
			return group;
		}

		CAMediaTimingFunction GetTimingFunctionFromAnimationInfo(XAnimationInfo animationInfo)
		{
			switch (animationInfo.Easing)
			{
				case EasingFunction.EaseIn:
					return CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseIn);					
				case EasingFunction.EaseOut:
					return CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);					
				case EasingFunction.EaseInOut:
					return CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);					
				case EasingFunction.Custom:
					return CAMediaTimingFunction.FromControlPoints(
						(float)animationInfo.EasingBezier.Start.X, (float)animationInfo.EasingBezier.Start.Y,
						(float)animationInfo.EasingBezier.End.X, (float)animationInfo.EasingBezier.End.Y);
				default:
					return CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);					
			}
		}

		float GetTime(long time)
		{
			return (float)(time * (XAnimationPackage.SlowAnimations ? 5 : 1) / 1000.0);
		}

		List<CAAnimation> GetAnimationsForElement(VisualElement element, UIView view, XAnimationInfo animationInfo)
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
			if (!element.Rotation.Equals(animationInfo.Rotate))
			{
				var rotateAnimation = new CABasicAnimation();
				rotateAnimation.KeyPath = "transform.rotation";
				rotateAnimation.From = new NSNumber((element.Rotation * Math.PI) / 180.0);
				rotateAnimation.To = new NSNumber((animationInfo.Rotate * Math.PI) / 180.0);
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

		IEnumerable<CAAnimation> GetRectangleAnimations(VisualElement element, UIView view, XAnimationInfo animationInfo)
		{
			var animations = new List<CAAnimation>();

			var fromBounds = view.Layer.Bounds;
			var toBounds = view.Layer.Bounds;
			toBounds.Size = new CGSize(animationInfo.Rectangle.Width, animationInfo.Rectangle.Height);

			var boundsAnimation = new CABasicAnimation();
			boundsAnimation.KeyPath = "bounds";
			boundsAnimation.From = NSValue.FromCGRect(fromBounds);
			boundsAnimation.To = NSValue.FromCGRect(toBounds);

			animations.Add(boundsAnimation);

			var fromPos = view.Layer.ValueForKey(new NSString("position"));
			var toPos = new CGPoint(
				(nfloat)animationInfo.Rectangle.X + (view.Layer.AnchorPoint.X * toBounds.Width),
				(nfloat)animationInfo.Rectangle.Y + (view.Layer.AnchorPoint.Y * toBounds.Height));

			var posAnimation = new CABasicAnimation();
			posAnimation.KeyPath = "position";
			posAnimation.From = fromPos;
			posAnimation.To = NSValue.FromCGPoint(toPos);

			animations.Add(posAnimation);

			return animations;
		}

		void SetPlatformElementFromAnimationInfo(VisualElement element, XAnimationInfo animationInfo)
		{
			var view = GetView(element);
			SetPlatformElementFromAnimationInfo(view, animationInfo);
		}

		void SetPlatformElementFromAnimationInfo(UIView view, XAnimationInfo animationInfo)
		{
			view.Layer.Transform = CATransform3D.Identity; 
			view.Layer.SetValueForKey(new NSNumber((float)animationInfo.Scale), new NSString("transform.scale"));
			view.Layer.SetValueForKey(new NSNumber((float)animationInfo.TranslationX), new NSString("transform.translation.x"));
			view.Layer.SetValueForKey(new NSNumber((float)animationInfo.TranslationY), new NSString("transform.translation.y"));
			view.Layer.SetValueForKey(new NSNumber((animationInfo.Rotate * Math.PI) / 180.0), new NSString("transform.rotate"));

			view.Layer.Opacity = (float)animationInfo.Opacity;

			if (animationInfo.AnimateRectangle)
			{
				var toBounds = view.Layer.Bounds;
				toBounds.Size = new CGSize(animationInfo.Rectangle.Width, animationInfo.Rectangle.Height);

				var toPos = new CGPoint(
					(nfloat)animationInfo.Rectangle.X + (view.Layer.AnchorPoint.X * toBounds.Width),
					(nfloat)animationInfo.Rectangle.Y + (view.Layer.AnchorPoint.Y * toBounds.Height));

				view.Layer.Position = toPos;
				view.Layer.Bounds = toBounds;
			}

			if (animationInfo.AnimateColor)
			{
				view.Layer.BackgroundColor = animationInfo.Color.ToCGColor();
			}
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

		float ConvertToRadians(double angle)
		{
			return (float)(angle * Math.PI / 180.0);
		}

		UIView GetView(VisualElement element)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
			{
				renderer = Platform.CreateRenderer(element);
				Platform.SetRenderer(element, renderer);
			}

			return renderer.NativeView;
		}

		#endregion
	}

}
