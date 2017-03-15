using System;
using Xamarin.Forms;
using NControl.Controls.iOS;
using NControl.Mvvm;
using NControl.Mvvm.iOS;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;

[assembly:ExportRenderer(typeof(FluidBlurOverlay), typeof(FluidBlurOverlayRenderer))]
namespace NControl.Mvvm.iOS
{
	public class FluidBlurOverlayRenderer: ViewRenderer<FluidBlurOverlay, UIView>
	{
		CGRect _lastFrame = CGRect.Empty;
		UIImageView _currentImageView = null;

		protected override void OnElementChanged(ElementChangedEventArgs<FluidBlurOverlay> e)
		{
			base.OnElementChanged(e);	
		}

		protected override void UpdateNativeWidget()
		{
			base.UpdateNativeWidget();
			NativeView.BackgroundColor = UIColor.Clear;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			if (_lastFrame != Frame)
			{				
				var snapshot = UIScreen.MainScreen.Capture();
				if (snapshot != null)
				{
					if (_currentImageView != null)
						_currentImageView.RemoveFromSuperview();

					_currentImageView = new UIImageView(snapshot);
					_currentImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
					_currentImageView.Frame = Bounds;

					var effectView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Light));
					effectView.Frame = _currentImageView.Bounds;
					effectView.Alpha = 0.9f;
					_currentImageView.AddSubview(effectView);

					AddSubview(_currentImageView);
				}

				_lastFrame = Frame;
			}
		}
	}
}
