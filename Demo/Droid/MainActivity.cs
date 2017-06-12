using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using NControl.Mvvm;

namespace MvvmDemo.Droid
{
	[Activity(Label = "MvvmDemo.Droid", Icon = "@drawable/icon", MainLauncher = true, Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			PerformanceTimer.Init();

			using (PerformanceTimer.Current.BeginTimer(this))
			{

				base.OnCreate(savedInstanceState);

				using (PerformanceTimer.Current.BeginTimer(this, "Call Xamarin.Forms.Init"))
					Xamarin.Forms.Forms.Init(this, savedInstanceState);

				using (PerformanceTimer.Current.BeginTimer(this, "Call LoadApplication"))
					LoadApplication(new DemoMvvmApp(new DemoFluidDroidPlatform(this)));
			}

			// Results
			Console.WriteLine(PerformanceTimer.Current.ToString());
		}
	}
}

