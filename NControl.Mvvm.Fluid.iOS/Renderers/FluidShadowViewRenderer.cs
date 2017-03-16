using System;
using System.ComponentModel;
using CoreAnimation;
using NControl.Mvvm;
using NControl.Mvvm.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FluidShadowView), typeof(FluidShadowViewRenderer))]
namespace NControl.Mvvm.iOS
{
	public class FluidShadowViewRenderer : ViewRenderer<FluidShadowView, UIView>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<FluidShadowView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{				
				UpdateShadow();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == nameof(Element.ShadowColor) ||
				e.PropertyName == nameof(Element.HasShadow) ||
			    e.PropertyName == nameof(Element.BorderRadius))
			{
				UpdateShadow();
			}
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			UpdateShadow();
		}

		void UpdateShadow()
		{
			// drop shadow
			if (Element.HasShadow)
			{
				NativeView.Layer.ShadowColor = Element.ShadowColor.ToCGColor();
				NativeView.Layer.ShadowOpacity = 0.3f;
				NativeView.Layer.ShadowRadius = 6.0f;
				NativeView.Layer.ShadowPath = UIBezierPath.FromRoundedRect(NativeView.Bounds, (nfloat)Element.BorderRadius).CGPath;
				NativeView.Layer.ShadowOffset = new CoreGraphics.CGSize(0.5, 2.0);
				NativeView.Layer.ShouldRasterize = true;
				NativeView.Layer.RasterizationScale = UIScreen.MainScreen.Scale;
			}
			else
			{
				NativeView.Layer.ShadowColor = UIColor.Clear.CGColor;
				NativeView.Layer.ShadowOpacity = 0.0f;
				NativeView.Layer.ShadowRadius = 0.0f;
				NativeView.Layer.ShadowOffset = CoreGraphics.CGSize.Empty;
			}
		}
	}
}
