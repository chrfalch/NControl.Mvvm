using System;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class CityDetailsView : BaseFluidContentsView<CityDetailsViewModel>, 
		INavigationContainerProvider
	{
		public INavigationContainer CreateNavigationContainer(PresentationMode mode)
		{
			return new FluidTransitionNavigationContainer(true);
		}

		protected override View CreateContents()
		{
			return new VerticalStackLayout
			{
				Spacing = 0,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions= LayoutOptions.FillAndExpand,
				Children =
				{
					new Image
					{
						Aspect = Aspect.AspectFill,
						HeightRequest = 180,
					}
					.BindTo(Image.SourceProperty, nameof(ViewModel.Image))
					.SetTransitionIdentifier("image-details", TransitionTarget.Target),

					new ScrollView{
						VerticalOptions = LayoutOptions.FillAndExpand,
						BackgroundColor = Config.ViewBackgroundColor,
						Content = new Label{
							Margin = Config.DefaultPadding,
						}
						.BindTo(Label.TextProperty, nameof(ViewModel.Description))
					}
					.SetTransitionIdentifier("name-details", TransitionTarget.Target)
				}
			};
		}
	}
}
