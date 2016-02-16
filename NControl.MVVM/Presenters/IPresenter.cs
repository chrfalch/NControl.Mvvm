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

namespace NControl.MVVM
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
		Page SetMainPage (Page mainPage);

		/// <summary>
		/// Sets the master detail master.
		/// </summary>
		/// <param name="page">Page.</param>
		void SetMasterDetailMaster(MasterDetailPage page);

		/// <summary>
		/// Toggles the drawer.
		/// </summary>
		void ToggleDrawer();

		// Regular navigation
		Task ShowViewModelAsync<TViewModel> () where TViewModel : BaseViewModel;
		Task ShowViewModelAsync<TViewModel> (bool animate) where TViewModel : BaseViewModel;
		Task ShowViewModelAsync<TViewModel> (object parameter) where TViewModel : BaseViewModel;
		Task ShowViewModelAsync<TViewModel> (object parameter, bool animate) where TViewModel : BaseViewModel;

		// Card navigation
		Task ShowViewModelAsPopupAsync<TViewModel>() where TViewModel : BaseViewModel;
		Task ShowViewModelAsPopupAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel;

		// Modal navigation
		Task<NavigationPage> ShowViewModelModalAsync<TViewModel> (object parameter) where TViewModel : BaseViewModel;
		Task<NavigationPage> ShowViewModelModalAsync<TViewModel> (Action<bool> dismissedCallback, object parameter) where TViewModel : BaseViewModel;
		Task<NavigationPage> ShowViewModelModalAsync<TViewModel> (Action<bool> dismissedCallback) where TViewModel : BaseViewModel;
		Task<NavigationPage> ShowViewModelModalAsync<TViewModel> () where TViewModel : BaseViewModel;

		/// <summary>
		/// Dismisses the view model async.
		/// </summary>
		/// <returns>The view model async.</returns>
		Task DismissViewModelAsync(PresentationMode presentationMode);
		Task DismissViewModelAsync(PresentationMode presentationMode, bool success);

	}
}

