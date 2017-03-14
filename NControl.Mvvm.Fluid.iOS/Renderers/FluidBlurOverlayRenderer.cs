using System;
using Xamarin.Forms;
using NControl.Controls.iOS;
using NControl.Mvvm;
using NControl.Mvvm.iOS;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly:ExportRenderer(typeof(FluidBlurOverlay), typeof(FluidBlurOverlayRenderer))]
namespace NControl.Mvvm.iOS
{
	public class FluidBlurOverlayRenderer: ViewRenderer<FluidBlurOverlay, UIView>
	{
		UIVisualEffectView _effect;

		protected override void OnElementChanged(ElementChangedEventArgs<FluidBlurOverlay> e)
		{
			base.OnElementChanged(e);
	
			if(e.OldElement != null)
				e.OldElement.OnScreenshotRequest -= Handle_OnScreenshotRequest;

			if (e.NewElement != null)
			{
				e.NewElement.OnScreenshotRequest += Handle_OnScreenshotRequest;

				if (_effect == null)
				{
					_effect = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.ExtraLight));
					SetNativeControl(_effect);
					SetNeedsLayout();
				}
			}
		}

		protected override void UpdateNativeWidget()
		{
			base.UpdateNativeWidget();
			NativeView.BackgroundColor = UIColor.Clear;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			if(_effect != null)
				_effect.Frame = NativeView.Bounds;
		}

		void Handle_OnScreenshotRequest(object sender, EventArgs e)
		{
			// var snapshot = NativeView.SnapshotView(true);
		}
	}
}
