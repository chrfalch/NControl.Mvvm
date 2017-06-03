using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public abstract class BaseFluidItemListView<TViewModel, TItemType> : BaseFluidContentsView<TViewModel>
		where TViewModel : BaseItemListViewModel<TItemType>
		where TItemType : class
	{
		public abstract Type GetCellType();

		protected override View CreateContents()
		{
			return new ListViewControl
			{
				ItemsSource = ViewModel.Items,
				IsPullToRefreshEnabled = true,
				RowHeight = 220,
				ItemSelectedCommand = ViewModel.SelectItemCommand,
				ItemTemplate = new DataTemplate(GetCellType()),
			}
				.BindTo(ListViewControl.RefreshCommandProperty, nameof(ViewModel.RefreshCommand))
				.BindTo(ListViewControl.StateProperty, nameof(ViewModel.CollectionState));
		}
	}
}
