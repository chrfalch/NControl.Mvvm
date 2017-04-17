using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidRoundCornerView: ContentView
	{
		public FluidRoundCornerView()
		{

		}

		/// <summary>
		/// The BorderColor property.
		/// </summary>
		public static BindableProperty BorderColorProperty = BindableProperty.Create(
			nameof(BorderColor), typeof(Color), typeof(FluidRoundCornerView), 
			Config.BorderColor, BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the BorderColor of the FluidShadowView instance.
		/// </summary>
		public Color BorderColor
		{
			get { return (Color)GetValue(BorderColorProperty); }
			set { SetValue(BorderColorProperty, value); }
		}

		/// <summary>
		/// The BorderWidth property.
		/// </summary>
		public static BindableProperty BorderWidthProperty = BindableProperty.Create(
			nameof(BorderWidth), typeof(double), typeof(FluidRoundCornerView), 
			Config.DefaultBorderSize, BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the BorderWidth of the FluidShadowView instance.
		/// </summary>
		public double BorderWidth
		{
			get { return (double)GetValue(BorderWidthProperty); }
			set { SetValue(BorderWidthProperty, value); }
		}

		/// <summary>
		/// The BorderRadius property.
		/// </summary>
		public static BindableProperty BorderRadiusProperty = BindableProperty.Create(
			nameof(BorderRadius), typeof(int), typeof(FluidRoundCornerView), 
			FluidConfig.DefaultBorderRadius, BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the BorderRadius of the FluidShadowView instance.
		/// </summary>
		public int BorderRadius
		{
			get { return (int)GetValue(BorderRadiusProperty); }
			set { SetValue(BorderRadiusProperty, value); }
		}
	}
}
