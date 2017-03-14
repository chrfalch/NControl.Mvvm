using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class NavigationContainerProvider: INavigationContainerProvider
	{
		public INavigationContainer CreateModalNavigationContainer()
		{
			return new FluidModalNavigationContainer();
		}

		public INavigationContainer CreateNavigationContainer()
		{
			return new FluidNavigationContainer();	
		}

		public INavigationContainer CreatePopupNavigationContainer(Size containerSize)
		{
			return new FluidPopupNavigationContainer
			{
				ContentSize = containerSize
			};
		}
	}
}
