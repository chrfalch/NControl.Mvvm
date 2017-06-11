using System;
using NControl.Controls;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class CityListView : BaseFluidItemListView<CityListViewModel, FeedItem>
	{
		public CityListView()
		{
			ToolbarItems.Add(new ToolbarItemEx
			{
				MaterialDesignIcon = FontMaterialDesignLabel.MDClose,
				Command = ViewModel.CloseCommand,
			});
		}

		public override DataTemplate GetDataTemplate()
		{
			return new DataTemplate(typeof(CityCell));
		}

		public override int GetRowHeight()
		{
			return 200;
		}
	}
}
