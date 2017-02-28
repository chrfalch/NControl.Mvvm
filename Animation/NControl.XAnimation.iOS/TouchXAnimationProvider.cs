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
				var opacityAnimation = new CABasicAnimation();
				opacityAnimation.KeyPath = "opacity";
				opacityAnimation.From = new NSNumber(element.Opacity);
				opacityAnimation.To = new NSNumber(animationInfo.Opacity);
				animations.Add(opacityAnimation);
			
				// Set up translation
				var translationAnimationX = new CABasicAnimation();
				translationAnimationX.KeyPath = "transform.translation.x";
				translationAnimationX.From = new NSNumber(element.TranslationX);
				translationAnimationX.To = new NSNumber(animationInfo.TranslationX);
				animations.Add(translationAnimationX);

				var translationAnimationY = new CABasicAnimation();
				translationAnimationY.KeyPath = "transform.translation.y";
				translationAnimationY.From = new NSNumber(element.TranslationY);
				translationAnimationY.To = new NSNumber(animationInfo.TranslationY);
				animations.Add(translationAnimationY);			

				// Set up scale
				var scaleAnimation = new CABasicAnimation();
				scaleAnimation.KeyPath = "transform.scale";
				scaleAnimation.From = new NSNumber(element.Scale);
				scaleAnimation.To = new NSNumber(animationInfo.Scale);
				animations.Add(scaleAnimation);				

				// Set up rotation
				var rotateAnimation = new CABasicAnimation();
				rotateAnimation.KeyPath = "transform.rotation";
				rotateAnimation.From = new NSNumber((element.Rotation * Math.PI) / 180.0);
				rotateAnimation.To = new NSNumber((animationInfo.Rotate * Math.PI) / 180.0);
				animations.Add(rotateAnimation);
			

				var group = new CAAnimationGroup();
				group.Duration = animationInfo.Duration / 1000.0;
				group.BeginTime = CAAnimation.CurrentMediaTime() + (animationInfo.Delay / 1000.0);
				group.Animations = animations.ToArray();
				view.Layer.AddAnimation(group, "animinfo-anims");
				animations.Clear();			
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
