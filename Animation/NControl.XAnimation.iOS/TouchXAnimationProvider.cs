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

			CATransaction.Begin();
			CATransaction.CompletionBlock = () => completed?.Invoke();			

			foreach (var element in _animation.Elements)
			{
				var animations = new List<CAAnimation>();
				var view = GetView(element);

				// Set up opacity
				if (!animationInfo.Opacity.Equals(1.0))
				{
					var animation = new CABasicAnimation();
					animation.KeyPath = "opacity";
					animation.From = new NSNumber(view.Layer.Opacity);
					animation.To = new NSNumber(animationInfo.Opacity);
					animations.Add(animation);
				}

				// Set up translation
				if (!animationInfo.TranslationX.Equals(0.0) || !animationInfo.TranslationY.Equals(0.0))
				{
					var animationX = new CABasicAnimation();
					animationX.KeyPath = "transform.translation.x";
					animationX.From = new NSNumber(element.TranslationX);
					animationX.To = new NSNumber(animationInfo.TranslationX);
					animations.Add(animationX);

					var animationY = new CABasicAnimation();
					animationY.KeyPath = "transform.translation.y";
					animationY.From = new NSNumber(element.TranslationY);
					animationY.To = new NSNumber(animationInfo.TranslationY);
					animations.Add(animationY);
				}

				// Set up scale
				if (!animationInfo.Scale.Equals(1.0))
				{
					var animation = new CABasicAnimation();
					animation.KeyPath = "transform.scale";
					animation.From = new NSNumber(element.Scale);
					animation.To = new NSNumber(animationInfo.Scale);
					animations.Add(animation);
				}

				// Set up rotation
				if (!animationInfo.Rotate.Equals(0.0))
				{
					var animation = new CABasicAnimation();
					animation.KeyPath = "transform.rotation";
					animation.From = new NSNumber((element.Rotation * Math.PI) / 180.0);
					animation.To = new NSNumber((animationInfo.Rotate * Math.PI) / 180.0);
					animations.Add(animation);
				}

				if (animations.Any())
				{
					var group = new CAAnimationGroup();
					group.Duration = animationInfo.Duration / 1000.0;
					group.BeginTime = CAAnimation.CurrentMediaTime() + (animationInfo.Delay / 1000.0);
					group.Animations = animations.ToArray();
					view.Layer.AddAnimation(group, "animinfo-anims");
					animations.Clear();
				}
			}

			Set(animationInfo);

			CATransaction.Commit();
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
