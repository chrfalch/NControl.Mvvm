using System;
using NControl.Controls;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class FeedView : BaseFluidItemListView<FeedViewModel, FeedItem>
	{
		public FeedView()
		{
			ToolbarItems.Add(new ToolbarItemEx
			{
				MaterialDesignIcon = FontMaterialDesignLabel.MDClose,
				Command = ViewModel.CloseCommand,
			});
		}

		public override Type GetCellType()
		{
			return typeof(FeedCell);
		}
	}
}
