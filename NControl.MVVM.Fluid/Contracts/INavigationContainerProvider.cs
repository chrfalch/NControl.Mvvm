using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface INavigationContainerProvider
	{
		INavigationContainer CreateNavigationContainer();
		INavigationContainer CreateModalNavigationContainer();
		INavigationContainer CreatePopupNavigationContainer(Size containerSize);
	}
}
