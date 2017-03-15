using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidContainerPage : ContentPage, IActivityIndicatorViewProvider
	{
		#region Members
		Grid _contentsContainer;
		Grid _activityContainer;
		readonly Stack<NavigationContext> _contextStack = new Stack<NavigationContext>();
		#endregion

		public FluidContainerPage()
		{			
			_contentsContainer = new Grid();
			_activityContainer = new Grid { Children = { _contentsContainer } };
			Content = _activityContainer;
		}

		protected override bool OnBackButtonPressed()
		{
			// Find current navigation element
			var currentNavigationElement = Stack.FirstOrDefault();
			if(currentNavigationElement == null || (Stack.Count == 1 && Stack.Peek().NavigationStack.Count == 1))
				return base.OnBackButtonPressed();

			// Find current view
			var currentView = currentNavigationElement.NavigationStack.FirstOrDefault();
			if(currentView == null)
				return base.OnBackButtonPressed();

			// Find current viewmodel
			var viewModelProvider = currentView as IView;
			if (viewModelProvider == null)
				return base.OnBackButtonPressed();

			// Handle navigation
			var viewModel = viewModelProvider.GetViewModel();

			// Do we have any overridden behaviour here?
			var viewModelBase = viewModel as BaseViewModel;
			if (viewModelBase != null && viewModelBase.BackButtonCommand != null &&
			   viewModelBase.BackButtonCommand.CanExecute(null))
				viewModelBase.BackButtonCommand.Execute(null);
			else
				MvvmApp.Current.Presenter.DismissViewModelAsync(viewModel.PresentationMode);
			
			return true;
		}

		public void RemoveFromParent(View view)
		{			
			_activityContainer.Children.Remove(view);
		}

		public void AddToParent(View view)
		{
			_activityContainer.Children.Add(view, 0, 0);
		}

		public Grid Container { get { return _contentsContainer; } }
		public Stack<NavigationContext> Stack { get { return _contextStack; } }
	}
}
