using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class GridWithPadding: Grid
	{
		public GridWithPadding()
		{
			ColumnSpacing = 0;
			RowSpacing = 0;
			Padding = new Thickness(MvvmApp.Current.Sizes.Get(Config.DefaultPadding) * 2, 
           		MvvmApp.Current.Sizes.Get(Config.DefaultPadding));
		}
	}
}
