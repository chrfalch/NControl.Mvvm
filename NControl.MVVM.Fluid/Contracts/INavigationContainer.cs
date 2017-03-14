using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface INavigationContainer
	{
		void AddChild(View view, PresentationMode presentationMode);
		void RemoveChild(View view, PresentationMode presentationMode);
		View GetChild(int index);
		int Count { get; }

		// Returns the top level view for the container
		View GetRootView();

		// Returns the navigation view for the container
		View GetNavigationBarView();

		// Returns the container view for the container. This 
		// is the view containing the contentviews themselves
		View GetContainerView();

		/// <summary>
		/// Returns overlay view if any exists
		/// </summary>
		View GetOverlayView();
	}
}
