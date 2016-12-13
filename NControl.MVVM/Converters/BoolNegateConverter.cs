using System;
using System.Globalization;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	/// <summary>
	/// Implements a converter that negates the boolean input.
	/// </summary>
	public class BoolNegateConverter: IValueConverter
	{
		/// <param name="value">To be added.</param>
		/// <param name="targetType">To be added.</param>
		/// <param name="parameter">To be added.</param>
		/// <param name="culture">To be added.</param>
		/// <summary>
		/// Convert the specified value, targetType, parameter and culture.
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return true;

			return !(bool)value;
		}

		/// <param name="value">To be added.</param>
		/// <param name="targetType">To be added.</param>
		/// <param name="parameter">To be added.</param>
		/// <param name="culture">To be added.</param>
		/// <summary>
		/// Converts the back.
		/// </summary>
		/// <returns>The back.</returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return true;

			return !(bool)value;
		}
	}
}
