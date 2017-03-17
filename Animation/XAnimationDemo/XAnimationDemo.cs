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
				Content = new Button
				{
					Text = "First",
					Command = new Command((obj) => {
						(MainPage as NavigationPage).Navigation.PushAsync(new XAnimationDemoPage2());
					})
				},
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
