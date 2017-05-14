using System;
using System.Threading.Tasks;
using NControl.XAnimation;
using Xamarin.Forms;

namespace XAnimationDemo
{
	public class CompareFormsXAnimationPage: ContentPage
	{
		public CompareFormsXAnimationPage()
		{
			var label = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Text = "Welcome to XAnimation"
			};

			var box = new ContentView
			{
				HeightRequest = 75,
				WidthRequest = 150,
				Padding = 8,
				Content = label,
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Red,
			};

			var box2 = new ContentView
			{
				HeightRequest = 75,
				WidthRequest = 150,
				Padding = 8,
				Content = label,
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Green,
			};

			var box3 = new ContentView
			{
				HeightRequest = 75,
				WidthRequest = 150,
				Padding = 8,
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Blue,
			};

			var box4 = new ContentView
			{
				HeightRequest = 75,
				WidthRequest = 150,
				Padding = 8,
				HorizontalOptions = LayoutOptions.Center,
				BackgroundColor = Color.Yellow,
			};

			var slider = new Slider
			{
				Maximum = 1.0,
				Minimum = 0.0,
			};

			var animation = new XAnimationPackage(box, box2, box3, box4);
			animation
				.Duration(1250)
			 	.Rotate(-45)
		 	.Then()
			 	.Scale(1.5f)
		 	.Then()
				 .Rotate(0)
				 .Scale(1)
		 	.Then()
			 	.Duration(200)
			 	.Translate(0, 100)
		 	.Then()
			 	.Translate(0, -100)
		 	.Then()
			 	.Reset()
		 	.Then()
		 	 	.Duration(1250)
		 	 	.Opacity(0)
			 .Then()
			 	.Reset()
			 .Then()
				.Color(Color.Red)
			 .Then()
				.Color(Color.Green)
			 .Then()
				.Color(Color.Blue);

			slider.ValueChanged += (s,e)=>animation.Interpolate(slider.Value);

			Title = "Animations";

			Content = new Grid()
			{
				BackgroundColor = Color.White,
				Children = {
					new StackLayout
					{
						Padding = 8,
						Spacing = 16,
						VerticalOptions = LayoutOptions.Center,
						Children = {

							new StackLayout{
								Children = {
									box,
									box2,
									box3,
									box4
								}
							},
							slider,
							new StackLayout{
								Spacing = 8,
								Orientation = StackOrientation.Horizontal,
								HorizontalOptions = LayoutOptions.FillAndExpand,
								Children = {

									new Button{
										Text = "Forms Anim",
										Command = new Command (async (obj) => {

											StartAnimation(box);
											StartAnimation(box2);
											StartAnimation(box3);
											await StartAnimation(box4);
										})
									},

									new Button{
										Text = "X Animation",
										Command = new Command ((obj) => {
											animation.Run(()=> System.Diagnostics.Debug.WriteLine("Animation done"));
										})
									},
								}
							},
						}
					}
				}
			};
		}

		async Task StartAnimation(View box)
		{
			await box.RotateTo(-45, 1250);
			await box.ScaleTo(1.5f, 1250);
			box.RotateTo(0, 1250);
			await box.ScaleTo(1, 1250);
			await box.TranslateTo(0, 100, 250);
			await box.TranslateTo(0, -100, 250);
			await box.TranslateTo(0, 0, 250);
			await box.FadeTo(0.0, 1250);
			await box.FadeTo(1.0, 1250);
		}
	}
}
