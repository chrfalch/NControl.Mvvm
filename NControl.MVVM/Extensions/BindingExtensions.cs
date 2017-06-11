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

namespace NControl.Mvvm
{
	/// <summary>
	/// Binding extensions.
	/// </summary>
	public static class BindingExtensions
	{
		/// <summary>
		/// Sets up a binding
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="view">View.</param>
		/// <param name="bindableProperty">Bindable property.</param>
		/// <param name="propertyName">Property name.</param>
		/// <param name="converter">Converter.</param>
		/// <param name="stringFormat">String format.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T BindTo<T>(this T view, BindableProperty bindableProperty, string propertyName, 
			IValueConverter converter = null, string stringFormat = null) where T : View
		{
			view.SetBinding(bindableProperty, propertyName, converter: converter, stringFormat:stringFormat);
			return view;
		}

		/// <summary>
		/// Sets up a binding
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="template">Template.</param>
		/// <param name="bindableProperty">Bindable property.</param>
		/// <param name="propertyName">Property name.</param>
		/// <param name="converter">Converter.</param>
		/// <param name="stringFormat">String format.</param>
		public static DataTemplate BindTo(this DataTemplate template, BindableProperty bindableProperty, 
			string propertyName, IValueConverter converter = null, string stringFormat = null){
			template.SetBinding(bindableProperty, new Binding(propertyName, converter: converter, stringFormat:stringFormat));
			return template;
		}
	}
}

