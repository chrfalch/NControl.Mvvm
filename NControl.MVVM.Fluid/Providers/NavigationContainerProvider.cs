using System;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public class NavigationContainerProvider: INavigationContainerProvider
	{
		public INavigationContainer CreateModalAndPopupNavigationContainer(Size containerSize)
		{
			return new FluidModalContainer
			{
				ContentSize = containerSize
			};
		}

		public INavigationContainer CreateNavigationContainer()
		{
			return new FluidNavigationContainer();
		}
	}
}
