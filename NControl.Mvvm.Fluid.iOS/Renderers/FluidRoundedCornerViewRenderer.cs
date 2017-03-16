using System;
using System.ComponentModel;
using CoreAnimation;
using NControl.Mvvm;
using NControl.Mvvm.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(FluidRoundCornerView), typeof(FluidRoundedCornerViewRenderer))]
namespace NControl.Mvvm.iOS
{
	public class FluidRoundedCornerViewRenderer: ViewRenderer<FluidRoundCornerView, UIView>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<FluidRoundCornerView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				UpdateBorder();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == nameof(Element.BorderColor) ||
				e.PropertyName == nameof(Element.BorderWidth) ||
				e.PropertyName == nameof(Element.BorderRadius))
			{
				UpdateBorder();
			}

		}

		void UpdateBorder()
		{
			NativeView.Layer.MasksToBounds = true;
			NativeView.Layer.BorderWidth = (nfloat)Element.BorderWidth;
			NativeView.Layer.BorderColor = Element.BorderColor.ToCGColor();
			NativeView.Layer.CornerRadius = Element.BorderRadius;

		}

	}
}
