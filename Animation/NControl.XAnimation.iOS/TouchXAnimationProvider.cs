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

			foreach (var element in _animation.Elements)
			{
				var animations = new List<CAAnimation>();
				var view = GetView(element);

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
					var fromColor = view.BackgroundColor;
					var toColor = animationInfo.Color.ToUIColor();

					var colorAnimation = new CABasicAnimation();
					colorAnimation.KeyPath = "backgroundColor";
					colorAnimation.From = fromColor;
					colorAnimation.To = toColor;
					animations.Add(colorAnimation);
				}

				if(animations.Any())
				{
					// Create group of animations
					var group = new CAAnimationGroup();
					group.Duration = animationInfo.Duration / 1000.0;
					group.BeginTime = CAAnimation.CurrentMediaTime() + (animationInfo.Delay / 1000.0);
					group.Animations = animations.ToArray();

					switch (animationInfo.Easing)
					{
						case EasingFunction.EaseIn:
							group.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseIn);
							break;
						case EasingFunction.EaseOut:
							group.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
							break;
						case EasingFunction.EaseInOut:
							group.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
							break;
						case EasingFunction.Custom:
							group.TimingFunction = CAMediaTimingFunction.FromControlPoints(
								(float)animationInfo.EasingBezier.Start.X, (float)animationInfo.EasingBezier.Start.Y,
								(float)animationInfo.EasingBezier.End.X, (float)animationInfo.EasingBezier.End.Y);

							break;
						default:
							group.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
							break;
					}

					CATransaction.Begin();
					CATransaction.CompletionBlock = () => 
						completed?.Invoke();

					// Add animation
					view.Layer.AddAnimation(group, "animinfo-anims");
					animations.Clear();

					// Commit transaction
					CATransaction.Commit();

					// Set final values (animation above only changes presentation values)
					Set(animationInfo);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("No animations to run");
					completed?.Invoke();
				}
			}
		}

		public void Set(XAnimationInfo animationInfo)
		{
			foreach (var element in _animation.Elements)
			{
				element.Rotation = animationInfo.Rotate;
				element.TranslationX = animationInfo.TranslationX;
				element.TranslationY = animationInfo.TranslationY;
				element.Scale = animationInfo.Scale;
				element.Opacity = (float)animationInfo.Opacity;

				if(animationInfo.AnimateColor)
					element.BackgroundColor = animationInfo.Color;				
			}
		}

		#region Private Members

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
