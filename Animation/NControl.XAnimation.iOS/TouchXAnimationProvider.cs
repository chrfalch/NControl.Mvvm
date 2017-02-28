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
			Set(animationInfo);

			if (animationInfo.OnlyTransform)
				return;

			CATransaction.Begin();
			CATransaction.CompletionBlock = () =>
			{
				completed?.Invoke();				
			};

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
					animationX.KeyPath = "transform.tx";
					animationX.From = new NSNumber(view.Layer.Transform.m41);
					animationX.To = new NSNumber(animationInfo.TranslationX);
					animations.Add(animationX);

					var animationY = new CABasicAnimation();
					animationY.KeyPath = "transform.ty";
					animationY.From = new NSNumber(view.Layer.Transform.m42);
					animationY.To = new NSNumber(animationInfo.TranslationY);
					animations.Add(animationY);
				}

				// Set up scale
				if (!animationInfo.Scale.Equals(1.0))
				{
					var animation = new CABasicAnimation();
					animation.KeyPath = "transform.scale";
					animation.From = new NSNumber(view.Layer.Transform.m11);
					animation.To = new NSNumber(animationInfo.Scale);
					animations.Add(animation);
				}

				// Set up rotation
				if (!animationInfo.Rotate.Equals(0.0))
				{
					var rotationAnimation = new CABasicAnimation();
					rotationAnimation.KeyPath = "transform.rotation";
					rotationAnimation.From = new NSNumber(view.Layer.Transform.m34);
					rotationAnimation.To = new NSNumber((animationInfo.Rotate * Math.PI) / 180.0);
					animations.Add(rotationAnimation);
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

			CATransaction.Commit();
		}

		public void Set(XAnimationInfo animationInfo)
		{
			foreach (var element in _animation.Elements)
			{
				var view = GetView(element);

				//var transform = CGAffineTransform.MakeTranslation((float)animationInfo.TranslationX, (float)animationInfo.TranslationY);
				//transform = CGAffineTransform.Scale(transform, (float)animationInfo.Scale, (float)animationInfo.Scale);
				//transform = CGAffineTransform.Rotate(transform, ConvertToRadians(animationInfo.Rotate));
				//view.Layer.AffineTransform = transform;

				var transform = CATransform3D.Identity;
				transform = transform.Translate((nfloat)animationInfo.TranslationX, (nfloat)animationInfo.TranslationY, 0);
				transform = transform.Scale((nfloat)animationInfo.Scale);
				transform = transform.Rotate((nfloat)(animationInfo.Rotate * (float)Math.PI / 180.0f), 0.0f, 0.0f, 1.0f);
				view.Layer.Transform = transform;
				view.Layer.Opacity = (float)animationInfo.Opacity;
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
