using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public static class FluidConfig
	{
		public static int DefaultToolbarItemWidth { get { return MvvmApp.Current.Get(() => 30); } }
		public static int DefaultNavigationBarHeight { get { return MvvmApp.Current.Get(() => Device.OnPlatform(46, 56, 46)); } }
		public static int DefaultStatusbarHeight { get { return MvvmApp.Current.Get(() => Device.OnPlatform(22, 0, 22)); } }

		public static int DefaultPopupTitleHeight { get { return MvvmApp.Current.Get(() => 34); } }
		public static Color DefaultPopupTitleBackgroundColor { get { return MvvmApp.Current.Get(() => Config.PrimaryColor); } }
		public static Color DefaultPopupTitleColor { get { return MvvmApp.Current.Get(() => Config.NegativeTextColor); } }
		public static int DefaultPopupCornerRadius { get { return MvvmApp.Current.Get(() => 8); } }

		public static Color  DefaultShadowColor { get { return MvvmApp.Current.Get(() => Color.Black); } }
		public static int  DefaultBorderRadius { get { return MvvmApp.Current.Get(() => 8); } }
	}
}
