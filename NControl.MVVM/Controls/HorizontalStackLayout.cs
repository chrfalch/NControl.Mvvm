using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class HorizontalStackLayout : StackLayout
	{
		public HorizontalStackLayout()
		{
			Padding = 0;
			Spacing = MvvmApp.Current.Sizes.Get(Config.DefaultSpacing);
			Orientation = StackOrientation.Horizontal;
		}
	}

	public class HorizontalStackLayoutWithPadding : VerticalStackLayout
	{
		public HorizontalStackLayoutWithPadding()
		{
			Padding = new Thickness(MvvmApp.Current.Sizes.Get(Config.DefaultPadding), 
			                        MvvmApp.Current.Sizes.Get(Config.DefaultPadding) * 2);
		}
	}
}
