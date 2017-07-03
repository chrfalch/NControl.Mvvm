using System;
using System.Collections.Generic;
using NControl.Mvvm;
using NControl.XAnimation;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class FeedDetailsView : BaseFluidContentsView<FeedDetailsViewModel>,
		INavigationContainerProvider, IXViewTransitionable
	{
		Label _nameLabel;
		Label _cityLabel;
		Image _image;
		View _footerView;
		View _whiteBox;
		ContentView _labelContainer;

		public INavigationContainer CreateNavigationContainer(PresentationMode mode)
		{
			return new FluidTransitionNavigationContainer(true);
		}

		protected override View CreateContents()
		{
			_nameLabel = new Label
			{
				VerticalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label)),
			}.BindTo(Label.TextProperty, nameof(ViewModel.Name));

			_cityLabel = new Label
			{
				VerticalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
			}.BindTo(Label.TextProperty, nameof(ViewModel.City));

			_image = new Image
			{				
				Aspect = Aspect.AspectFill,	

			}.BindTo(Image.SourceProperty, nameof(ViewModel.Image));

			var labelShadow = new ContentView
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Content = _labelContainer = new ContentView
				{
					Content = new VerticalStackLayout
					{
						Margin = 25,
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center,
						Children = {
							_nameLabel,
							_cityLabel
						}
					},
					BackgroundColor = Color.Blue,
				},
			};

			_whiteBox = new BoxView { BackgroundColor = Color.White.MultiplyAlpha(0.6) };

			_labelContainer.SetTransitionIdentifier("labels", TransitionTarget.Target);
			_image.SetTransitionIdentifier("image", TransitionTarget.Target);

			return new Grid
			{
				RowSpacing = 0,
				ColumnSpacing = 0,
				RowDefinitions = new RowDefinitionCollection{
					new RowDefinition { Height = new GridLength(33, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength(33, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength(33, GridUnitType.Star) }
				}
			}.AddChildTo(_image, 0, 0)
			 .AddChildTo(new Grid
				 {
					 RowSpacing = 0,
					 ColumnSpacing = 0,
					 ColumnDefinitions = new ColumnDefinitionCollection{
						new ColumnDefinition { Width = new GridLength(50, GridUnitType.Star) },
						new ColumnDefinition { Width = new GridLength(50, GridUnitType.Star) }
					}
				 }.AddChildTo(labelShadow, 0, 0)
				  .AddChildTo(_whiteBox, 1, 0), 0, 1)
			 
			 .AddChildTo(_footerView = new ScrollView { 
				BackgroundColor = Color.DarkBlue ,
				Content =new Label { 
					TextColor =Color.White,
					VerticalTextAlignment = TextAlignment.Center,
					HorizontalTextAlignment= TextAlignment.Center,
					Margin = 10,
				}.BindTo(Label.TextProperty, nameof(ViewModel.Description))
				 
			}, 0, 2);
		}

		protected override IEnumerable<XAnimationPackage> DefaultTransitionIn(
			INavigationContainer container, IEnumerable<XAnimationPackage> animations)
		{
			var retVal = new List<XAnimationPackage>(base.DefaultTransitionIn(container, animations));

			retVal.Add(new XAnimationPackage(_footerView, _whiteBox)
			           .Set((t) => t.SetOpacity(0.0))
			           .Add((t) => t.SetOpacity(1.0))
					   as XAnimationPackage);

			return retVal;
		}

		protected override IEnumerable<XAnimationPackage> DefaultTransitionOut(
			INavigationContainer container, IEnumerable<XAnimationPackage> animations)
		{
			var retVal = new List<XAnimationPackage>(base.DefaultTransitionIn(container, animations));

			retVal.Add(new XAnimationPackage(_footerView, _whiteBox)					   
					   .Add((t) => t.SetOpacity(0.0))
					   as XAnimationPackage);

			return retVal;
		}

		public IEnumerable<XInterpolationPackage> OverrideTransition(
			string transitionIdentifier, VisualElement fromElement, 
			VisualElement toElement, Rectangle fromRect, Rectangle toRectangle, IEnumerable<XInterpolationPackage> packages)
		{
			return packages;
		}

		public void BeforeOverrideTransition(
			string transitionIdentifier, VisualElement fromElement, VisualElement toElement, 
			ref Rectangle fromRect, ref Rectangle toRectangle)
		{
			if (toElement == _labelContainer)
			{
				toElement.Opacity = fromElement.Opacity;
				toElement.BackgroundColor = fromElement.BackgroundColor;
			}
			else if (toElement == _image)
			{
				fromRect.Y -= (fromElement as View).Margin.Top;
			}
		}
	}
}
