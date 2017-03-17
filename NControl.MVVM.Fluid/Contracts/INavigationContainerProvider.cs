using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface INavigationContainerProvider
	{
		INavigationContainer CreateNavigationContainer(PresentationMode mode, Size pageSize);
	}
}
