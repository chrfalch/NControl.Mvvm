using System;
using System.Collections.Generic;
using NControl.Mvvm;
using NControl.XAnimation;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class CityView: BaseFluidContentsView<CityViewModel>, INavigationContainerProvider
	{
		VerticalStackLayout _bottomBox;

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
				.AddBehaviorTo(new ClickBehavior(()=> ViewModel.ViewDetailsCommand, ()=> ViewModel.CityModel)), 
				0, 0)

				.AddChildTo(_bottomBox = new VerticalStackLayoutWithSmallPadding
				{
					//BackgroundColor = Color.Black.MultiplyAlpha(0.5),
					//VerticalOptions = LayoutOptions.StartAndExpand,
					//HorizontalOptions = LayoutOptions.FillAndExpand,
					Children = {
						new Label {
							TextColor = Color.Red,							
						BackgroundColor = Color.Yellow,
							FontAttributes = FontAttributes.Bold,
							FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
						}.BindTo(Label.TextProperty, nameof(ViewModel.City))
					     .SetTransitionIdentifier("city", TransitionTarget.Target),

						new Label {
							TextColor = Color.Red,							
						BackgroundColor = Color.Yellow,
							FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
						}.BindTo(Label.TextProperty, nameof(ViewModel.Name))
					     .SetTransitionIdentifier("name", TransitionTarget.Target),
					}
			}, 0, 0);
		}

		protected override IEnumerable<XAnimationPackage> DefaultTransitionIn(
			INavigationContainer container, IEnumerable<XAnimationPackage> animations)
		{
			var list = new List<XAnimationPackage>();
			list.AddRange(base.DefaultTransitionIn(container, animations));
			//list.Add(new XAnimationPackage(_bottomBox).SetDuration(50)
		 //        .Set((transform) => transform.SetOpacity(0))
		 //        .Add((transform) => transform
		 //             .SetEasing(EasingFunctions.EaseInOut)
			//          .SetOpacity(0)) 
		 //        as XAnimationPackage);
			
			return list;
		}

		protected override IEnumerable<XAnimationPackage> DefaultTransitionOut(
			INavigationContainer container, IEnumerable<XAnimationPackage> animations)
		{
			var list = new List<XAnimationPackage>();
			list.AddRange(base.DefaultTransitionOut(container, animations));
			list.Add(new XAnimationPackage(_bottomBox).SetDuration(50)
				 .Add((transform) => transform
					  .SetEasing(EasingFunctions.EaseInOut)
			          .SetOpacity(0.0))
				 as XAnimationPackage);

			return list;
		}
	}
}
