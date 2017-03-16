using System;
using Xamarin.Forms;
using NControl.Mvvm;

namespace MvvmDemo
{
	public class FeedCell: ExtendedViewCell
	{
		readonly Label _label;
		readonly StackLayout _contents;
		readonly Image _image;

		public FeedCell()
		{
			_label = new Label
			{
				Margin = 4,
				FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
			};

			View = new FluidRoundCornerView
			{
				BorderRadius = 4,
				Margin = new Thickness(8, 6, 12, 6),
				Content = _contents = new VerticalStackLayout {  
					BackgroundColor = Color.FromHex("ECECEC"),
				},
			};

			_image = new Image { 
				HeightRequest = 165,
				Aspect = Aspect.AspectFill,
			};

			_contents.Children.Add(_image);
			_contents.Children.Add(_label);
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (BindingContext != null)
			{
				_label.Text = (BindingContext as FeedItem).Name;
				_image.Source = (BindingContext as FeedItem).Image;
			}
			else
			{
				_label.Text = "";
				_image.Source = "";
			}
		}
	}
}
