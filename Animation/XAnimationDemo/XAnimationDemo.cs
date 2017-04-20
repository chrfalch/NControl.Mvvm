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
							Text = "First",
							Command = new Command((obj) => {
								GC.Collect();
								(MainPage as NavigationPage).Navigation.PushAsync(new XAnimationDemoPage2());
							})
						},
						//new Button
						//{
						//	Text = "Second",
						//	Command = new Command((obj) => {								
						//		(MainPage as NavigationPage).Navigation.PushAsync(new XAnimationDemoPage3());
						//	})
						//},
						new Button
						{
							Text = "Third",
							Command = new Command((obj) => {								
								(MainPage as NavigationPage).Navigation.PushAsync(new XAnimationDemoPage4());
							})
						},
						new Button{
							Text = "Performane",
							Command = new Command(()=> {
								(MainPage as NavigationPage).Navigation.PushAsync(new XAnimationDemoPage());
							})
						},

						new Button{
							Text = "Interpolate",
							Command = new Command(() => {
								(MainPage as NavigationPage).Navigation.PushAsync(new XAnimationInterpolatePage());
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
