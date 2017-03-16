using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NControl.Mvvm;

namespace MvvmDemo
{
	public class FeedViewModel: BaseViewModel
	{
		public FeedViewModel()
		{
			Title = "Feed";
		}

		public override async Task InitializeAsync()
		{			
			await base.InitializeAsync();
			await ReloadItemsAsync();


		}

		public ObservableCollectionWithAddRange<FeedItem> FeedItems
		{
			get { return GetValue(() => new ObservableCollectionWithAddRange<FeedItem>()); }		
		}

		public ICommand SelectItemCommand
		{
			get
			{
				return GetOrCreateCommandAsync<FeedItem>(async (arg) => {
					await Task.Delay(100);
				});
			}
		}

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
				return GetOrCreateCommandAsync(async _=>
					await ReloadItemsAsync(), _=> CollectionState != CollectionState.Loading);
			}
		}

		async Task ReloadItemsAsync()
		{
			CollectionState = CollectionState.Loading;

			try
			{
				await Task.Delay(10);
				foreach (var item in FeedItem.FeedRepository)
					FeedItems.Add(item);
			}
			finally
			{
				CollectionState = CollectionState.Loaded;
			}
		}
	}
}
