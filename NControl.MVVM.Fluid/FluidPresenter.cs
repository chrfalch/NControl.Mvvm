/****************************** Module Header ******************************\
Module Name:  DefaultPresenter.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using System.Reflection;
using NControl.Mvvm.Fluid;

namespace NControl.Mvvm
{
	/// <summary>
	/// Default presenter.
	/// </summary>
	public class FluidPresenter: IPresenter
	{
		#region Private Members

		Grid _contentsContainer;
		ContentPage _contentPage;
		readonly Stack<NavigationElement> _contentStack = new Stack<NavigationElement>();

		#endregion

		#region IPresenter implementation

		/// <summary>
		/// Gets the main page.
		/// </summary>
		/// <returns>The main page.</returns>
		public void SetMainPage(Xamarin.Forms.Page page)
		{
			// Create container page
			_contentPage = new ContentPage();
			_contentsContainer = new Grid();
			_contentPage.Content = _contentsContainer;
			Application.Current.MainPage = _contentPage;

			// instantiate view type
			var mainViewType = (MvvmApp.Current as FluidMvvmApp).GetMainViewType();
			var mainView = Container.Resolve(mainViewType) as ContentView;
			_contentsContainer.Children.Add(mainView, 0, 0);
			_contentStack.Push(new NavigationElement(mainView, null));
			(mainView as IView).OnAppearing();
		}

		/// <summary>
		/// Sets the master detail master.
		/// </summary>
		/// <param name="page">Page.</param>
		public void SetMasterDetailMaster(MasterDetailPage page, bool useMasterAsNavigationStack = false)
		{
			throw new NotSupportedException("MasterDetailPage not supported by fluid presenter");
		}

		/// <summary>
		/// Toggles the drawer.
		/// </summary>
		public void ToggleDrawer()
		{
			throw new NotSupportedException("MasterDetailPage not supported by fluid presenter");
		}

		#region Dismissing ViewModels

		/// <summary>
		/// Dismisses the view model async.
		/// </summary>
		/// <returns>The view model async.</returns>
		/// <param name="presentationMode">Presentation model.</param>
		public Task DismissViewModelAsync(PresentationMode presentationMode)
		{
			return DismissViewModelAsync (presentationMode, true);
		}

		/// <summary>
		/// Dismisses the view model async.
		/// </summary>
		/// <returns>The view model async.</returns>
		/// <param name="presentationMode">Presentation mode</param>
		/// <param name="success">If set to <c>true</c> success.</param>
		public Task DismissViewModelAsync(PresentationMode presentationMode, bool success)
		{
			if (presentationMode == PresentationMode.Default)
				return PopViewModelAsync();
			
			if (presentationMode == PresentationMode.Modal)
				return PopModalViewModelAsync(success);
			
			if (presentationMode == PresentationMode.Popup)
				return PopCardViewModelAsync();

			throw new InvalidOperationException("Could not pop presentation mode " + presentationMode);
		}

		#endregion

		#region Dialogs

		/// <summary>
		/// Shows the message async.
		/// </summary>
		/// <returns>The message async.</returns>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		public async Task<bool> ShowMessageAsync(string title, string message, string accept, string cancel)
		{
			if (cancel == null) 
			{
				await _contentPage.DisplayAlert (title, message, accept ?? "OK");
				return true;
			}
			
			return await _contentPage.DisplayAlert (title, message, accept ?? "OK", cancel ?? "Cancel");
		}

		/// <summary>
		/// Shows the action sheet.
		/// </summary>
		/// <returns>The action sheet.</returns>
		/// <param name="title">Title.</param>
		/// <param name="cancel">Cancel.</param>
		/// <param name="destruction">Destruction.</param>
		/// <param name="buttons">Buttons.</param>
		public Task<string> ShowActionSheet(string title, string cancel, string destruction, params string[] buttons)
		{
			return _contentPage.DisplayActionSheet (title, cancel, destruction, buttons);
		}
		#endregion

		#region Regular Navigation

		/// <summary>
		/// Navigates to the provided view model of type
		/// </summary>
		/// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
		public Task ShowViewModelAsync<TViewModel>(object parameter = null, bool animate = true)
			where TViewModel : BaseViewModel
		{
			return ShowViewModelAsync(typeof(TViewModel), parameter, animate);
		}

		/// <summary>
		/// Navigates to the provided view model of type
		/// </summary>
		/// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
		public Task ShowViewModelAsync(Type viewModelType, object parameter = null, bool animate = true)
		{
			return ShowViewModelAsync(viewModelType, null, parameter, animate, PresentationMode.Default);
		}

		/// <summary>
		/// Shows the view model async.
		/// </summary>
		public Task ShowViewModelAsync(Type viewModelType, Action<bool> dismissedCallback = null, object parameter = null, bool animate = true, 
			PresentationMode presentationMode = PresentationMode.Default)
		{
			var view = MvvmApp.Current.ViewContainer.GetViewFromViewModel(viewModelType);

			view.GetViewModel().PresentationMode = presentationMode;

			if (parameter != null)
			{				
				var bt = viewModelType.GetTypeInfo().BaseType;
				var paramType = parameter.GetType ();
				var met = bt.GetRuntimeMethod ("InitializeAsync", new Type[]{paramType});
				if(met != null)
				{
					met.Invoke (view.GetViewModel(), new object[]{ parameter });
				}
			}

			var contents = view as ContentView;
			_contentsContainer.Children.Add(contents, 0, 0);
			_contentStack.Push(new NavigationElement(contents, dismissedCallback));

			return Task.FromResult(true);
		}

		/// <summary>
		/// Pops the view model async.
		/// </summary>
		/// <returns>The view model async.</returns>
		Task PopViewModelAsync()
		{
			var contentToPop = _contentStack.Pop();
			_contentsContainer.Children.Remove(contentToPop.View);
			(contentToPop.View as IView).GetViewModel().ViewModelDismissed();
			return Task.FromResult(true);
		}

		#endregion

		#region Modal Navigation

		/// <summary>
		/// Navigates to the provided view model of type
		/// </summary>
		/// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
		public Task ShowViewModelModalAsync<TViewModel>(
			Action<bool> dismissedCallback = null, object parameter = null, bool animate = false)
			where TViewModel : BaseViewModel
		{
			return ShowViewModelModalAsync(typeof(TViewModel), dismissedCallback, parameter, animate);
		}

		/// <summary>
		/// Shows the view model modal async.
		/// </summary>
		/// <returns>The view model modal async.</returns>
		public Task ShowViewModelModalAsync(Type viewModelType,
			Action<bool> dismissedCallback = null, object parameter = null, bool animate = false)
		{
			return ShowViewModelAsync(viewModelType, dismissedCallback, parameter, animate, PresentationMode.Modal);
		}

		/// <summary>
		/// Pops the active modal view 
		/// </summary>
		/// <returns>The modal view model async.</returns>
		public Task PopModalViewModelAsync(bool success)
		{
			var contentToPop = _contentStack.Pop();
			_contentsContainer.Children.Remove(contentToPop.View);
			if (contentToPop.DismissedAction != null)
				contentToPop.DismissedAction(success);

			(contentToPop.View as IView).GetViewModel().ViewModelDismissed();
			
			return Task.FromResult(true);
		}

		#endregion

		#region Card Navigation

		/// <summary>
		/// Shows the view model as popup async.
		/// </summary>
		/// <returns>The view model as popup async.</returns>
		/// <param name="parameter">Parameter.</param>
		public Task ShowViewModelAsPopupAsync<TViewModel>(object parameter)
			where TViewModel : BaseViewModel
		{
			return ShowViewModelAsPopupAsync(typeof(TViewModel), parameter);
		}

		/// <summary>
		/// Navigates to card view model async.
		/// </summary>
		/// <returns>The to card view model async.</returns>
		/// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
		public Task ShowViewModelAsPopupAsync(Type viewModelType, object parameter)			
		{
			return ShowViewModelAsync(viewModelType, null, parameter, true, PresentationMode.Popup);
		}

		/// <summary>
		/// Pops the card view model async.
		/// </summary>
		/// <returns>The card view model async.</returns>
		public Task PopCardViewModelAsync()
		{
			var contentToPop = _contentStack.Pop();
			_contentsContainer.Children.Remove(contentToPop.View);
			(contentToPop.View as IView).GetViewModel().ViewModelDismissed();
			return Task.FromResult(true);
		}
		#endregion

		#endregion
	}

	/// <summary>
	/// Navigation Element Helper 
	/// </summary>
	internal class NavigationElement
	{
		public NavigationElement(ContentView view, Action<bool> dismissedAction)
		{
			View = view;
			DismissedAction = dismissedAction;
		}
		public ContentView View { get; set; }
		public Action<bool> DismissedAction { get; set; }
	}
}

