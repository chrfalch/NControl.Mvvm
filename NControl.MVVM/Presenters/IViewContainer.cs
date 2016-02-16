/****************************** Module Header ******************************\
Module Name:  IViewContainer.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	/// <summary>
	/// Container for views
	/// </summary>
	public interface IViewContainer
	{
		/// <summary>
		/// Registers the view.
		/// </summary>
		/// <typeparam name="TViewModelType">The 1st type parameter.</typeparam>
		/// <typeparam name="TViewType">The 2nd type parameter.</typeparam>
		void RegisterView<TViewModelType, TViewType> ()
			where TViewModelType : BaseViewModel
			where TViewType : Page;

		/// <summary>
		/// Gets the view from view model.
		/// </summary>
		/// <returns>The view from view model.</returns>
		/// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
		Page GetViewFromViewModel<TViewModel>() where TViewModel : BaseViewModel;
	}
}

