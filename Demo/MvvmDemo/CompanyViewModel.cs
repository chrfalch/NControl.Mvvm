using System;
using NControl.Mvvm;
using Xamarin.Forms;
using System.Threading.Tasks;

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

			await Task.Run (async() => {
				MvvmApp.Current.ActivityIndicator.UpdateProgress (true, "Loading...");

				await Task.Delay (1000);

				MvvmApp.Current.ActivityIndicator.UpdateProgress (false);
			});
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

					await MvvmApp.Current.Presenter.ShowMessageAsync("Title", "Message");
					await MvvmApp.Current.Presenter.ShowActionSheet("Title", "Cancel", "Delete", "item 1", "item 2");
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

