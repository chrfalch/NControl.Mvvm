/****************************** Module Header ******************************\
Module Name:  TouchMvvmApp.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using NControl.Controls.iOS;

namespace NControl.Mvvm.iOS
{
	/// <summary>
	/// Touch mvvm app.
	/// </summary>
	public class TouchPlatform: IMvvmPlatform
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NControl.Mvvm.iOS.TouchMvvmApp"/> class.
		/// </summary>
		public virtual void Initialize ()
		{
			NControls.Init ();
			RegisterActivityIndicator();
			Container.Register<IImageProvider, TouchImageProvider> ();
			Container.Register<IGestureRecognizerProvider, TouchGestureRecognizerProvider>();
			Container.RegisterSingleton<IEnvironmentProvider, TouchEnvironmentProvider>();
		}

		public virtual void RegisterActivityIndicator()
		{
			Container.Register<IActivityIndicator, TouchActivityIndicator>();
		}
	}
}

