/****************************** Module Header ******************************\
Module Name:  IPresenter.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	/// <summary>
	/// Presentation mode.
	/// </summary>
	public enum PresentationMode{
		Default,
		Modal,
		Popup
	};

	/// <summary>
	/// Presenter interface
	/// </summary>
	public interface IPresenter
	{
		/// <summary>
		/// Sets the main page.
		/// </summary>
		/// <returns>The main page.</returns>
		/// <param name="mainPage">Main page.</param>
		void SetMainPage (Page mainPage);

		/// <summary>
		/// Sets the master detail master.
		/// </summary>
		/// <param name="page">Page.</param>
		void SetMasterDetailMaster(MasterDetailPage page, bool useMasterAsNavigationPage = false);

		/// <summary>
		/// Toggles the drawer.
		/// </summary>
		void ToggleDrawer();

		// Regular navigation
		Task ShowViewModelAsync<TViewModel> (
			object parameter = null, PresentationMode presentationMode = PresentationMode.Default,  
			bool animate = true, Action<bool> dismissedCallback = null) where TViewModel : BaseViewModel;

		Task ShowViewModelAsync(
			Type viewModelType, object parameter = null, 
			PresentationMode presentationMode = PresentationMode.Default,
			bool animate = true, Action<bool> dismissedCallback = null);

		/// <summary>
		/// Dismisses the view model async.
		/// </summary>
		/// <returns>The view model async.</returns>
		Task DismissViewModelAsync(PresentationMode presentationMode, bool success = true, bool animate = true);

		// Dialog
		Task<bool> ShowMessageAsync(string title, string message, string accept = null, string cancel = null);
		Task<string> ShowActionSheet (string title, string cancel, string destruction, params string[] buttons);
	}
}

