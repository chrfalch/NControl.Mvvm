using System;
using NControl.Abstractions;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public class FluidActivityIndicator: IActivityIndicator
	{
		readonly IActivityIndicatorViewProvider _provider;
		readonly View _overlay;
		readonly Label _titleLabel;
		readonly Label _subTitleLabel;
		readonly CustomActivitySpinner _spinner;

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

			_spinner = new CustomActivitySpinner
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
				_titleLabel.Text = title;
				_subTitleLabel.Text = subtitle;
				_spinner.Invalidate();

				// Hide
				if (!visible && _overlay.Parent != null)
				{
					new XAnimation.XAnimationPackage(_overlay).Opacity(0.0).Run(()=> _provider.RemoveFromParent(_overlay));
				}
				else if (visible && _overlay.Parent == null)
				{					
					_overlay.Opacity = 0.0;
					_provider.AddToParent(_overlay);
					new XAnimation.XAnimationPackage(_overlay).Opacity(1.0).Run();
				}
			});
		}
	}

	public class CustomActivitySpinner : NControlView, IDisposable
	{
		bool _isDisposed;

		public CustomActivitySpinner()
		{
			Action animationAction = null;
			animationAction = ()=> 
			{
				if(!_isDisposed)
					new XAnimation.XAnimationPackage(this)					              
					              
					              .Rotate(180)
								  .Run();
			};

			animationAction();
		}

		public void Dispose()
		{
			_isDisposed = true;
		}

		public override void Draw(NGraphics.ICanvas canvas, NGraphics.Rect rect)
		{
			base.Draw(canvas, rect);

			if (rect.Width > rect.Height)
				rect = new NGraphics.Rect((rect.Width - rect.Height) / 2, rect.Y, rect.Height, rect.Height);
			else if (rect.Height > rect.Width)
				rect = new NGraphics.Rect(rect.X, (rect.Height - rect.Width) / 2, rect.Width, rect.Width);

			rect.Inflate(-2, -2);

			canvas.DrawPath(new NGraphics.PathOp[]{
				new NGraphics.MoveTo(2, 2 + rect.Height/2),
				new NGraphics.ArcTo(new NGraphics.Size(
					rect.Width/2, rect.Height/2),true, false,
                    new NGraphics.Point(rect.Width, rect.Height/2)),				
			}, new NGraphics.Pen(NGraphics.Colors.White, 2), null);

		}
	}
}
