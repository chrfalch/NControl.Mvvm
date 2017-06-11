using System;
using NControl.Mvvm;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;

namespace MvvmDemo
{
	public class CompanyViewModel : BaseItemListViewModel<Company>
	{
		public CompanyViewModel()
		{
			Title = "NControl.Mvvm";

		}

		public override Task<IEnumerable<Company>> LoadItemsAsync()
		{			
			return Task.FromResult(Company.CompanyRepository);
		}

		public override ICommand SelectItemCommand => GetCommand(() => new NavigateCommand<EmployeeViewModel>());

		public ICommand RefreshEmptyCommand => GetOrCreateCommandAsync(async (arg) => {

			CollectionState = CollectionState.Loading;
			Items.Clear();
			await Task.Delay(100);
			CollectionState = CollectionState.Loaded;
		});

		public ICommand ShowAboutCommand => GetCommand(() => new NavigateModalCommand<AboutViewModel>());
		public ICommand SearchCommand => GetCommand(() => new NavigateModalCommand<SearchViewModel>());
		public ICommand ShowFeedCommand => GetCommand(() => new NavigateModalCommand<FeedViewModel>());
		public ICommand ShowCityListCommand => GetCommand(() => new NavigateModalCommand<CityListViewModel>());
	}
}

