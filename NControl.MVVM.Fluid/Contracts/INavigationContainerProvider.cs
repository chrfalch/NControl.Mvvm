﻿using System;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public interface INavigationContainerProvider
	{
		INavigationContainer CreateNavigationContainer();
		INavigationContainer CreateModalAndPopupNavigationContainer(Size containerSize);
	}
}