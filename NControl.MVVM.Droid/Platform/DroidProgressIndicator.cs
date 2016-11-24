using System;
using Android.App;
using System.Threading;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	/// <summary>
	/// Droid activity indicator.
	/// </summary>
	public class DroidActivityIndicator: IActivityIndicator
	{
		#region Class Members

		/// <summary>
		/// The progress dialog.
		/// </summary>
		private ProgressDialog _progressDialog;

		#endregion

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

				// Check if we already have a progress dialog up and running
				if (_progressDialog == null) {
					_progressDialog = new ProgressDialog (Xamarin.Forms.Forms.Context);
					_progressDialog.SetCancelable(false);
				}

				_progressDialog.SetMessage(title);
				if(visible && !_progressDialog.IsShowing)
					_progressDialog.Show();

				if (!visible && _progressDialog.IsShowing)
				{
					_progressDialog.Hide();
					_progressDialog = null;
				}
			};

			if (Android.App.Application.SynchronizationContext != SynchronizationContext.Current)
				Device.BeginInvokeOnMainThread (updateProgressAction);
			else
				updateProgressAction ();
		}
		#endregion
	}
}