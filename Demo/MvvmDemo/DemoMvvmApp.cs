using System;
using NControl.MVVM;
using Xamarin.Forms;

namespace MvvmDemo
{
	/// <summary>
	/// Demo mvvm app.
	/// </summary>
	public class DemoMvvmApp: MvvmApp
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MvvmDemo.DemoMvvmApp"/> class.
		/// </summary>
		/// <param name="platform">Platform.</param>
		public DemoMvvmApp (IMvvmPlatform platform): base(platform)
		{
		}

		/// <summary>
		/// Registers the views.
		/// </summary>
		protected override void RegisterViews ()
		{
			ViewContainer.RegisterView<CompanyViewModel, CompanyView> ();
			ViewContainer.RegisterView<EmployeeViewModel, EmployeeView> ();
			ViewContainer.RegisterView<AboutViewModel, AboutView> ();
			ViewContainer.RegisterView<EmployeeDetailsViewModel, EmployeeDetailsView> ();
			ViewContainer.RegisterView<SearchViewModel, SearchView> ();
		}

		/// <summary>
		/// Gets the main page.
		/// </summary>
		/// <returns>The main page.</returns>
		protected override Xamarin.Forms.Page GetMainPage ()
		{
			return new NavigationPage(Container.Resolve<CompanyView> ());
		}
	}
}

