using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public static class Config
	{
		#region Size Constants

		public static int DefaultPadding { get { return MvvmApp.Current.Get(()=> 8); } }
		public static int DefaultSpacing { get { return MvvmApp.Current.Get(()=> 8); } }
		public static double DefaultBorderSize { get { return MvvmApp.Current.Get(()=> 0.5 * MvvmApp.Current.Environment.DisplayDensity); } }
		public static double DefaultBoldBorderSize { get { return MvvmApp.Current.Get(()=> 1.0 * MvvmApp.Current.Environment.DisplayDensity); } }
		public static int DefaultLargeSpacing { get { return MvvmApp.Current.Get(()=> 14); } }
		public static int DefaultLargePadding { get { return MvvmApp.Current.Get(()=> 24); } }
		public static int DefaultActivityIndicatorSize { get { return MvvmApp.Current.Get(()=> 44); } }

		#endregion

		#region Color Constants

		public static Color PrimaryColor { get { return MvvmApp.Current.Get(()=> Color.FromHex("#2196F3")); } }
		public static Color PrimaryDarkColor { get { return MvvmApp.Current.Get(()=> Color.FromHex("#1976D2")); } }
		public static Color AccentColor { get { return MvvmApp.Current.Get(()=> Color.Accent); } }
		public static Color TextColor { get { return MvvmApp.Current.Get(()=> Color.Black); } }
		public static Color NegativeTextColor { get { return MvvmApp.Current.Get(()=> Color.White); } }
		public static Color LightTextColor { get { return MvvmApp.Current.Get(()=> Color.FromHex("CECECE")); } }
		public static Color AccentTextColor { get { return MvvmApp.Current.Get(()=> Color.Accent); } }
		public static Color BorderColor { get { return MvvmApp.Current.Get(()=> Color.FromHex("CECECE")); } }
		public static Color ViewBackgroundColor { get { return MvvmApp.Current.Get(()=> Color.FromHex("FEFEFE")); } }
		public static Color ViewTransparentBackgroundColor { get { return MvvmApp.Current.Get(()=> Color.Black.MultiplyAlpha(0.75)); } }

		#endregion
	}
}
