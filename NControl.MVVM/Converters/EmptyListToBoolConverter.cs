using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	/// <summary>
	/// Returns true if the collection is empty or null, false if it contains any elements. Use
	/// to bind activity indicators etc. to collections
	/// </summary>
	public class EmptyListToBoolConverter : IValueConverter
	{
		public EmptyListToBoolConverter()
		{
			ReturnValueWhenEmpty = true;
		}

		public EmptyListToBoolConverter(bool returnValueWhenEmpty)
		{
			ReturnValueWhenEmpty = returnValueWhenEmpty;
		}

		public bool ReturnValueWhenEmpty { get; private set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return ReturnValueWhenEmpty;

			if (value is IList)
				return (value as IList).Count == 0 ? ReturnValueWhenEmpty : !ReturnValueWhenEmpty;

			return ReturnValueWhenEmpty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
