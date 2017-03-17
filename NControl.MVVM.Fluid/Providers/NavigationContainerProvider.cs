using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class NavigationContainerProvider: INavigationContainerProvider
	{
		public INavigationContainer CreateNavigationContainer(PresentationMode mode, Size pageSize)
		{
			switch (mode)
			{
				default:
				case PresentationMode.Default:
					return new FluidNavigationContainer();
				case PresentationMode.Modal:
					return new FluidModalNavigationContainer();
				case PresentationMode.Popup:
					return new FluidPopupNavigationContainer
					{
						ContentSize = new Size(pageSize.Width * 0.8, pageSize.Height * 0.7)
					};
			}
		}
	}
}
