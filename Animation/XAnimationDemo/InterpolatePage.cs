using System;
using NControl.XAnimation;
using Xamarin.Forms;

namespace XAnimationDemo
{
	public class InterpolatePage: ContentPage
	{
		public InterpolatePage()
		{
			var box = new ContentView
			{
				BackgroundColor = Color.Aqua,
				HeightRequest = 44,
				Content = new Label{
					Text = "Hello World",
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
				},
			};

			var box2 = new ContentView
			{
				BackgroundColor = Color.Lime,
				HeightRequest = 44,
				Rotation = 45,
				Content = new Label
				{
					Text = "Goodbye",
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
				},
			};

			var slider = new Slider
			{
				Maximum = 1.0,
				Minimum = 0.0,
			};

			// Set up animation
			var animation = new XAnimationPackage(box, box2)
				.Rotate(180)
				.Then()
				.Opacity(0.2)
				.Then()
				.Translate(50, 50);

			slider.ValueChanged += (s, e)=>{
				animation.Interpolate(slider.Value);
			};

			Content = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Spacing = 14,
				Padding = 24,
				Children = {
					new StackLayout{
						Orientation = StackOrientation.Vertical,
						Spacing = 50,
						Padding = new Thickness(24, 80),
						Children = {
							box,
							box2
						}
					},

					slider,

				},
			};
		}
	}
}
