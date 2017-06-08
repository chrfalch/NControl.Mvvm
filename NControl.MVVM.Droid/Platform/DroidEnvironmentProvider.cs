using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NControl.Mvvm.Droid
{
	public class DroidEnvironmentProvider: IEnvironmentProvider
	{
		float _displayDensity = float.MinValue;
		string _appVersion = null;

		public string AppVersion
		{
			get
			{
				if (string.IsNullOrEmpty(_appVersion))
				{
					_appVersion = Forms.Context.PackageManager.GetPackageInfo(
						Forms.Context.PackageName, 0).VersionName;
				}

				return _appVersion;
			}
		}

		public float DisplayDensity
		{
			get
			{
				if (_displayDensity.Equals(float.MinValue))
				{
					// Get display density to fix pixel scaling
					using (var metrics = Forms.Context.Resources.DisplayMetrics)
						_displayDensity = metrics.Density;
				}

				return _displayDensity;
			}
		}
	}
}
