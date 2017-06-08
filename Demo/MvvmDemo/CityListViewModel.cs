using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using NControl.Mvvm;

namespace MvvmDemo
{
	public class CityListViewModel: BaseItemListViewModel<FeedItem>
	{
		public override Task InitializeAsync()
		{
			Title = "Cities";
			return base.InitializeAsync();
		}

		public override ICommand SelectItemCommand => GetCommand(
			() => new NavigateCommand<CityViewModel>());

		public override Task<IEnumerable<FeedItem>> LoadItemsAsync()
		{
			return Task.FromResult(FeedItem.FeedRepository);
		}
	}
}
