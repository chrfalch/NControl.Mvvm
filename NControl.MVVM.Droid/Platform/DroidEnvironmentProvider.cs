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

		public Rectangle GetLocationOnScreen(VisualElement element)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
				return Rectangle.Zero;

			var nativeView = renderer.ViewGroup;
			var rect = new int[2];
			nativeView.GetLocationOnScreen(rect);
			return new Rectangle(
				Forms.Context.FromPixels(rect[0]), Forms.Context.FromPixels(rect[1]), 
				Forms.Context.FromPixels(nativeView.Width), Forms.Context.FromPixels(nativeView.Height));
		}
	}
}
