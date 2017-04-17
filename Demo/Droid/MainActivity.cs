using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using NControl.Mvvm.Droid;
using NControl.Controls.Droid;
using NControl.Mvvm;

namespace MvvmDemo.Droid
{
	[Activity (Label = "MvvmDemo.Droid", Icon = "@drawable/icon", MainLauncher = true, Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			PerformanceTimer.Init();
			PerformanceTimer.Current.BeginSection();

			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate (bundle);

			using(PerformanceTimer.Current.BeginTimer(this, "Call Xamarin.Forms.Init"))
				global::Xamarin.Forms.Forms.Init (this, bundle);


			using(PerformanceTimer.Current.BeginTimer(this, "Call LoadApplication"))
				LoadApplication (new DemoMvvmApp (new FluidDroidPlatform(this)));

			PerformanceTimer.Current.EndSection();

			// Results
			System.Diagnostics.Debug.WriteLine(PerformanceTimer.Current.ToString());
		}
	}
}

