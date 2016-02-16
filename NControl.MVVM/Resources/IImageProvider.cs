/****************************** Module Header ******************************\
Module Name:  IImageProvider.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using Xamarin.Forms;

namespace NControl.MVVM
{
	/// <summary>
	/// image provider.
	/// </summary>
	public interface IImageProvider
	{
		/// <summary>
		/// Returns an image asset loaded on the platform
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="imageName">Image name.</param>
		FileImageSource GetImageSource(string imageName);
	}
}

