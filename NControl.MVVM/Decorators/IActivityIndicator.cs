/****************************** Module Header ******************************\
Module Name:  IActivityIndicator.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;

namespace NControl.MVVM
{
	/// <summary>
	/// I activity indicator.
	/// </summary>
	public interface IActivityIndicator
	{
		/// <summary>
		/// Updates the progress.
		/// </summary>
		/// <param name="visible">If set to <c>true</c> visible.</param>
		/// <param name="title">Title.</param>
		/// <param name="subtitle">Subtitle.</param>
		void UpdateProgress(bool visible, string title = "", string subtitle = "");
	}
}

