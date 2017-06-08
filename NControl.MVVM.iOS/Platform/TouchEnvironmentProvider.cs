using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace NControl.Mvvm.iOS
{
	public class TouchEnvironmentProvider: IEnvironmentProvider
	{
		public TouchEnvironmentProvider()
		{
		}

		public string AppVersion
		{
			get
			{
				return NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleShortVersionString")] +
					"." + NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersion")];
			}
		}

		public float DisplayDensity
		{
			get
			{
				return 1.0f;
			}
		}
	}
}
