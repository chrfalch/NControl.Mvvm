using System;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class CityListView : BaseFluidItemListView<CityListViewModel, FeedItem>
	{
		public override Type GetCellType()
		{
			return typeof(CityCell);
		}
	}
}
