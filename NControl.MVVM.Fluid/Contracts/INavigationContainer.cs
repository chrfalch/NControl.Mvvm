using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface INavigationContainer
	{
		/// <summary>
		/// Returns the top level view for the container
		/// </summary>
		View GetBaseView();

		/// <summary>
		/// Returns the view containing the contents of the container
		View GetContentsView();

		/// <summary>
		/// Returns the chrome view for the container. Chrome is the
		/// navigationbar, frame or other "bling"
		/// </summary>
		View GetChromeView();

		/// <summary>
		/// Returns overlay view if any exists
		/// </summary>
		View GetOverlayView();

		/// <summary>
		/// Sets the content that is to be displayed in the container
		/// </summary>
		void SetContent(View content);

		/// <summary>
		/// Gets or sets the navigation context for the container
		/// </summary>
		NavigationContext NavigationContext { get; set; }
	}
}
