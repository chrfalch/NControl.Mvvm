using System;
using System.Linq;
using NControl.Abstractions;
using NControl.Controls;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidActivityIndicator : ContentView
	{
		readonly RelativeLayout _layout;

		public FluidActivityIndicator()
		{
			HeightRequest = Config.DefaultActivityIndicatorSize;
			WidthRequest = Config.DefaultActivityIndicatorSize;

			Content = _layout = new RelativeLayout();
			Opacity = 0.0;
			IsVisible = false;
		
			// var random = new Random();

			// add spinners
			_layout.Children.Add(new SpinningCircleControl
			{
				BindingContext = this,
				Angle = 0.0, //random.Next(0, 360),
				DurationMilliseconds = 1500
			}
				.BindTo(SpinningCircleControl.ColorProperty, nameof(Color))
                .BindTo(SpinningCircleControl.IsRunningProperty, nameof(IsRunning)),
				() => new Rectangle(0, 0, Width, Height));

		}

		/// <summary>
		/// The IsRunning property.
		/// </summary>
		public static BindableProperty IsRunningProperty = BindableProperty.Create(
			nameof(IsRunning), typeof(bool), typeof(FluidActivityIndicator), false,
			BindingMode.OneWay, null, propertyChanged: (bindable, oldValue, newValue) =>
			{
				var ctrl = (FluidActivityIndicator)bindable;
				if (oldValue == newValue)
					return;

				if ((bool)newValue == true)
				{
					ctrl.IsVisible = true;
					var animation = new XAnimationPackage(ctrl);
					animation.Add().SetOpacity(1.0);
					animation.SetDuration(50).Animate();
				}
				else
				{
					var animation = new XAnimationPackage(ctrl);
					animation.Add().SetOpacity(0.0);
					animation.SetDuration(50).Animate(() => ctrl.IsVisible = false);
				}
			});

		/// <summary>
		/// Gets or sets the IsRunning of the CustomActivityIndicator instance.
		/// </summary>
		public bool IsRunning
		{
			get { return (bool)GetValue(IsRunningProperty); }
			set { SetValue(IsRunningProperty, value); }
		}

		/// <summary>
		/// The Color property.
		/// </summary>
		public static BindableProperty ColorProperty = BindableProperty.Create(
			nameof(Color), typeof(Color), typeof(FluidActivityIndicator),
			Config.PrimaryColor, BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the Color of the CustomActivitySpinner instance.
		/// </summary>
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
	}
}
