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
using NControl.XAnimation;
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

		FluidNavigationContainer _contentsContainer;
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
			_contentsContainer = new FluidNavigationContainer();
			_contentPage.Content = _contentsContainer;
			Application.Current.MainPage = _contentPage;

			// instantiate view type
			var mainViewType = (MvvmApp.Current as FluidMvvmApp).GetMainViewType();
			var mainView = Container.Resolve(mainViewType) as ContentView;
			_contentsContainer.AddChild(mainView);
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
			return PopViewModelAsync(presentationMode, success);
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

			return ShowViewModelAsync(view, dismissedCallback, presentationMode);
		}

		/// <summary>
		/// Internal show viewmodel method
		/// </summary>
		Task ShowViewModelAsync(IView view, Action<bool> dismissedCallback, PresentationMode presentationMode)
		{
			var tcs = new TaskCompletionSource<bool>();

			// Start the actual presentation of the view
			View contents = view as ContentView;
			View overlay = null;

			if (contents == null)
				throw new ArgumentException("View must inherit from ContentView.");

			// Get previous/current view
			var currentContent = _contentStack.Peek();

			if (presentationMode == PresentationMode.Default || presentationMode == PresentationMode.Modal)
			{
				// Add view itself
				_contentsContainer.AddChild(contents);
				_contentStack.Push(new NavigationElement(contents, dismissedCallback));
			}
			else
			{
				// Add overlay
				overlay = new BoxView { BackgroundColor = Color.Gray.MultiplyAlpha(0.5) };
				_contentsContainer.AddChild(overlay);

				// Add container
				var container = new RelativeLayout();
				var popupRect = new Rectangle(0, 0, _contentsContainer.Width * 0.85, _contentsContainer.Height * 0.65);

				container.Children.Add(contents, () => new Rectangle(container.Width / 2 - popupRect.Width / 2,
			  		container.Height / 2 - popupRect.Height / 2,
					popupRect.Width, popupRect.Height));

				_contentsContainer.AddChild(container);
				_contentStack.Push(new NavigationElement(container, dismissedCallback) { Overlay = overlay });

				contents = container;
			}

			if (view is IXAnimatable)
			{
				if(overlay != null)
					overlay.Opacity = 0.0;

				var animations = (view as IXAnimatable).TransitionIn(currentContent.View, overlay, presentationMode);
				if (animations != null)
				{
					var animationFinishedCounter = 0;
					foreach (var anim in animations)
					{
						anim.Run(() =>
						{
							animationFinishedCounter++;
							if (animationFinishedCounter == animations.Count())
								tcs.TrySetResult(true);
						});
					}
				}
			}
			else
			{
				// No animation, just return straight await
				tcs.TrySetResult(true);
			}

			return tcs.Task;
		}

		/// <summary>
		/// Pops the view model async.
		/// </summary>
		/// <returns>The view model async.</returns>
		Task PopViewModelAsync(PresentationMode presentationMode, bool success)
		{
			var tcs = new TaskCompletionSource<bool>();

			var contentToPop = _contentStack.Pop();
			var viewToPop = contentToPop.View;
			if (presentationMode == PresentationMode.Popup)
				viewToPop = (contentToPop.View as RelativeLayout).Children.First();
			
			var nextContent = _contentStack.Peek();

			// Function for removing when we're done.
			Action removeAction = () => 
			{
				_contentsContainer.RemoveChild(contentToPop.View);
				var viewModel = (viewToPop as IView).GetViewModel();

				if (presentationMode == PresentationMode.Popup)
					_contentsContainer.RemoveChild(contentToPop.Overlay);				
				
				viewModel.ViewModelDismissed();

				if (contentToPop.DismissedAction != null)
					contentToPop.DismissedAction(success);

				tcs.TrySetResult(true);
			};

			if (viewToPop is IXAnimatable)
			{
				var animations = (viewToPop as IXAnimatable).TransitionOut(nextContent.View, contentToPop.Overlay, presentationMode);
				if (animations != null)
				{
					var animationFinishedCounter = 0;
					foreach (var anim in animations)
					{
						anim.Run(() =>
						{
							animationFinishedCounter++;
							if (animationFinishedCounter == animations.Count())
								removeAction();
						});
					}
				}
			}
			else
			{
				// Just remove
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

	/// <summary>
	/// Navigation Element Helper 
	/// </summary>
	internal class NavigationElement
	{
		public NavigationElement(View view, Action<bool> dismissedAction)
		{
			View = view;
			DismissedAction = dismissedAction;
		}
		public View View { get; private set; }
		public View Overlay { get; set; }
		public Action<bool> DismissedAction { get; private set; }
	}
}

