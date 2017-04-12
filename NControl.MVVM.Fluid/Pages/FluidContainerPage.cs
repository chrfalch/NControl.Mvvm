using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidContainerPage : ContentPage, IActivityIndicatorViewProvider
	{
		#region Members

		/// container for content
		Grid _contentsContainer;

		/// Container for activity indicator
		Grid _activityContainer;

		// Navigation contexts
		readonly Stack<NavigationContext> _contexts = new Stack<NavigationContext>();

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NControl.Mvvm.FluidContainerPage"/> class.
		/// </summary>
		public FluidContainerPage()
		{
			_contentsContainer = new Grid();
			_activityContainer = new Grid { Children = { _contentsContainer } };
			Content = _activityContainer;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the contents container.
		/// </summary>
		public Grid Container { get { return _contentsContainer; } }

		/// <summary>
		/// Returns the current navigation context.
		/// </summary>
		/// <value>The current context.</value>
		public NavigationContext CurrentContext
		{
			get { return _contexts.Peek(); }
		}

		/// <summary>
		/// Returns the current navigation element from the current context
		/// </summary>
		public NavigationElement CurrentNavigationElement
		{
			get { return CurrentContext.Elements.Peek(); }
		}

		/// <summary>
		/// Returns the list of navigation contexts.
		/// </summary>
		public Stack<NavigationContext> Contexts
		{
			get { return _contexts; }
		}

		#endregion

		#region Overridden Members

		/// <summary>
		/// Handle backbutton press
		/// </summary>
		protected override bool OnBackButtonPressed()
		{
			// Find current navigation element
			if(CurrentNavigationElement == null || CurrentContext.Elements.Count == 1)
				return base.OnBackButtonPressed();

			// Find current viewmodel
			var viewModelProvider = CurrentNavigationElement.Container.GetContentsView() as IView;
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

		#endregion

		#region IActivityIndicatorViewProvider

		/// <summary>
		/// Removes from parent.
		/// </summary>
		/// <param name="view">View.</param>
		public void RemoveFromParent(View view)
		{
			_activityContainer.Children.Remove(view);
		}

		/// <summary>
		/// Adds to parent.
		/// </summary>
		/// <param name="view">View.</param>
		public void AddToParent(View view)
		{
			_activityContainer.Children.Add(view, 0, 0);
		}

		#endregion
	}
}
