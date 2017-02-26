using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public class FluidContainerPage : ContentPage
	{
		#region Members
		Grid _contentsContainer;
		readonly Stack<NavigationContext> _contextStack = new Stack<NavigationContext>();
		#endregion

		public FluidContainerPage()
		{
			_contentsContainer = new Grid();
			Content = _contentsContainer;
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

		public Grid Container { get { return _contentsContainer; } }
		public Stack<NavigationContext> Stack { get { return _contextStack; } }
	}
}
