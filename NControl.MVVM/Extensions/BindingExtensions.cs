/****************************** Module Header ******************************\
Module Name:  BindingExtensions.cs
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
	/// Binding extensions.
	/// </summary>
	public static class BindingExtensions
	{
		/// <summary>
		/// Sets up a binding
		/// </summary>
		/// <param name="bindableObject">Bindable object.</param>
		/// <param name="bindableProperty">Bindable property.</param>
		/// <param name="viewModelProperty">View model property.</param>
		public static View BindTo(this View view, BindableProperty bindableProperty, string propertyName, 
			IValueConverter converter = null, string stringFormat = null)
		{
			view.SetBinding(bindableProperty, propertyName, converter: converter, stringFormat:stringFormat);
			return view;
		}

		/// <summary>
		/// Binds to.
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="template">Template.</param>
		/// <param name="bindableProperty">Bindable property.</param>
		/// <param name="propertyName">Property name.</param>
		/// <param name="converter">Converter.</param>
		public static DataTemplate BindTo(this DataTemplate template, BindableProperty bindableProperty, 
			string propertyName, IValueConverter converter = null, string stringFormat = null){
			template.SetBinding(bindableProperty, new Binding(propertyName, converter: converter, stringFormat:stringFormat));
			return template;
		}
	}
}

