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
using Android.App;

namespace NControl.Mvvm.Droid
{
	/// <summary>
	/// Droid mvvm app.
	/// </summary>
	public class DroidPlatform: IMvvmPlatform
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NControl.MVVM.Droid.DroidPlatform"/> class.
		/// </summary>
		public DroidPlatform (Activity activity)
		{			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NControl.MVVM.Droid.DroidMvvmApp"/> class.
		/// </summary>
		public virtual void Initialize ()
		{
			using (PerformanceTimer.Current.BeginTimer(this))
			{
				using(PerformanceTimer.Current.BeginTimer(this, "NControls.Init()"))
					NControls.Init();

				RegisterActivityIndicator();

				using (PerformanceTimer.Current.BeginTimer(this, "Default providers"))
				{
					Container.Register<IImageProvider, DroidImageProvider>();
					Container.Register<IGestureRecognizerProvider, DroidGestureRecognizerProvider>();
					Container.RegisterSingleton<IEnvironmentProvider, DroidEnvironmentProvider>();
				}
			}
		}

		public virtual void RegisterActivityIndicator()
		{
			using(PerformanceTimer.Current.BeginTimer(this))
				Container.RegisterSingleton<IActivityIndicator, DroidActivityIndicator>();
		}
	}
}

