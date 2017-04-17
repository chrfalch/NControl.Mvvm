using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class VerticalStackLayout: StackLayout
	{
		public VerticalStackLayout()
		{
			Padding = 0;
			Spacing = Config.DefaultSpacing;
			Orientation = StackOrientation.Vertical;
		}
	}

	public class VerticalStackLayoutWithPadding : VerticalStackLayout
	{
		public VerticalStackLayoutWithPadding()
		{
			Padding = Config.DefaultPadding * 2;
		}
	}
}
