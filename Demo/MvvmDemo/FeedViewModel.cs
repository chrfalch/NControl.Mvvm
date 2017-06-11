using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using NControl.Mvvm;

namespace MvvmDemo
{
	public class FeedViewModel: BaseItemListViewModel<FeedItem>
	{
		public override Task InitializeAsync()
		{
			Title = "Feed";
			return base.InitializeAsync();
		}

		public override Task<IEnumerable<FeedItem>> LoadItemsAsync()
		{
			return Task.FromResult(FeedItem.FeedRepository);
		}
	}
}
