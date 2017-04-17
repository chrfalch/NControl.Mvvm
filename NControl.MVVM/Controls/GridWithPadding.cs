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
			Padding = new Thickness(Config.DefaultPadding * 2, Config.DefaultPadding);
		}
	}
}
