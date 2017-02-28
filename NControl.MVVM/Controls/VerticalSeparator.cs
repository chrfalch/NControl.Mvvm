using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class VerticalSeparator : BoxView
	{
		public VerticalSeparator()
		{			
			HeightRequest = MvvmApp.Current.Sizes.Get(Config.DefaultBorderSize);
			Color = MvvmApp.Current.Colors.Get(Config.BorderColor);
		}
	}

	public class VerticalLightSeparator : BoxView
	{
		public VerticalLightSeparator()
		{			
			HeightRequest = MvvmApp.Current.Sizes.Get(Config.DefaultBorderSize) * 0.5;
			Color = MvvmApp.Current.Colors.Get(Config.BorderColor);
		}
	}
}
