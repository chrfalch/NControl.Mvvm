using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class VerticalStackLayout: StackLayout
	{
		public VerticalStackLayout()
		{
			Padding = 0;
			// TODO: Spacing = SizeConstants.DefaultSpacing;
			Orientation = StackOrientation.Vertical;
		}
	}

	public class VerticalStackLayoutWithPadding : VerticalStackLayout
	{
		public VerticalStackLayoutWithPadding()
		{
			// TODO:// Padding = SizeConstants.DefaultPadding * 2;
		}
	}
}
