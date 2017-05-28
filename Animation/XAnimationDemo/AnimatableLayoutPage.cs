using System;
using NControl.XAnimation;
using Xamarin.Forms;

namespace XAnimationDemo
{
	public class AnimatableLayoutPage: ContentPage
	{
		public AnimatableLayoutPage()
		{
			var slider = new Slider
			{
				Minimum = 0.0,
				Maximum = 1.0,
				Value = 0.0,
				Margin = 20,
			};

			var layout = new XAnimatableLayout { 
				Margin = 24,	
				BackgroundColor = Color.Yellow,
				BindingContext = slider,
			};

			layout.SetBinding(XAnimatableLayout.InterpolationProperty, nameof(slider.Value));

            layout.Children.Add(new ContentView
			{
				BackgroundColor = Color.Gainsboro,
				Content =
				new BoxView { BackgroundColor = Color.Red, Margin = 10 }
				
			}, (transformPackage, initialTransform, theLayout) =>
            {
				initialTransform.SetRectangle(0, 0, 60, 40);
				transformPackage.Add().SetRectangle(0, 0, theLayout.Width, 150);
            });

			layout.Children.Add(new ContentView
			{
				BackgroundColor = Color.Silver,
				Content =
					new Label
					{
						Text = "Hello world!",
						LineBreakMode = LineBreakMode.MiddleTruncation,
						BackgroundColor = Color.Tan,
					}

			}, (transformPackage, initialTransform, theLayout) =>
            {
				initialTransform.SetRectangle(70, 0, theLayout.Width - 70, 20);
				transformPackage.Add().SetRectangle(0, 160, theLayout.Width, 60);
            });

			layout.Children.Add(
				new BoxView { BackgroundColor = Color.Silver }, 
				(transformPackage, initialTransform, theLayout) =>
            {
				initialTransform.SetRectangle(70, 30, theLayout.Width - 70, 10);
				transformPackage.Add().SetRectangle(0, 210, theLayout.Width, 0);
            });

			Content = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Children = {
					new BoxView {VerticalOptions= LayoutOptions.FillAndExpand, BackgroundColor = Color.Lime},
					layout,
					new BoxView {VerticalOptions= LayoutOptions.FillAndExpand, BackgroundColor = Color.Aqua},
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.Center,
						Children = {
							new Button{
								Text = "Animate",
								Command = new Command(_=> layout.Animate()),
							},
							new BoxView{WidthRequest = 50},
							new Button{
								Text = "Reverse",
								Command = new Command(_=> layout.Reverse()),
							}
						}
					},
					slider,
				},
			};
		}
	}
}
