using System;
using NControl.Mvvm;
using NControl.XAnimation;
using Xamarin.Forms;

namespace MvvmDemo
{
	/// <summary>
	/// Demo mvvm app.
	/// </summary>
	public class DemoMvvmApp: FluidMvvmApp
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MvvmDemo.DemoMvvmApp"/> class.
		/// </summary>
		/// <param name="platform">Platform.</param>
		public DemoMvvmApp (IMvvmPlatform platform): base(platform)
		{
			//XElementContainer.SlowAnimations = true;
			//XElementContainer.ShowSetTransforms = true;
		}

		public override Type GetMainViewType()
		{
			return typeof(CompanyView);
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
			ViewContainer.RegisterView<FeedViewModel, FeedView>();
			ViewContainer.RegisterView<CityListViewModel, CityListView>();
			ViewContainer.RegisterView<CityViewModel, CityView>();
			ViewContainer.RegisterView<CityDetailsViewModel, CityDetailsView>();
		}
	}
}

