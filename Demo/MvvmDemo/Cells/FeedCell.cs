using System;
using Xamarin.Forms;
using NControl.Mvvm;

namespace MvvmDemo
{
	public class FeedCell: ExtendedViewCell
	{
		readonly Label _nameLabel;
		readonly Label _cityLabel;
		readonly Grid _contents;
		readonly Image _image;
		readonly ContentView _labelContainer;

		protected override void OnTapped()
		{
			_labelContainer.SetTransitionIdentifier("labels", TransitionTarget.Source);
			_image.SetTransitionIdentifier("image", TransitionTarget.Source);

			base.OnTapped();
		}

		public FeedCell()
		{
			_nameLabel = new Label
			{
				VerticalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label)),
			};

			_cityLabel = new Label
			{
				VerticalTextAlignment = TextAlignment.Center,
				TextColor = Color.White,
				FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
			};

			var labelGrid = new Grid { 
				ColumnDefinitions = new ColumnDefinitionCollection{ 
					new ColumnDefinition { Width = new GridLength(
						IntGetColumnForLabel() == 0 ? 60 : 40, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength(
						IntGetColumnForLabel() == 1 ? 60 : 40, GridUnitType.Star) }
				}
			};

			var labelShadow = new FluidShadowView
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Content = _labelContainer = new ContentView
				{
					Content = new VerticalStackLayout { 
						Margin = 25,
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center,
						Children = { 
							_nameLabel,
							_cityLabel
						} 
					},
					BackgroundColor = IntGetColorForLabel(),
				},
			};

			labelGrid.Children.Add(labelShadow, IntGetColumnForLabel(), 0);

			_contents = new Grid { Padding= 14};

			_image = new Image { 
				HeightRequest = 165,
				Aspect = Aspect.AspectFill,
				Margin = new Thickness(0, 14),
			};

			_contents.Children.Add(new FluidShadowView
			{
				Content = _image
			});

			_contents.Children.Add(labelGrid);

			View = _contents;
		}

		int IntGetColumnForLabel()
		{
			return GetColumnForLabel();
		}

		protected virtual int GetColumnForLabel()
		{
			return 0;
		}

		Color IntGetColorForLabel()
		{
			return GetColorForLabel();
		}

		protected virtual Color GetColorForLabel()
		{
			return Color.LightCoral.MultiplyAlpha(0.75);
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

	public class OddFeedCell : FeedCell
	{
		protected override int GetColumnForLabel()
		{
			return 1;
		}

		protected override Color GetColorForLabel()
		{
			return Color.LightBlue.MultiplyAlpha(0.75);
		}
	}
}
