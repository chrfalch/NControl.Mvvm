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

			CollectionState = CollectionState.Loading;
			await Task.Delay(1550);
			Companies.AddRange (Company.CompanyRepository);
			CollectionState = CollectionState.Loaded;
		}

		public ObservableCollectionWithAddRange<Company> Companies =>
			GetValue(() => new ObservableCollectionWithAddRange<Company> ());

		public CollectionState CollectionState
		{
			get { return GetValue<CollectionState>(); }
			set { SetValue(value); }
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
					}
					finally
					{
						CollectionState = CollectionState.Loaded;
					}

				}, _=> CollectionState != CollectionState.Loading);
			}
		}

		public ICommand SelectCompanyCommand => GetCommand(() => new PresentDefaultCommand<EmployeeViewModel>());
		public ICommand ShowAboutCommand => GetCommand(() => new PresentModalCommand<AboutViewModel>());
		public ICommand SearchCommand => GetCommand(() => new PresentModalCommand<SearchViewModel>());
		public ICommand ShowFeedCommand => GetCommand(() => new PresentModalCommand<FeedViewModel>());
		public ICommand ShowMenuCommand => GetCommand(() => new PresentModalCommand<MenuViewModel>());
	}
}

