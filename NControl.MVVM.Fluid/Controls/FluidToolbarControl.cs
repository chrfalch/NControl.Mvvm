using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidToolbarControl: ContentView
	{
		public FluidToolbarControl(View innerControl)
		{
			WidthRequest = FluidConfig.DefaultToolbarItemWidth;
			Content = innerControl;
		}
	}
}
