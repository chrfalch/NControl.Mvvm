using System;
using System.Collections.Generic;
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

		public IEnumerable<XInterpolationPackage> OverrideTransition(
			string transitionIdentifier, VisualElement fromElement, VisualElement toElement, 
			Rectangle fromRect, Rectangle toRectangle, XInterpolationPackage package)
		{
			var list = new List<XInterpolationPackage>();
			list.Add(package);

			if (transitionIdentifier == "name-details")
			{
				// Create new package
				//list.Add(new XInterpolationPackage(fromElement.Parent as VisualElement)				         
				//         .Add((transform) => transform.SetDuration(1).SetTranslation(0, (fromElement.Parent as VisualElement).Height))
				//         .Add((transform) => transform.SetDuration(99).SetTranslation(0, 0)) 
				//         as XInterpolationPackage);
			}

			return list;
		}

		protected override View CreateContents()
		{
			return new Grid().AddChildTo(new VerticalStackLayout
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
						HorizontalOptions = LayoutOptions.FillAndExpand,
						BackgroundColor = Config.ViewBackgroundColor,
						Content = new Label{
							Margin = Config.DefaultPadding * 2,
						}
						.BindTo(Label.TextProperty, nameof(ViewModel.Description))
					}.SetTransitionIdentifier("name-details", TransitionTarget.Target),
				}
			}, 0, 0)
             .AddChildTo(new VerticalStackLayoutWithSmallPadding{
				 BackgroundColor = Color.Black.MultiplyAlpha(0.5),
				 VerticalOptions = LayoutOptions.End,
				 HorizontalOptions = LayoutOptions.FillAndExpand,
				 Children = {
					new Label {
						TextColor = Color.White,
						FontAttributes = FontAttributes.Bold,
						FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
					}.BindTo(Label.TextProperty, nameof(ViewModel.City)),

					new Label {
						TextColor = Color.White,
						FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
					}.BindTo(Label.TextProperty, nameof(ViewModel.Name))
				}
			},0,0);
		}
	}
}
