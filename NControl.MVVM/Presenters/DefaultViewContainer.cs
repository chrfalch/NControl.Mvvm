/****************************** Module Header ******************************\
Module Name:  DefaultViewContainer.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NControl.MVVM
{
	public class DefaultViewContainer: IViewContainer
	{
		#region Private Members

		/// <summary>
		/// Viewmodel/View mapping
		/// </summary>
		private Dictionary<Type, Type> ViewModels = new Dictionary<Type, Type>();

		#endregion

		/// <summary>
		/// Registers the view.
		/// </summary>
		/// <typeparam name="TViewModelType">The 1st type parameter.</typeparam>
		/// <typeparam name="TViewType">The 2nd type parameter.</typeparam>
		public void RegisterView<TViewModelType, TViewType>()
			where TViewModelType : BaseViewModel
			where TViewType : Page
		{
			ViewModels.Add (typeof(TViewModelType), typeof(TViewType));
		}

		/// <summary>
		/// Gets the view from view model.
		/// </summary>
		/// <returns>The view from view model.</returns>
		/// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
		public Page GetViewFromViewModel<TViewModel>() where TViewModel : BaseViewModel
		{
			return Container.Resolve (ViewModels [typeof(TViewModel)]) as Page;
		}
	}
}

