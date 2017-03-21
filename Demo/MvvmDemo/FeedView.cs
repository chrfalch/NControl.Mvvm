using System;
using NControl.Controls;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class FeedView : BaseFluidContentsView<FeedViewModel>
	{
		public FeedView()
		{
			ToolbarItems.Add(new ToolbarItemEx
			{
				MaterialDesignIcon = FontMaterialDesignLabel.MDClose,
				Command = ViewModel.CloseCommand,
			});
		}

		protected override View CreateContents()
		{
			return new ListViewControl
			{
				ItemsSource = ViewModel.FeedItems,
				IsPullToRefreshEnabled = true,
				RowHeight = 220,
				ItemSelectedCommand = ViewModel.SelectItemCommand,
				ItemTemplate = new DataTemplate(typeof(FeedCell)),
			}
				.BindTo(ListViewControl.RefreshCommandProperty, nameof(ViewModel.RefreshCommand))
				.BindTo(ListViewControl.StateProperty, nameof(ViewModel.CollectionState));
		}
	}
}
