using System;
using NControl.XAnimation;
using Xamarin.Forms;
namespace XAnimationDemo
{
	public class XAnimationDemoPage2: ContentPage
	{
		static int pageNumber = 1;
		readonly int myPageNumber;

		~XAnimationDemoPage2()
		{
			System.Diagnostics.Debug.WriteLine("Demo2 " + myPageNumber + " finalizing");
		}

		public XAnimationDemoPage2()
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

			var button = new Button
			{
				Text = "Animate!",
				Command = new Command(() =>
				{

					Action action = null;
					action = () =>
					{
						new XAnimationPackage(label)
							.Rotate(0)
							.Translate(0, 0)
							.Set()
							.Duration(1000)
							//.Color(Color.Red)
							//.Rotate(slider.Value)
							//.Easing(.07, .62, .58, 1.51)
							//.Animate()
							//.Color(Color.Transparent)
							.Rotate(180)
							.Translate(0, -60)
							.Animate()
							.Run(() =>
							{
								if (checkbox.IsToggled) action();
							});
					};

					action();
				}),
			};

			// The root page of your application
			Title = "XAnimationDemo";
			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				Children = {
					label,
					button,
					slider,
					slidervalue,

					checkbox,
				}
			};
		}
	}
}
