using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public abstract class BaseFluidItemListView<TViewModel, TItemType> :
		BaseFluidContentsView<TViewModel>
			where TViewModel : BaseItemListViewModel<TItemType>
			where TItemType : class
	{
		#region Abstract Members

		/// <summary>
		/// Gets the data template.
		/// </summary>
		/// <returns>The data template.</returns>
		public abstract DataTemplate GetDataTemplate();

		/// <summary>
		/// Gets the height of the row.
		/// </summary>
		/// <returns>The row height.</returns>
		public virtual int GetRowHeight()
		{
			return -1;
		}

		/// <summary>
		/// Override to customize listview
		/// </summary>
		protected virtual void CustomizeListView(ListViewEx listView)
		{ 
		}

		#endregion

		#region Overridden Members

		protected override View CreateContents()
		{
			var retVal = new ListViewControl
			{
				IsPullToRefreshEnabled = true,
				ItemTemplate = GetDataTemplate(),
				RowHeight = GetRowHeight(),
			}
				.BindTo(ListViewControl.ItemsSourceProperty, nameof(ViewModel.Items))
				.BindTo(ListViewControl.ItemSelectedCommandProperty, nameof(ViewModel.SelectItemCommand))
				.BindTo(ListViewControl.RefreshCommandProperty, nameof(ViewModel.RefreshCommand))
				.BindTo(ListViewControl.StateProperty, nameof(ViewModel.CollectionState));

			CustomizeListView(retVal.InternalListView);

			return retVal;
		}
		#endregion
	}
}
