/****************************** Module Header ******************************\
Module Name:  IView.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;

namespace NControl.Mvvm
{
	/// <summary>
	/// I view.
	/// </summary>
	public interface IView<TViewModel>: IView where TViewModel: BaseViewModel, IViewModel
	{
		/// <summary>
		/// Gets the view model.
		/// </summary>
		/// <value>The view model.</value>
		TViewModel ViewModel { get; }
	}

	/// <summary>
	/// I view.
	/// </summary>
	public interface IView
	{
		IViewModel GetViewModel();
		void OnAppearing();
		void OnDisappearing();

	}
}

