using System;
using NControl.Abstractions;
using NControl.Controls;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public class FluidActivityIndicator: IActivityIndicator
	{
		readonly IActivityIndicatorViewProvider _provider;
		readonly View _overlay;
		readonly Label _titleLabel;
		readonly Label _subTitleLabel;
		readonly CustomActivityIndicator _spinner;

		public FluidActivityIndicator(IActivityIndicatorViewProvider provider)
		{
			_provider = provider;

			_titleLabel = new Label { 
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
			};
			_subTitleLabel = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.White,
			};

			_spinner = new CustomActivityIndicator
			{
				HeightRequest = 44,
			};

			_overlay = new ContentView
			{
				BackgroundColor = Color.Black.MultiplyAlpha(0.83),
				Content = new VerticalWizardStackLayout
				{
					HorizontalOptions = LayoutOptions.CenterAndExpand,
					VerticalOptions = LayoutOptions.CenterAndExpand,
					Children = {
						new StackLayout{
							Padding = 0,
							Spacing =0,
							HorizontalOptions = LayoutOptions.Center,
							VerticalOptions = LayoutOptions.Center,
							HeightRequest = 44,
							WidthRequest = 44,
							Children = {
								_spinner
							}
						},
						new VerticalStackLayout{
							Padding = 24,
							Spacing = 8,
							Children = {
								_titleLabel,
								_subTitleLabel,
							}
						}
					}
				},
			};
		}

		public void UpdateProgress(bool visible, string title = "", string subtitle = "")
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				if (visible)
				{
					_titleLabel.Text = title;
					_subTitleLabel.Text = subtitle;
				}

				// Hide
				if (!visible && _overlay.Parent != null)
				{
					new XAnimationPackage(_spinner).Scale(0.0).Run(()=> new XAnimationPackage(_overlay).Opacity(0.0).Run(() =>
					   {
						   _provider.RemoveFromParent(_overlay);
							_titleLabel.Text = title;
						   _subTitleLabel.Text = subtitle;
						   _spinner.IsRunning = false;
					   }));
				}
				else if (visible && _overlay.Parent == null)
				{					
					_spinner.IsRunning = true;
					_overlay.Opacity = 0.0;
					_provider.AddToParent(_overlay);
					new XAnimationPackage(_spinner).Scale(1.0).Run();
					new XAnimationPackage(_overlay).Opacity(1.0).Run();
				}
			});
		}
	}

	public class CustomActivityIndicator: RelativeLayout
	{
		public CustomActivityIndicator()
		{
			// add spinners
			Children.Add(new CustomActivitySpinner { BindingContext = this, Angle = 0, DurationMilliseconds = 2500 }
			             .BindTo(CustomActivitySpinner.IsRunningProperty, nameof(IsRunning)), () =>
						  new Rectangle(0, 0, Width, Height));

			Children.Add(new CustomActivitySpinner { BindingContext = this, Angle = -70, DurationMilliseconds = 1500 }
			             .BindTo(CustomActivitySpinner.IsRunningProperty, nameof(IsRunning)), () =>
			             new Rectangle(Width/2 - ((Width*0.75)/2), Height / 2 - ((Height * 0.75) / 2), 
			                           Width*0.75, Height*0.75));

			Children.Add(new CustomActivitySpinner { BindingContext = this, Angle = 160, DurationMilliseconds = 1000 }
			             .BindTo(CustomActivitySpinner.IsRunningProperty, nameof(IsRunning)), () =>
						 new Rectangle(Width / 2 - ((Width * 0.5) / 2), Height / 2 - ((Height * 0.5) / 2), 
			                           Width * 0.5, Height * 0.5));
		}

		/// <summary>
		/// The IsRunning property.
		/// </summary>
		public static BindableProperty IsRunningProperty = BindableProperty.Create(
			nameof(IsRunning), typeof(bool), typeof(CustomActivityIndicator), false,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the IsRunning of the CustomActivityIndicator instance.
		/// </summary>
		public bool IsRunning
		{
			get { return (bool)GetValue(IsRunningProperty); }
			set { SetValue(IsRunningProperty, value); }
		}
	}

	public class CustomActivitySpinner : NControlView, IDisposable
	{
		bool _isDisposed;

		public CustomActivitySpinner()
		{
			StartAnimation();
		}

		void StartAnimation()
		{
			Action animationAction = null;
			animationAction = () =>
			{
				if (!_isDisposed && IsRunning)
					new XAnimationPackage(this)
						.Duration(DurationMilliseconds)
						.Rotate(Angle + 360)
						.Animate()
						.Rotate(Angle)
						.Set()
						.Animate()
						.Run(animationAction);
			};

			animationAction();
		}

		/// <summary>
		/// The IsRunning property.
		/// </summary>
		public static BindableProperty IsRunningProperty = BindableProperty.Create(
			nameof(IsRunning), typeof(bool), typeof(CustomActivitySpinner), false,
			BindingMode.OneWay, null, propertyChanged: (bindable, oldValue, newValue) =>
			{
				var ctrl = (CustomActivitySpinner)bindable;
				if(oldValue != newValue && (bool)newValue == true)				
					ctrl.StartAnimation();
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
		/// The SpinnerColor property.
		/// </summary>
		public static BindableProperty SpinnerColorProperty = BindableProperty.Create(
			nameof(SpinnerColor), typeof(Color), typeof(CustomActivitySpinner), Color.White,
			BindingMode.OneWay, null, propertyChanged: (bindable, oldValue, newValue) =>
			{
				var ctrl = (CustomActivitySpinner)bindable;
				ctrl.Invalidate();
			});

		/// <summary>
		/// Gets or sets the SpinnerColor of the CustomActivitySpinner instance.
		/// </summary>
		public Color SpinnerColor
		{
			get { return (Color)GetValue(SpinnerColorProperty); }
			set { SetValue(SpinnerColorProperty, value); }
		}

		/// <summary>
		/// The Angle property.
		/// </summary>
		public static BindableProperty AngleProperty = BindableProperty.Create(
			nameof(Angle), typeof(double), typeof(CustomActivitySpinner), 0.0,
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
			nameof(DurationMilliseconds), typeof(int), typeof(CustomActivitySpinner), 1250,
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

			if (rect.Width > rect.Height)
				rect = new NGraphics.Rect((rect.Width - rect.Height) / 2, rect.Y, rect.Height, rect.Height);
			else if (rect.Height > rect.Width)
				rect = new NGraphics.Rect(rect.X, (rect.Height - rect.Width) / 2, rect.Width, rect.Width);

			rect.Inflate(-2, -2);

			canvas.DrawPath(new NGraphics.PathOp[]{
				new NGraphics.MoveTo(rect.X, rect.Y + rect.Height/2),
				new NGraphics.ArcTo(
					new NGraphics.Size(rect.Width/2, rect.Height/2),
				true, false,
					new NGraphics.Point(rect.X + rect.Width, rect.Y + rect.Height/2)),

			}, new NGraphics.Pen(SpinnerColor.ToNColor(), 2 * MvvmApp.Current.Environment.DisplayDensity), null);

		}
		#endregion
	}
}
