using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class HorizontalStackLayout : StackLayout
	{
		public HorizontalStackLayout()
		{
			Padding = 0;
			Spacing = Config.DefaultSpacing;
			Orientation = StackOrientation.Horizontal;
		}
	}

	public class HorizontalStackLayoutWithPadding : VerticalStackLayout
	{
		public HorizontalStackLayoutWithPadding()
		{
			Padding = new Thickness(Config.DefaultPadding, Config.DefaultPadding * 2);
		}
	}
}
