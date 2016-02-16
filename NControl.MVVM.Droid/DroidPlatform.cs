/****************************** Module Header ******************************\
Module Name:  DroidMvvmApp.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using NControl.Controls.Droid;
using Android.Content;
using Acr.UserDialogs;
using Android.App;

namespace NControl.MVVM.Droid
{
	/// <summary>
	/// Droid mvvm app.
	/// </summary>
	public class DroidPlatform: IMvvmPlatform
	{
		/// <summary>
		/// The activity
		/// </summary>
		private readonly Activity _activity;

		/// <summary>
		/// Initializes a new instance of the <see cref="NControl.MVVM.Droid.DroidPlatform"/> class.
		/// </summary>
		public DroidPlatform (Activity activity)
		{
			_activity = activity;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NControl.MVVM.Droid.DroidMvvmApp"/> class.
		/// </summary>
		public void Initialize ()
		{
			NControls.Init ();
			UserDialogs.Init(_activity);

			Container.Register<IImageProvider, DroidImageProvider> ();
		}
	}
}

