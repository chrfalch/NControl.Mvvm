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

			await Task.Delay(1550);
			Companies.AddRange (Company.CompanyRepository);
			CollectionState = CollectionState.Loaded;

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
				return GetValue(() => new ObservableCollectionWithAddRange<Company> ());
			}
		}

		public CollectionState CollectionState
		{
			get { return GetValue<CollectionState>(); }
			set { SetValue(value); }
		}

		public ICommand SelectCompanyCommand
		{
			get {
				return GetOrCreateCommandAsync<Company> (async(company) => {
					await MvvmApp.Current.Presenter.ShowViewModelAsync<EmployeeViewModel>(parameter:company, dismissedCallback:(b) => {
						System.Diagnostics.Debug.WriteLine("Employee Details Closed.");
					});
				});
			}
		}

		public ICommand ShowAboutCommand
		{
			get{
				return GetOrCreateCommandAsync (async _=> {

					await MvvmApp.Current.Presenter.ShowViewModelAsync<AboutViewModel>(PresentationMode.Modal, dismissedCallback: (b) =>
					{
						MvvmApp.Current.Presenter.ShowMessageAsync("Yup", "AboutViewModel Closed.");
					});

				});
			}
		}

		[DependsOn(nameof(CollectionState))]
		public ICommand RefreshCommand
		{
			get 
			{ 
				return GetOrCreateCommandAsync(async _ =>{					

					CollectionState = CollectionState.Loading;

					try
					{						
						await Task.Delay(1500);
						Companies.Clear();
						// Companies.AddRange(Company.CompanyRepository);
					}
					finally
					{
						CollectionState = CollectionState.Loaded;
					}

				}, _=> CollectionState != CollectionState.Loading);
			}
		}

		/// <summary>
		/// Returns the SearchCommand command
		/// </summary>
		/// <value>The view SearchCommand command.</value>
		public ICommand SearchCommand {
			get {
				return GetOrCreateCommandAsync (async _=> {
					await MvvmApp.Current.Presenter.ShowViewModelAsync<SearchViewModel>(PresentationMode.Modal);           
				});
			}
		}

		public ICommand ShowFeedCommand
		{
			get
			{
				return GetOrCreateCommandAsync(async _ =>
				{
					await MvvmApp.Current.Presenter.ShowViewModelAsync<FeedViewModel>(PresentationMode.Modal);
				});
			}
		}

		public ICommand ShowMenuCommand
		{
			get
			{
				return GetOrCreateCommandAsync(async _ =>
				{
					await MvvmApp.Current.Presenter.ShowViewModelAsync<MenuViewModel>(PresentationMode.Modal);
				});
			}
		}
	}
}

