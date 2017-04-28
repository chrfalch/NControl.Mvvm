using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class ModalNavigationPage: NavigationPage
	{		
		/// <summary>
		/// Initializes a new instance of the <see cref="NControl.Mvvm.ModalNavigationPage"/> class.
		/// </summary>
		/// <param name="page">Page.</param>
		public ModalNavigationPage (Page page, BaseViewModel firstChildViewModel): base(page)
		{
			FirstChildViewModel = firstChildViewModel;
		}

		/// <summary>
		/// Gets the first child view model.
		/// </summary>
		/// <value>The first child view model.</value>
		public BaseViewModel FirstChildViewModel
		{
			get;
			private set;
		}
	}
}

