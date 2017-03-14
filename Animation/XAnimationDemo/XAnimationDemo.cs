using System;
using NControl.XAnimation;
using Xamarin.Forms;

namespace XAnimationDemo
{
	public class App : Application
	{
		public App()
		{
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

			var button = new Button
			{
				Text = "Animate!",
				Command = new Command(() => {

					Action action = null;
					action = () =>
					{
						new XAnimationPackage(label)
							.Duration(1000)
							.Color(Color.Red)
							.Rotate(slider.Value)
							.Easing(EasingFunction.EaseInOut)
							.Animate()
							.Color(Color.Transparent)
							.Animate()
							.Run();
					};

					action();
				}),
			};

			// The root page of your application
			ContentPage contentPage = null;
			contentPage = new ContentPage
			{
				Title = "XAnimationDemo",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children = {
						label,
						button,
						slider,
						slidervalue,
						new Button{
							Text = "More...",
							Command = new Command(()=> {
								contentPage.Navigation.PushAsync(new XAnimationDemoPage());
							})
						}
					}
				}
			};

			MainPage = new NavigationPage(contentPage);
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
