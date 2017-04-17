using System;
using Xamarin.Forms;
namespace NControl.Mvvm
{
	public class VerticalSpacer: BoxView
	{
		public VerticalSpacer()
		{
			HeightRequest = Config.DefaultSpacing * 2;
		}
	}

	public class LargeVerticalSpacer : BoxView
	{
		public LargeVerticalSpacer()
		{
			HeightRequest = Config.DefaultSpacing * 4;
		}
	}
}
