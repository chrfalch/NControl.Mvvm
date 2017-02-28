using System;
using Xamarin.Forms;
namespace NControl.Mvvm
{
	public class VerticalSpacer: BoxView
	{
		public VerticalSpacer()
		{
			HeightRequest = MvvmApp.Current.Sizes.Get(Config.DefaultSpacing) * 2;
		}
	}

	public class LargeVerticalSpacer : BoxView
	{
		public LargeVerticalSpacer()
		{
			HeightRequest = MvvmApp.Current.Sizes.Get(Config.DefaultSpacing) * 4;
		}
	}
}
