using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class NavigationContainerProvider: INavigationContainerProvider
	{
		public INavigationContainer CreateNavigationContainer(PresentationMode mode)
		{
			switch (mode)
			{
				default:
				case PresentationMode.Default:
					return new FluidNavigationContainer();
				case PresentationMode.Modal:
					return new FluidModalNavigationContainer();
				case PresentationMode.Popup:
					return new FluidPopupNavigationContainer();
			}
		}
	}
}
