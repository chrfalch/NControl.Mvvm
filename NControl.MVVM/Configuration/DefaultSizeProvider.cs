using System;
using System.Collections.Generic;

namespace NControl.Mvvm
{
	public class DefaultSizeProvider : ISizeProvider
	{
		#region Members

		readonly Dictionary<string, double> _sizes = new Dictionary<string, double>();

		#endregion

		#region Constants

		#endregion

		public double Get(string name)
		{
			if (_sizes.ContainsKey(name))
				return _sizes[name];

			return 0.0;
		}

		public void Set(string name, double value)
		{
			_sizes[name] = value;
		}
	}
}
