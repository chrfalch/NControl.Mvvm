using System;
using Foundation;

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
