using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface IViewHelperProvider
	{
		/// <summary>
		/// Gets the location on screen.
		/// </summary>
		Point GetLocationOnScreen(VisualElement element);

		/// <summary>
		/// Returns the local coordinate converted from a view
		/// </summary>
		/// <returns>The local location.</returns>
		Point GetLocalLocation(VisualElement element, Point point);
	}
}
