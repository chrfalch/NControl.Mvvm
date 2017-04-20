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

		/// <summary>
		/// Called when the container has been navigated to and is presented and ready on screen.
		/// </summary>
		void OnNavigatedTo(NavigationElement toElement);

		/// <summary>
		/// Called when the container has been navigated out/from and is dismissed and not longer visible on screen.
		/// </summary>
		void OnNavigatedFrom(NavigationElement fromElement);
	}
}
