using System;
using System.Collections.Generic;
using NControl.Mvvm;
using NControl.XAnimation;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class CityView : BaseFluidContentsView<CityViewModel>, INavigationContainerProvider
	{
		public INavigationContainer CreateNavigationContainer(PresentationMode mode)
		{
			return new FluidTransitionNavigationContainer(true);
		}

		protected override View CreateContents()
		{
			return new Grid().AddChildTo(
				new Image
				{
					Aspect = Aspect.AspectFill,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
				}
				.BindTo(Image.SourceProperty, nameof(ViewModel.Image))
				.SetTransitionIdentifier("image", TransitionTarget.Target)
				.SetTransitionIdentifier("image-details", TransitionTarget.Source)
				.AddBehaviorTo(new ClickBehavior(() => ViewModel.ViewDetailsCommand,
												 () => ViewModel.CityModel)),
				0, 0)

				.AddChildTo(new VerticalStackLayoutWithSmallPadding
				{				
					BackgroundColor = Color.Black.MultiplyAlpha(0.5),
					VerticalOptions = LayoutOptions.EndAndExpand,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Children = {
						new BoxView{
							HeightRequest = 0,
						}.SetTransitionIdentifier("name-details", TransitionTarget.Source),
						new Label {
							TextColor = Color.White,
							FontAttributes = FontAttributes.Bold,
							FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
						}.BindTo(Label.TextProperty, nameof(ViewModel.City))
						 .SetTransitionIdentifier("city", TransitionTarget.Target),

						new Label {
							TextColor = Color.White,
							FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
						}.BindTo(Label.TextProperty, nameof(ViewModel.Name))
						 .SetTransitionIdentifier("name", TransitionTarget.Target)
					     
					}
				}, 0, 0);
		}
	}
}
