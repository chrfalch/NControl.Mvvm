using System;
using NControl.Controls;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidBlurOverlay : ContentView
	{
		#region Events

		public event EventHandler OnScreenshotRequest;

		#endregion

		public FluidBlurOverlay()
		{			
		}

		#region Private Members

		/// <summary>
		/// Takes the screenshot.
		/// </summary>
		void TakeScreenshot()
		{
			OnScreenshotRequest?.Invoke(this, EventArgs.Empty);
		}
		#endregion

		#region Overridden Members

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
			TakeScreenshot();
		}
		#endregion
	}
}
