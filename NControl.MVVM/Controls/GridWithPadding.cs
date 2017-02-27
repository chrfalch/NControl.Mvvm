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
			Padding = new Thickness(8 * 2, 8);
		}
	}
}
