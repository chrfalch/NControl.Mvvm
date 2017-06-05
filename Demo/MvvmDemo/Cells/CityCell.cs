using System;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class CityCell : ExtendedViewCell
	{
		readonly Label _nameLabel;
		readonly Label _cityLabel;
		readonly StackLayout _contents;
		readonly Image _image;

		protected override void OnTapped()
		{
			base.OnTapped();

			_nameLabel.SetTransitionIdentifier("name", TransitionTarget.Source);
			_image.SetTransitionIdentifier("image", TransitionTarget.Source);
			_cityLabel.SetTransitionIdentifier("city", TransitionTarget.Source);
		}

		public CityCell()
		{
			_nameLabel = new Label
			{
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
			};

			_cityLabel = new Label
			{
				TextColor = Color.White,
				FontAttributes = FontAttributes.Bold,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
			};

			_contents = new VerticalStackLayoutWithSmallPadding
			{
				VerticalOptions = LayoutOptions.End,
			};

			_image = new Image
			{
				HeightRequest = 180,
				Aspect = Aspect.AspectFill,
			};

			View = new FluidShadowView
			{
				Margin = new Thickness(8, 6, 12, 6),
				BorderRadius = 2,
				Content = new FluidRoundCornerView
				{
					BorderRadius = 2,
					Content = new Grid()
						.AddChildTo(_image, 0, 0)
						.AddChildTo(_contents, 0, 0),
				}
			};

			_contents.Children.Add(_cityLabel);
			_contents.Children.Add(_nameLabel);
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (BindingContext != null)
			{
				_nameLabel.Text = (BindingContext as FeedItem).Name;
				_cityLabel.Text = (BindingContext as FeedItem).City;
				_image.Source = (BindingContext as FeedItem).Image;
			}
			else
			{
				_nameLabel.Text = "";
				_cityLabel.Text = "";
				_image.Source = "";
			}
		}
	}
}
