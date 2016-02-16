using System;
using NControl.MVVM;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace MvvmDemo
{
	public class CompanyViewModel: BaseViewModel
	{
		public CompanyViewModel ()
		{
			Title = "NControl.MVVM";
		}

		public override async Task InitializeAsync ()
		{
			await base.InitializeAsync ();

			Companies.AddRange (Company.CompanyRepository);
		}

		public ObservableCollectionWithAddRange<Company> Companies
		{
			get {
				return GetValue <ObservableCollectionWithAddRange<Company>> (() =>
				new ObservableCollectionWithAddRange<Company> ());
			}
		}

		public Command<Company> SelectCompanyCommand
		{
			get {
				return GetOrCreateCommand<Company> (async(company) => {
					await MvvmApp.Current.Presenter.ShowViewModelAsync<EmployeeViewModel>(company);
				});
			}
		}

		public Command ShowAboutCommand
		{
			get{
				return GetOrCreateCommand (async () => {

					await MvvmApp.Current.Presenter.ShowViewModelModalAsync<AboutViewModel>();

				});
			}
		}

		/// <summary>
		/// Returns the SearchCommand command
		/// </summary>
		/// <value>The view SearchCommand command.</value>
		public Command SearchCommand {
			get {
				return GetOrCreateCommand (async() => {
					await MvvmApp.Current.Presenter.ShowViewModelModalAsync<SearchViewModel>();           
				});
			}
		}
	}
}

