using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class VerticalStackLayout: StackLayout
	{
		public VerticalStackLayout()
		{
			Padding = 0;
			Spacing = MvvmApp.Current.Sizes.Get(Config.DefaultSpacing);
			Orientation = StackOrientation.Vertical;
		}
	}

	public class VerticalStackLayoutWithPadding : VerticalStackLayout
	{
		public VerticalStackLayoutWithPadding()
		{
			Padding = MvvmApp.Current.Sizes.Get(Config.DefaultPadding) * 2;
		}
	}
}
