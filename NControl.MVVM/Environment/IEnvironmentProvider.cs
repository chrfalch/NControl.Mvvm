using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface IEnvironmentProvider
	{
		/// <summary>
		/// Returns a string indicating the current app version
		/// </summary>
		string AppVersion { get; }

		/// <summary>
		/// Returns the display density. Use to multiply with sizes etc.
		/// </summary>
		float DisplayDensity { get; }

		/// <summary>
		/// Gets the location on screen.
		/// </summary>
		Rectangle GetLocationOnScreen(VisualElement element);
		Rectangle GetLocalLocation(VisualElement element, Rectangle rect);
	}
}
