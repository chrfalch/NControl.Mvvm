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
			nameof(HasShadow), typeof(bool), typeof(FluidShadowView), false,
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
			MvvmApp.Current.Colors.Get(FluidConfig.DefaultShadowColor), BindingMode.OneWay);

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
	}
}
