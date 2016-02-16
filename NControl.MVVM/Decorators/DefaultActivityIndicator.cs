/****************************** Module Header ******************************\
Module Name:  BindingExtensions.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using Acr.UserDialogs;

namespace NControl.MVVM
{
	/// <summary>
	/// Default activity indicator.
	/// </summary>
	public class DefaultActivityIndicator: IActivityIndicator
	{
		/// <summary>
		/// The progress dialog.
		/// </summary>
		private IProgressDialog _progressDialog;

		/// <summary>
		/// Shows the progress.
		/// </summary>
		/// <param name="visible">If set to <c>true</c> visible.</param>
		/// <param name="title">Title.</param>
		/// <param name="subtitle">Subtitle.</param>
		public void UpdateProgress(bool visible, string title = "", string subtitle = "")
		{
			if (_progressDialog == null && visible == false)
				return;

			if (_progressDialog == null)
			{
				_progressDialog = UserDialogs.Instance.Progress();
				_progressDialog.IsDeterministic = false;
			}

			_progressDialog.Title = title ?? string.Empty;

			if (visible)
			{
				if(!_progressDialog.IsShowing)
					_progressDialog.Show();
			}
			else
			{
				if(_progressDialog.IsShowing)
					_progressDialog.Hide();

				_progressDialog = null;
			}
		}
	}
}

