using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using NControl.Mvvm;

namespace MvvmDemo
{
	public class CityListViewModel: BaseItemListViewModel<FeedItem>
	{
		public override Task<IEnumerable<FeedItem>> LoadItemsAsync()
		{
			return Task.FromResult(FeedItem.FeedRepository);
		}
	}
}
