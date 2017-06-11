using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using NControl.Mvvm.iOS;
using NControl.Controls.iOS;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using NControl.Mvvm;
using Lottie.Forms.iOS.Renderers;

namespace MvvmDemo.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication uiApplication, NSDictionary launchOptions)
		{
			PerformanceTimer.Init();

			AnimationViewRenderer.Init();

			using (PerformanceTimer.Current.BeginTimer(this))
			{
				global::Xamarin.Forms.Forms.Init();
				LoadApplication(new DemoMvvmApp(new DemoFluidTouchPlatform()));
			}

			var retVal = base.FinishedLaunching(uiApplication, launchOptions);

			// Results
			Console.WriteLine(PerformanceTimer.Current.ToString());

			return retVal;
		}
	}
}

