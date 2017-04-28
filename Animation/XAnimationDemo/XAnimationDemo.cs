using System;
using NControl.XAnimation;
using Xamarin.Forms;

namespace XAnimationDemo
{
	public class App : Application
	{
		public App()
		{
			MainPage = new NavigationPage(new ContentPage
			{
				Content = new StackLayout
				{
					Children = {
						new Button
						{
							Text = "Animation",
							Command = new Command((obj) => {
								GC.Collect();
								(MainPage as NavigationPage).Navigation.PushAsync(new RotateDemoPage());
							})
						},
						new Button
						{
							Text = "Animate Layout",
							Command = new Command((obj) => {								
								(MainPage as NavigationPage).Navigation.PushAsync(new AnimatableLayoutPage());
							})
						},

						new Button{
							Text = "Compare",
							Command = new Command(()=> {
								(MainPage as NavigationPage).Navigation.PushAsync(new CompareFormsXAnimationPage());
							})
						},

						new Button{
							Text = "Interpolate",
							Command = new Command(() => {
								(MainPage as NavigationPage).Navigation.PushAsync(new InterpolatePage());
							})
						},


					},
				}
			});
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
