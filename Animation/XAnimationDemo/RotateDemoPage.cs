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
				Text = "Welcome to Xamarin Forms!",
				// Rotation = 45,
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
			XAnimationPackage animation = null;
			var animateButton = new Button
			{
				Text = "Animate",
				Command = new Command(() => {
					Action action = null;
					action = () =>
					{
						animation = new XAnimationPackage(label);
						animation.Add()
							.SetDuration(1000)
							.SetTranslation(0, 50);

						animation.Add()
							.SetDuration(1000)							
							.SetTranslation(0, 100);
						
						animation.Animate(() =>
						{
							if (checkbox.IsToggled)
								action();
							
						}, 4000, EasingFunctions.Custom(1, 0, 1, 0));
					};

                   	action();
				}),
			};

			var reverseButton = new Button
			{
				Text = "Reverse",
				Command = new Command(() =>
				{
					if (animation != null)
						animation.AnimateReverse();
				})
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
