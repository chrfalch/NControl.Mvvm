using System;
using NControl.XAnimation;
using Xamarin.Forms;
namespace XAnimationDemo
{
	public class RotateDemoPage: ContentPage
	{
		static int pageNumber = 1;
		readonly int myPageNumber;

		~RotateDemoPage()
		{
			System.Diagnostics.Debug.WriteLine("Demo2 " + myPageNumber + " finalizing");
		}

		public RotateDemoPage()
		{
			myPageNumber = pageNumber++;
			System.Diagnostics.Debug.WriteLine("Demo2 " + myPageNumber + " starting");

			var label = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				Text = "Welcome to Xamarin Forms!"
			};
			var slider = new Slider
			{
				Minimum = 0,
				Maximum = 360,
				Value = 180,
			};

			var slidervalue = new Label
			{
				HorizontalTextAlignment = TextAlignment.Center,
				BindingContext = slider,
			};
			slidervalue.SetBinding(Label.TextProperty, nameof(Slider.Value));

			var checkbox = new Switch { HorizontalOptions = LayoutOptions.Center };
			var animation = new XAnimationPackage(label)
							.Rotate(0)
							.Translate(0, 0)
							.Set()
							.Duration(1000)
							.Rotate(slider.Value)
							.Translate(0, -60)
							.Animate();

			var animateButton = new Button
			{
				Text = "Animate",
				Command = new Command(() => {
					Action action = null;
					action = () =>
					{
						animation.Run(() =>
						{
							if (checkbox.IsToggled)
								action();
						});
					};

                   	action();
				}),
			};

			var reverseButton = new Button
			{
				Text = "Reverse",
				Command = new Command(() => animation.RunReverse())
			};

			// The root page of your application
			Title = "XAnimationDemo";
			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				Children = {
					label,
					new StackLayout { 
						Orientation = StackOrientation.Horizontal, 
						HorizontalOptions = LayoutOptions.Center,
						Children =  {animateButton, reverseButton}
					},
					slider,
					slidervalue,

					checkbox,
				}
			};
		}
	}
}
