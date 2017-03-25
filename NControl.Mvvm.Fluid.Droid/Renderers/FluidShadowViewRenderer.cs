using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using NControl.Mvvm;
using NControl.Mvvm.Droid;
using System.ComponentModel;
using Android.Graphics;
using Android.Views;

[assembly: ExportRenderer(typeof(FluidShadowView), typeof(FluidShadowViewRenderer))]
namespace NControl.Mvvm.Droid
{
	public class FluidShadowViewRenderer: ViewRenderer<FluidShadowView, FormsViewGroup>
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
				e.PropertyName == nameof(Element.ShadowRadius) ||
				e.PropertyName == nameof(Element.ShadowOffset) ||
				e.PropertyName == nameof(Element.ShadowOpacity) ||
				e.PropertyName == nameof(Element.BorderRadius))
			{
				UpdateShadow();
			}
		}

		protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged(w, h, oldw, oldh);
			UpdateShadow();
		}

		void UpdateShadow()
		{
			if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Lollipop)
				return;
			
			// drop shadow
			if (Element.HasShadow)
			{
				Elevation = (float)Element.ShadowRadius * MvvmApp.Current.Environment.DisplayDensity;
				OutlineProvider = new RoundCornerOutlineProvider(Element);
			}
			else
			{
				Elevation = 0;
				//SetBackgroundColor(Xamarin.Forms.Color.Transparent.ToAndroid());
			}
		}
	}

	class RoundCornerOutlineProvider : ViewOutlineProvider
	{
		FluidShadowView _sourceElement;

		public RoundCornerOutlineProvider(FluidShadowView sourceElement)
		{
			_sourceElement = sourceElement;
		}

		public override void GetOutline(Android.Views.View view, Outline outline)
		{
			outline.SetRoundRect(
				new Rect(0, 0, view.Width, view.Height), 
				Forms.Context.ToPixels(_sourceElement.BorderRadius));
		}
	}
}
