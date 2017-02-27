using System;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public class FluidActivityIndicator: IActivityIndicator
	{
		readonly IActivityIndicatorViewProvider _provider;
		readonly View _overlay;
		readonly Label _titleLabel;
		readonly Label _subTitleLabel;

		public FluidActivityIndicator(IActivityIndicatorViewProvider provider)
		{
			_provider = provider;

			_titleLabel = new Label { HorizontalTextAlignment = TextAlignment.Center };
			_subTitleLabel = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))				                              
			};

			_overlay = new ContentView
			{
				BackgroundColor = Color.Black.MultiplyAlpha(0.43),
				Content = new VerticalStackLayout
				{
					HorizontalOptions = LayoutOptions.CenterAndExpand,
					VerticalOptions = LayoutOptions.CenterAndExpand,
					Children = {
						new ActivityIndicator{
							IsRunning = true,
							Color = Color.White,
						},
						_titleLabel,
						_subTitleLabel,
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

				// Hide
				if (!visible && _overlay.Parent != null)
				{
					new XAnimation.XAnimation(_overlay).Opacity(0.0).Run(()=> _provider.RemoveFromParent(_overlay));
				}
				else if (visible && _overlay.Parent == null)
				{					
					_overlay.Opacity = 0.0;
					_provider.AddToParent(_overlay);
					new XAnimation.XAnimation(_overlay).Opacity(1.0).Run();
				}

				System.Diagnostics.Debug.WriteLine(this.GetType().Name + " UpdateProgress(" + visible + ", '" + title + "', '" + subtitle + "')");
			});
		}
	}
}
