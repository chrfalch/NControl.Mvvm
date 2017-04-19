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
using NControl.Mvvm;
using NControl.XAnimation;

namespace NControl.Mvvm
{
	/// <summary>
	/// Default presenter.
	/// </summary>
	public class FluidPresenter: IPresenter, IActivityIndicatorViewProvider
	{
		#region Private Members

		// Container page
		FluidContainerPage _contentPage;

		// Content container provider
		readonly INavigationContainerProvider _navigationContainerProvider;

		#endregion

		/// <summary>
		/// Initializes a new instance of the FluidPresenter class.
		/// </summary>
		public FluidPresenter(INavigationContainerProvider navigationContainerProvider)
		{
			_navigationContainerProvider = navigationContainerProvider;
		}

		#region IPresenter implementation

		/// <summary>
		/// Sets the main page of the application
		/// </summary>
		public void SetMainPage(Page page)
		{
			using (PerformanceTimer.Current.BeginTimer(this))
			{
				using (PerformanceTimer.Current.BeginTimer(this, "Creating container page"))
				{
					// Create container page
					_contentPage = new FluidContainerPage();
					Application.Current.MainPage = _contentPage;
				}

				// instantiate view type
				Type mainViewType = null;
				using (PerformanceTimer.Current.BeginTimer(this, "Getting Main View Type"))
					mainViewType = (MvvmApp.Current as FluidMvvmApp).GetMainViewType();

				ContentView mainView = null;
				using (PerformanceTimer.Current.BeginTimer(this, "Getting Main View"))
					mainView = Container.Resolve(mainViewType) as ContentView;

				using (PerformanceTimer.Current.BeginTimer(this, "Presenting Main View"))
					PresentView(mainView, PresentationMode.Default, (b) =>
					{
						throw new InvalidOperationException("Should not dismiss main view/viewmodel!");
					});
			}
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

		#region Navigation

		/// <summary>
		/// Navigates to the provided view model of type
		/// </summary>
		/// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
		public Task ShowViewModelAsync<TViewModel>(
			PresentationMode presentationMode = PresentationMode.Default, Action<bool> dismissedCallback = null,
			bool animate = true, object parameter = null) where TViewModel : BaseViewModel
		{
			return ShowViewModelAsync(typeof(TViewModel), presentationMode, dismissedCallback, animate, parameter);
		}

		/// <summary>
		/// Shows the view model async.
		/// </summary>
		/// <returns>The view model async.</returns>
		/// <param name="viewModelType">View model type.</param>
		/// <param name="presentationMode">Presentation mode.</param>
		/// <param name="parameter">Parameter.</param>
		/// <param name="animate">If set to <c>true</c> animate.</param>
		/// <param name="dismissedCallback">Dismissed callback.</param>
		public Task ShowViewModelAsync(Type viewModelType,
			PresentationMode presentationMode = PresentationMode.Default, Action<bool> dismissedCallback = null,
			bool animate = true, object parameter = null)
		{
			// Get view from viewmodel-mapping
			var view = MvvmApp.Current.ViewContainer.GetViewFromViewModel(viewModelType);
			view.GetViewModel().PresentationMode = presentationMode;

			// Should we use a parameter?
			if (parameter != null)
			{
				// TODO: Accept null values as parameters?
				var bt = viewModelType.GetTypeInfo().BaseType;
				var paramType = parameter.GetType();
				var met = bt.GetRuntimeMethod("InitializeAsync", new Type[] { paramType });
				if (met != null)
				{
					met.Invoke(view.GetViewModel(), new object[] { parameter });
				}
			}

			// Present the view itself
			return PresentViewAsync(view, dismissedCallback, presentationMode, animate);
		}

		/// <summary>
		/// Internal show viewmodel method
		/// </summary>
		Task PresentViewAsync(IView view, Action<bool> dismissedCallback, PresentationMode presentationMode, bool animate)
		{
			var tcs = new TaskCompletionSource<bool>();

			// Start the actual presentation of the view
			var newView = view as View;
			if (newView == null)
				throw new ArgumentException("View must inherit from Xamarin.Forms.View.");

			// Get previous container
			var fromContainer = _contentPage.CurrentContext.Elements.Peek().Container;

			// Present the view
			var navigationElement = PresentView(newView, presentationMode, dismissedCallback);

			// Animate or present regularly?
			if (animate && navigationElement.Container is IXAnimatable)
			{
				var animations = (navigationElement.Container as IXAnimatable).TransitionIn(
					fromContainer, presentationMode);
				
				XAnimationPackage.RunAll(animations, () => {
					// Notify
					view.OnAppearing();
					tcs.TrySetResult(true);
				});
			}
			else
			{
				// No animation, just return straight await
				view.OnAppearing();
				tcs.TrySetResult(true);
			}

			return tcs.Task;
		}

		/// <summary>
		/// Presents a view with a given presentation mode.
		/// </summary>
		NavigationElement PresentView(View view, PresentationMode presentationMode, Action<bool> dismissedAction)
		{
			// Create container
			var container = _navigationContainerProvider.CreateNavigationContainer(
				presentationMode);

			// Add contents view to container's content area
			container.SetContent(view);
			_contentPage.Container.Children.Add(container.GetBaseView(), 0, 0);

			// Create navigation element
			var navigationElement = new NavigationElement(view, container, dismissedAction);

			// Notify view about view lifecycle events
			(view as IView).OnAppearing();

			// Add to or create context depending on type of navigation
			if (presentationMode == PresentationMode.Default && _contentPage.Contexts.Count > 0)
			{
				container.NavigationContext = _contentPage.CurrentContext;
				_contentPage.CurrentContext.Elements.Push(navigationElement);
			}
			else
			{
				container.NavigationContext = new NavigationContext(navigationElement);
				_contentPage.Contexts.Push(container.NavigationContext);
			}

			return navigationElement;
		}

		/// <summary>
		/// Pops the view model async.
		/// </summary>
		/// <returns>The view model async.</returns>
		Task PopViewModelAsync(PresentationMode presentationMode, bool success, bool animate)
		{
			var tcs = new TaskCompletionSource<bool>();

			// Get previous container
			var fromElement = _contentPage.CurrentContext.Elements.Pop();
			NavigationElement toElement = null;
			if (_contentPage.CurrentContext.Elements.Count == 0)
			{
				// Pop the context, we're on the last one
				_contentPage.Contexts.Pop();
				toElement = _contentPage.CurrentContext.Elements.Peek();
			}
			else
			{
				toElement = _contentPage.CurrentContext.Elements.Peek();
			}

			// Set up action to run when all transitions and animations
			// are done
			Action removeAction = () =>
			{
				var viewModelProvider = fromElement.View as IView;
				if (viewModelProvider == null)
					throw new ArgumentException("View must implement IView");
				
				viewModelProvider.OnDisappearing();
				viewModelProvider.GetViewModel().ViewModelDismissed();
		
				// Call dismissed action
				fromElement.DismissedAction?.Invoke(success);

				// Remove from view hierarchy
				_contentPage.Container.Children.Remove(fromElement.Container.GetBaseView());
				fromElement.View = null;fromElement.Container = null;fromElement.DismissedAction = null;

				tcs.TrySetResult(true);
			};

			// Should we animate?
			if (animate && fromElement.Container is IXAnimatable)
				XAnimationPackage.RunAll(
					(fromElement.Container as IXAnimatable).TransitionOut(toElement.Container,
					presentationMode), removeAction);
			else
				removeAction();		
					
			return tcs.Task;
		}

		#endregion

		public void RemoveFromParent(View view)
		{
			(_contentPage as IActivityIndicatorViewProvider).RemoveFromParent(view);
		}

		public void AddToParent(View view)
		{
			(_contentPage as IActivityIndicatorViewProvider).AddToParent(view);
		}

		#endregion

	}

	public class NavigationContext
	{
		public Stack<NavigationElement> Elements { get; private set; }

		public NavigationContext(NavigationElement root)
		{
			Elements = new Stack<NavigationElement>();
			Elements.Push(root);
		}
	}

	public class NavigationElement
	{
		public View View { get; set; }
		public Action<bool> DismissedAction { get; set; }
		public INavigationContainer Container { get; set; }

		public NavigationElement(View view, INavigationContainer container, Action<bool> dismissedAction)
		{
			DismissedAction = dismissedAction;
			Container = container;
			View = view;
		}

		~NavigationElement()
		{
			System.Diagnostics.Debug.WriteLine("NavigationElement finalizer");
		}

		public override string ToString()
		{
			return string.Format("{0}", Container);
		}
	}
}

