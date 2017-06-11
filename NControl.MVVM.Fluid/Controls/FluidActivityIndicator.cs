using System;
using System.Linq;
using NControl.Abstractions;
using NControl.Controls;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidActivityIndicator : BaseFluidActivityIndicator
	{
		protected override View CreateActivityControl()
		{
			return new ActivityIndicator
			{
				BindingContext = this,
				HeightRequest = Config.DefaultActivityIndicatorSize,
				WidthRequest = Config.DefaultActivityIndicatorSize,
			}
			.BindTo(ActivityIndicator.ColorProperty, nameof(Color))
			.BindTo(ActivityIndicator.IsRunningProperty, nameof(IsRunning));
		}
	}

	public abstract class BaseFluidActivityIndicator : ContentView
	{
		public BaseFluidActivityIndicator()
		{
			Content = InternalCreateActivityControl();
			Opacity = 0.0;
			IsVisible = false;
		}

		View InternalCreateActivityControl()
		{
			return CreateActivityControl();
		}

		protected abstract View CreateActivityControl();
		protected virtual void RunningChanged(bool isRunning) { }

		/// <summary>
		/// The IsRunning property.
		/// </summary>
		public static BindableProperty IsRunningProperty = BindableProperty.Create(
			nameof(IsRunning), typeof(bool), typeof(BaseFluidActivityIndicator), true,
			BindingMode.OneWay, null, propertyChanged: (bindable, oldValue, newValue) =>
			{
				var ctrl = (BaseFluidActivityIndicator)bindable;
				if (oldValue == newValue)
					return;

				if ((bool)newValue == true)
				{
					ctrl.IsVisible = true;
					var animation = new XAnimationPackage(ctrl);
					animation.Add().SetOpacity(1.0);
					animation.SetDuration(50).Animate(() => {
						ctrl.RunningChanged(true);
					});
				}
				else
				{
					var animation = new XAnimationPackage(ctrl);
					animation.Add().SetOpacity(0.0);
					animation.SetDuration(50).Animate(() =>
					{
						ctrl.IsVisible = false;
						ctrl.RunningChanged(false);
					});
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
			nameof(Color), typeof(Color), typeof(BaseFluidActivityIndicator),
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
