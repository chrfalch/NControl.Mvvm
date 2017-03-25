using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using NControl.Mvvm.iOS;
using NControl.Controls.iOS;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;

namespace MvvmDemo.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication uiApplication, NSDictionary launchOptions)
		{
			global::Xamarin.Forms.Forms.Init ();
			LoadApplication (new DemoMvvmApp (new FluidTouchPlatform()));

			return base.FinishedLaunching (uiApplication, launchOptions);
		}
	}
}

