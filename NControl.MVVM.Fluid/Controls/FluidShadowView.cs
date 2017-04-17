using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidShadowView : ContentView
	{
		/// <summary>
		/// The HasShadow property.
		/// </summary>
		public static BindableProperty HasShadowProperty = BindableProperty.Create(
			nameof(HasShadow), typeof(bool), typeof(FluidShadowView), true,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the HasShadow of the FluidShadowView instance.
		/// </summary>
		public bool HasShadow
		{
			get { return (bool)GetValue(HasShadowProperty); }
			set { SetValue(HasShadowProperty, value); }
		}

		/// <summary>
		/// The ShadowColor property.
		/// </summary>
		public static BindableProperty ShadowColorProperty = BindableProperty.Create(
			nameof(ShadowColor), typeof(Color), typeof(FluidShadowView),
			FluidConfig.DefaultShadowColor, BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the ShadowColor of the FluidShadowView instance.
		/// </summary>
		public Color ShadowColor
		{
			get { return (Color)GetValue(ShadowColorProperty); }
			set { SetValue(ShadowColorProperty, value); }
		}

		/// <summary>
		/// The BorderRadius property.
		/// </summary>
		public static BindableProperty BorderRadiusProperty = BindableProperty.Create(
			nameof(BorderRadius), typeof(int), typeof(FluidShadowView), 0,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the BorderRadius of the FluidShadowView instance.
		/// </summary>
		public int BorderRadius
		{
			get { return (int)GetValue(BorderRadiusProperty); }
			set { SetValue(BorderRadiusProperty, value); }
		}

		/// <summary>
		/// The ShadowOffset property.
		/// </summary>
		public static BindableProperty ShadowOffsetProperty = BindableProperty.Create(
			nameof(ShadowOffset), typeof(Size), typeof(FluidShadowView), new Size(0, 2),
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the ShadowOffset of the FluidShadowView instance.
		/// </summary>
		public Size ShadowOffset
		{
			get { return (Size)GetValue(ShadowOffsetProperty); }
			set { SetValue(ShadowOffsetProperty, value); }
		}

		/// <summary>
		/// The ShadowOpacity property.
		/// </summary>
		public static BindableProperty ShadowOpacityProperty = BindableProperty.Create(
			nameof(ShadowOpacity), typeof(double), typeof(FluidShadowView), 0.3,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the ShadowOpacity of the FluidShadowView instance.
		/// </summary>
		public double ShadowOpacity
		{
			get { return (double)GetValue(ShadowOpacityProperty); }
			set { SetValue(ShadowOpacityProperty, value); }
		}

		/// <summary>
		/// The ShadowRadius property.
		/// </summary>
		public static BindableProperty ShadowRadiusProperty = BindableProperty.Create(
			nameof(ShadowRadius), typeof(double), typeof(FluidShadowView), 6.0,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the ShadowRadius of the FluidShadowView instance.
		/// </summary>
		public double ShadowRadius
		{
			get { return (double)GetValue(ShadowRadiusProperty); }
			set { SetValue(ShadowRadiusProperty, value); }
		}
	}
}
