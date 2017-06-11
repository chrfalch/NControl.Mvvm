using System;
using System.Linq;
using NControl.Abstractions;
using NControl.Controls;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidActivityIndicatorView<TActivityIndicator>: IActivityIndicator
		where TActivityIndicator : BaseFluidActivityIndicator, new()
	{
		readonly IActivityIndicatorViewProvider _provider;
		readonly View _overlay;
		readonly Label _titleLabel;
		readonly Label _subTitleLabel;
		readonly TActivityIndicator _spinner;

		public FluidActivityIndicatorView(IActivityIndicatorViewProvider provider)
		{
			_provider = provider;

			_titleLabel = new Label { 
				HorizontalTextAlignment = TextAlignment.Center,
				TextColor = Config.NegativeTextColor,
			};

			_subTitleLabel = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Config.NegativeTextColor,
			};

			_spinner = new TActivityIndicator
			{
				HeightRequest = 38,
			};

			_overlay = new ContentView
			{
				BackgroundColor = Config.ViewTransparentBackgroundColor,
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
							HeightRequest = 38,
							WidthRequest = 38,
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

				if (!visible && _overlay.Parent != null)
				{
					// Hide
					var animation = new XAnimationPackage(_overlay);
					animation.SetDuration(150).Add(
						(transform) => transform.SetOpacity(0.0));

					animation.Animate(() => {
					   _provider.RemoveFromParent(_overlay);
						_titleLabel.Text = title;
					   _subTitleLabel.Text = subtitle;
					   _spinner.IsRunning = false;
					});
				}
				else if (visible && _overlay.Parent == null)
				{		
					// Show
					_spinner.IsRunning = true;
					_overlay.Opacity = 0.0;
					_provider.AddToParent(_overlay);
					var animation = new XAnimationPackage(_overlay);
					animation.SetDuration(150).Add().SetOpacity(1.0);
					animation.Animate();
				}
			});
		}
	}
}
