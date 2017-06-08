using System;
using NControl.Mvvm;
using NControl.XAnimation;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class CityDetailsView : BaseFluidContentsView<CityDetailsViewModel>, 
		INavigationContainerProvider, IXViewTransitionable
	{
		public INavigationContainer CreateNavigationContainer(PresentationMode mode)
		{
			return new FluidTransitionNavigationContainer(true);
		}

		public XInterpolationPackage OverrideTransition(
			string transitionIdentifier, VisualElement fromElement, VisualElement toElement, 
			Rectangle fromRect, Rectangle toRectangle, XInterpolationPackage package)
		{
			//if (transitionIdentifier == "image-details")
			//{
			//	// Create new package
			//	return new XInterpolationPackage(toElement)
			//		.Set((transform) => transform.SetRectangle(new Rectangle(fromRect.X, fromRect.Y, 
			//		                                                         fromRect.Width, fromRect.Height - 80)))
			//		.Add((transform) => transform.SetRectangle(toRectangle)) 
			//		as XInterpolationPackage;
			//}

			return package;
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
						HeightRequest = 220,
					}
					.BindTo(Image.SourceProperty, nameof(ViewModel.Image))
					.SetTransitionIdentifier("image-details", TransitionTarget.Target),

					new ScrollView{
						VerticalOptions = LayoutOptions.FillAndExpand,
						BackgroundColor = Config.ViewBackgroundColor,
						Content = new Label{
							Margin = Config.DefaultPadding * 2,
						}
						.BindTo(Label.TextProperty, nameof(ViewModel.Description))
					}
					.SetTransitionIdentifier("name-details", TransitionTarget.Target)
				}
			};
		}
	}
}
