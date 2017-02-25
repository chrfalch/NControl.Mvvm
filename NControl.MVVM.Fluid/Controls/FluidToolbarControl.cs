using System;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public class FluidToolbarControl: ContentView
	{
		public FluidToolbarControl(View innerControl)
		{
			Content = innerControl;
			WidthRequest = 26;
		}
	}
}
