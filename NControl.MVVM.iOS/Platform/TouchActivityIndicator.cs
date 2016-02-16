using System;
using BigTed;
using Foundation;
using Xamarin.Forms;

namespace NControl.MVVM.iOS
{
	/// <summary>
	/// Touch activity indicator.
	/// </summary>
	public class TouchActivityIndicator: IActivityIndicator
	{
		#region IActivityIndicator implementation

		/// <summary>
		/// Updates the progress.
		/// </summary>
		/// <param name="visible">If set to <c>true</c> visible.</param>
		/// <param name="title">Title.</param>
		/// <param name="subtitle">Subtitle.</param>
		public void UpdateProgress (bool visible, string title = "", string subtitle = "")
		{
			Action updateProgressAction = () => {
				if (visible) {
					if (!BTProgressHUD.IsVisible)
						BTProgressHUD.Show (title, maskType: ProgressHUD.MaskType.Black);
					else
						BTProgressHUD.SetStatus (title);
				} else if (!visible && BTProgressHUD.IsVisible)
					BTProgressHUD.Dismiss ();
			};

			if (!NSThread.IsMain)
				Device.BeginInvokeOnMainThread (updateProgressAction);
			else
				updateProgressAction ();
		}

		#endregion
	}
}

