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
			HeightRequest = MvvmApp.Current.Sizes.Get(Config.DefaultActivityIndicatorSize);
			WidthRequest = MvvmApp.Current.Sizes.Get(Config.DefaultActivityIndicatorSize);

			Content = _layout = new RelativeLayout();

			// add spinners
			_layout.Children.Add(new SpinningCircleControl
			{
				BindingContext = this,
				Angle = 0,
				DurationMilliseconds = 2500
			}
				.BindTo(SpinningCircleControl.ColorProperty, nameof(Color))
				.BindTo(SpinningCircleControl.IsRunningProperty, nameof(IsRunning)),
				() => new Rectangle(0, 0, Width, Height));

			_layout.Children.Add(new SpinningCircleControl
			{
				BindingContext = this,
				IsCounterClockWise = true,
				Angle = 70,
				DurationMilliseconds = 1500
			}
				.BindTo(SpinningCircleControl.ColorProperty, nameof(Color))
				.BindTo(SpinningCircleControl.IsRunningProperty, nameof(IsRunning)),
				 () => new Rectangle(Width / 2 - ((Width * 0.80) / 2), Height / 2 - ((Height * 0.80) / 2),
					   Width * 0.80, Height * 0.80));

			_layout.Children.Add(new SpinningCircleControl
			{
				BindingContext = this,
				Angle = 160,
				DurationMilliseconds = 1000
			}
				 .BindTo(SpinningCircleControl.ColorProperty, nameof(Color))
				 .BindTo(SpinningCircleControl.IsRunningProperty, nameof(IsRunning)),
				 () => new Rectangle(Width / 2 - ((Width * 0.60) / 2), Height / 2 - ((Height * 0.6) / 2),
					   Width * 0.6, Height * 0.60));
		}

		/// <summary>
		/// The IsRunning property.
		/// </summary>
		public static BindableProperty IsRunningProperty = BindableProperty.Create(
			nameof(IsRunning), typeof(bool), typeof(FluidActivityIndicator), false,
			BindingMode.OneWay);

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
			MvvmApp.Current.Colors.Get(Config.PrimaryColor), BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the Color of the CustomActivitySpinner instance.
		/// </summary>
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
	}

	class SpinningCircleControl : NControlView, IDisposable
	{
		enum SpinnerState
		{
			NotRunning,
			Starting,
			Running,
			Stopping,
		};

		bool _isDisposed;
		SpinnerState _state;

		public SpinningCircleControl()
		{
			StartAnimation();
		}

		void StartAnimation()
		{
			Invalidate();
			Action animationAction = null;
			animationAction = () =>
			{
				if (_isDisposed)
					return;

				switch (_state)
				{
					case SpinnerState.Starting:

						// Animate in
						_state = SpinnerState.Running;

						new XAnimationPackage(this)
							.Duration(DurationMilliseconds)
							.Easing(EasingFunction.EaseInOut)
							.Rotate(IsCounterClockWise ? -1 * (Angle + 360) : Angle + 360)
							.Animate()
							.Rotate(IsCounterClockWise ? -1 * Angle : Angle)
							.Set()
							.Animate()
							.Run(animationAction);

						new XAnimationPackage(this)
							.Opacity(0.0)
							.Rotate(Angle)
							.Set()
							.Duration(150)
							.Easing(EasingFunction.EaseInOut)
							.Opacity(1.0)
							.Animate()
							.Run();

						break;

					case SpinnerState.Running:

						// Animate running
						new XAnimationPackage(this)
							.Duration(DurationMilliseconds)
							.Easing(EasingFunction.EaseInOut)
							.Rotate(IsCounterClockWise ? -1 * (Angle + 360) : Angle + 360)
							.Animate()
							.Rotate(IsCounterClockWise ? -1 * Angle : Angle)
							.Set()
							.Animate()
							.Run(animationAction);

						break;

					case SpinnerState.Stopping:

						// Animate stopping
						_state = SpinnerState.NotRunning;

						//new XAnimationPackage(this)
						//	.Duration(150)
						//	.Easing(EasingFunction.EaseInOut)
						//	.Opacity(0.0)
						//	.Animate()
						//	.Run(animationAction);

						break;

					case SpinnerState.NotRunning:
						// No animation
						break;
				}
			};

			animationAction();
		}

		/// <summary>
		/// The IsRunning property.
		/// </summary>
		public static BindableProperty IsRunningProperty = BindableProperty.Create(
			nameof(IsRunning), typeof(bool), typeof(SpinningCircleControl), false,
			BindingMode.OneWay, null, propertyChanged: (bindable, oldValue, newValue) =>
			{
				var ctrl = (SpinningCircleControl)bindable;
				if (oldValue == newValue)
					return;

				if ((bool)newValue == true)
				{
					ctrl._state = SpinnerState.Starting;
					ctrl.StartAnimation();
				}
				else
				{
					ctrl._state = SpinnerState.Stopping;

					new XAnimationPackage(ctrl)
						.Duration(150)
						.Easing(EasingFunction.EaseInOut)
						.Scale(0.0)
						.Animate()
						.Run(()=> ctrl.Scale = 1.0);
				}
			});

		/// <summary>
		/// Gets or sets the IsRunning of the CustomActivitySpinner instance.
		/// </summary>
		public bool IsRunning
		{
			get { return (bool)GetValue(IsRunningProperty); }
			set { SetValue(IsRunningProperty, value); }
		}

		/// <summary>
		/// The IsCounterClockWise property.
		/// </summary>
		public static BindableProperty IsCounterClockWiseProperty = BindableProperty.Create(
			nameof(IsCounterClockWise), typeof(bool), typeof(SpinningCircleControl), false,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the IsCounterClockWise of the CustomActivitySpinner instance.
		/// </summary>
		public bool IsCounterClockWise
		{
			get { return (bool)GetValue(IsCounterClockWiseProperty); }
			set { SetValue(IsCounterClockWiseProperty, value); }
		}

		/// <summary>
		/// The SpinnerColor property.
		/// </summary>
		public static BindableProperty ColorProperty = BindableProperty.Create(
			nameof(Color), typeof(Color), typeof(SpinningCircleControl),
			MvvmApp.Current.Colors.Get(Config.TextColor), BindingMode.OneWay,
			null, propertyChanged: (bindable, oldValue, newValue) =>
			{
				var ctrl = (SpinningCircleControl)bindable;
				ctrl.Invalidate();
			});

		/// <summary>
		/// Gets or sets the SpinnerColor of the CustomActivitySpinner instance.
		/// </summary>
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}

		/// <summary>
		/// The Angle property.
		/// </summary>
		public static BindableProperty AngleProperty = BindableProperty.Create(
			nameof(Angle), typeof(double), typeof(SpinningCircleControl), 0.0,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the Angle of the CustomActivitySpinner instance.
		/// </summary>
		public double Angle
		{
			get { return (double)GetValue(AngleProperty); }
			set { SetValue(AngleProperty, value); }
		}

		/// <summary>
		/// The DurationMilliseconds property.
		/// </summary>
		public static BindableProperty DurationMillisecondsProperty = BindableProperty.Create(
			nameof(DurationMilliseconds), typeof(int), typeof(SpinningCircleControl), 1250,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the DurationMilliseconds of the CustomActivitySpinner instance.
		/// </summary>
		public int DurationMilliseconds
		{
			get { return (int)GetValue(DurationMillisecondsProperty); }
			set { SetValue(DurationMillisecondsProperty, value); }
		}

		#region IDisposable

		public void Dispose()
		{
			_isDisposed = true;
		}

		#endregion

		#region Drawing

		/// <summary>
		/// Drawing
		/// </summary>
		public override void Draw(NGraphics.ICanvas canvas, NGraphics.Rect rect)
		{
			base.Draw(canvas, rect);

			if (!IsRunning)
				return;

			if (rect.Width > rect.Height)
				rect = new NGraphics.Rect((rect.Width - rect.Height) / 2, rect.Y, rect.Height, rect.Height);
			else if (rect.Height > rect.Width)
				rect = new NGraphics.Rect(rect.X, (rect.Height - rect.Width) / 2, rect.Width, rect.Width);

			rect.Inflate(-2, -2);

			canvas.DrawEllipse(rect, new NGraphics.Pen(
				Color.MultiplyAlpha(0.15).ToNColor(), 2 * MvvmApp.Current.Environment.DisplayDensity), null);

			canvas.DrawPath(new NGraphics.PathOp[]{
				new NGraphics.MoveTo(rect.X, rect.Y + rect.Height/2),
				new NGraphics.ArcTo(
					new NGraphics.Size(rect.Width/2, rect.Height/2),
				true, false,
					new NGraphics.Point(rect.X + rect.Width, rect.Y + rect.Height/2)),

			}, new NGraphics.Pen(Color.ToNColor(), 2 * MvvmApp.Current.Environment.DisplayDensity), null);

		}
		#endregion
	}
}
