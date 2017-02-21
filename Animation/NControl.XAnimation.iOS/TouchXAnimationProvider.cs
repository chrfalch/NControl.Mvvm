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

[assembly:Dependency(typeof(TouchXAnimationProvider))]
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
		XAnimation _animation;

		#endregion

		public void Initialize(XAnimation animation)
		{
			_animation = animation;
		}


		public void Animate(XAnimationInfo animationInfo, Action completed)
		{
			UIView.AnimateNotify(
				animationInfo.Duration / 1000.0,
				animationInfo.Delay.Equals(0) ? 0 : animationInfo.Delay / 1000.0,
				UIViewAnimationOptions.CurveEaseInOut,
				() => Set(animationInfo),
				delegate (bool finished) {
					//if (finished)
					//	completed?.Invoke();
				}
			);

			Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(animationInfo.Duration), () =>
			{
				completed?.Invoke();
				return false;
			});
		}

		public void Set(XAnimationInfo animationInfo)
		{
			foreach (var element in _animation.Elements)
			{
				var view = GetView(element);

				var transform = CGAffineTransform.MakeTranslation((float)animationInfo.TranslationX, (float)animationInfo.TranslationY);
				transform = CGAffineTransform.Scale(transform, (float)animationInfo.Scale, (float)animationInfo.Scale);
				transform = CGAffineTransform.Rotate(transform, ConvertToRadians((float)animationInfo.Rotate));

				view.Layer.AffineTransform = transform;
				view.Layer.Opacity = (float)animationInfo.Opacity;
			}
		}

		#region Private Members

		float ConvertToRadians(float angle)
		{
			return (float)((Math.PI / 180) * (double)angle);		
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
