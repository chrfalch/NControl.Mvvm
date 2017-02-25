using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public static class GridExtensions
	{
		public static Grid AddChildTo(this Grid grid, View child, int column, int row)
		{
			grid.Children.Add(child, column, row);
			return grid;
		}
	}
}
