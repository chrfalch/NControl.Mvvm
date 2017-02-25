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
		readonly Stack<NavigationContext> _contextStack = new Stack<NavigationContext>();

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

			// Create container
			var container = new FluidNavigationContainer();
			container.AddChild(mainView, PresentationMode.Default);

			// Add to container
			_contentsContainer.Children.Add(container, 0, 0);
			_contextStack.Push(new NavigationContext(container, null));
			_contextStack.Peek().NavigationStack.Push(mainView);

			// Notify
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
			return PopViewModelAsync(presentationMode, success, true);
		}

		/// <summary>
		/// Dismiss the view model async.
		/// </summary>
		public Task DismissViewModelAsync(PresentationMode presentationMode, bool success, bool animate)
		{
			return PopViewModelAsync(presentationMode, success, animate);
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
				var paramType = parameter.GetType();
				var met = bt.GetRuntimeMethod("InitializeAsync", new Type[] { paramType });
				if (met != null)
				{
					met.Invoke(view.GetViewModel(), new object[] { parameter });
				}
			}

			return ShowViewModelAsync(view, dismissedCallback, presentationMode, animate);
		}

		/// <summary>
		/// Internal show viewmodel method
		/// </summary>
		Task ShowViewModelAsync(IView view, Action<bool> dismissedCallback, 
		                        PresentationMode presentationMode, bool animate)
		{
			var tcs = new TaskCompletionSource<bool>();

			// Start the actual presentation of the view
			var contents = view as View;
			if (contents == null)
				throw new ArgumentException("View must inherit from Xamarin.Forms.View.");

			if (presentationMode == PresentationMode.Default)
			{
				// Get previous/current view
				var currentContext = _contextStack.Peek();

				// Add view 
				currentContext.Container.AddChild(contents, presentationMode);
				currentContext.NavigationStack.Push(contents);

				if (animate && currentContext.Container is IXAnimatable)
				{
					var animations = (currentContext.Container as IXAnimatable).TransitionIn(
						contents, presentationMode);

					XAnimation.XAnimation.RunAll(animations, () => tcs.TrySetResult(true));
				}
				else
				{
					// No animation, just return straight await
					tcs.TrySetResult(true);
				}

			}
			else if (presentationMode == PresentationMode.Modal || 
			         presentationMode == PresentationMode.Popup)
			{
				// Container and navigation context
				View container = null;

				if (presentationMode == PresentationMode.Modal)
				{
					container = new FluidNavigationContainer();
				}
				else
				{
					container = new FluidModalContainer { 
						ContentSize = new Size(
							_contentsContainer.Width * 0.8, _contentsContainer.Height * 0.7)
					};
				}

				var navigationContainer = container as INavigationContainer;
				if (navigationContainer == null)
					throw new InvalidOperationException("Need a INavigationContainer when " +
														"showing modal or as popup.");
				
				navigationContainer.AddChild(contents, presentationMode);

				// Add to container
				_contentsContainer.Children.Add(container, 0, 0);
				_contextStack.Push(new NavigationContext(navigationContainer, dismissedCallback));
				_contextStack.Peek().NavigationStack.Push(contents);

				if (animate && container is IXAnimatable)
				{
					var animations = (container as IXAnimatable).TransitionIn(
						container, presentationMode);

					XAnimation.XAnimation.RunAll(animations, () => tcs.TrySetResult(true));
				}
				else
				{
					// No animation, just return straight await
					tcs.TrySetResult(true);
				}
			}

			return tcs.Task;
		}

		/// <summary>
		/// Pops the view model async.
		/// </summary>
		/// <returns>The view model async.</returns>
		Task PopViewModelAsync(PresentationMode presentationMode, bool success, bool animate)
		{
			var tcs = new TaskCompletionSource<bool>();

			if (presentationMode == PresentationMode.Default)
			{
				var currentContext = _contextStack.Peek();
				var view = currentContext.NavigationStack.FirstOrDefault();

				// Set up action to run when all transitions and animations
				// are done
				Action removeAction = () =>
				{
					var viewModelProvider = view as IView;
					if (viewModelProvider != null)
						viewModelProvider.GetViewModel().ViewModelDismissed();
					
					currentContext.Container.RemoveChild(view, presentationMode);
					currentContext.NavigationStack.Pop();
					tcs.TrySetResult(true);
				};

				// Should we animate?
				if (animate && currentContext.Container is IXAnimatable)
					XAnimation.XAnimation.RunAll(
						(currentContext.Container as IXAnimatable).TransitionOut(
						view, presentationMode), removeAction);				
				else
					removeAction();				
			}
			else if (presentationMode == PresentationMode.Modal ||
			         presentationMode == PresentationMode.Popup)
			{
				// Get current context (modal)
				var currentContext = _contextStack.Pop();

				Action removeAction = () =>
				{
					// Call dismiss on all view models
					foreach (var view in currentContext.NavigationStack.Reverse())
					{
						var viewModelProvider = view as IView;
						if (viewModelProvider != null)
							viewModelProvider.GetViewModel().ViewModelDismissed();
					}

					// Clear navigation stack
					currentContext.NavigationStack.Clear();

					// Remove from view stack
					_contentsContainer.Children.Remove(currentContext.Container as View);

					// Call dismissed action
					if (currentContext.DismissedAction != null)
						currentContext.DismissedAction(success);
					
					tcs.TrySetResult(true);
				};

				if (animate && currentContext.Container is IXAnimatable)
					XAnimation.XAnimation.RunAll(
						(currentContext.Container as IXAnimatable).TransitionOut(
							currentContext.Container as View, presentationMode), removeAction);
				else
					removeAction();
			}
					
			return tcs.Task;
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

		#endregion

		#endregion
	}

	public class NavigationContext
	{
		public Stack<View> NavigationStack { get; private set; }
		public Action<bool> DismissedAction { get; private set; }
		public INavigationContainer Container { get; private set; }

		public NavigationContext (INavigationContainer container, 
		                         Action<bool> dismissedAction)
		{
			Container = container;
			NavigationStack = new Stack<View>();
			DismissedAction = dismissedAction;
		}
	}
}

