using System;
using NControl.Mvvm;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmDemo
{
	public class CompanyViewModel: BaseViewModel
	{
		public CompanyViewModel ()
		{
			Title = "NControl.Mvvm";
		}

		public override async Task InitializeAsync ()
		{
			await base.InitializeAsync ();

			Companies.AddRange (Company.CompanyRepository);

			//await Task.Run (async() => {
			//	{
			//		MvvmApp.Current.ActivityIndicator.UpdateProgress(true, "Loading...", "Loading everything. This will be so go that... bla bla.");

			//		await Task.Delay(1500);
			//		MvvmApp.Current.ActivityIndicator.UpdateProgress(true, "Preparing...", "Preparing everything. This will be so go that... bla bla.");

			//		await Task.Delay(1500);
			//		MvvmApp.Current.ActivityIndicator.UpdateProgress(true, "Finding...", "Finding everything. This will be so go that... bla bla.");

			//		await Task.Delay(1500);
			//	}

			//	MvvmApp.Current.ActivityIndicator.UpdateProgress (false);
			//});
		}

		public ObservableCollectionWithAddRange<Company> Companies
		{
			get {
				return GetValue <ObservableCollectionWithAddRange<Company>> (() =>
				new ObservableCollectionWithAddRange<Company> ());
			}
		}

		public ICommand SelectCompanyCommand
		{
			get {
				return GetOrCreateCommandAsync<Company> (async(company) => {
					await MvvmApp.Current.Presenter.ShowViewModelAsync<EmployeeViewModel>(company);
				});
			}
		}

		public ICommand ShowAboutCommand
		{
			get{
				return GetOrCreateCommandAsync (async _=> {

					//await MvvmApp.Current.Presenter.ShowMessageAsync("Title", "Message");
					//await MvvmApp.Current.Presenter.ShowActionSheet("Title", "Cancel", "Delete", "item 1", "item 2");
					await MvvmApp.Current.Presenter.ShowViewModelModalAsync<AboutViewModel>();

				});
			}
		}

		/// <summary>
		/// Returns the SearchCommand command
		/// </summary>
		/// <value>The view SearchCommand command.</value>
		public ICommand SearchCommand {
			get {
				return GetOrCreateCommandAsync (async _=> {
					await MvvmApp.Current.Presenter.ShowViewModelModalAsync<SearchViewModel>();           
				});
			}
		}
	}
}

