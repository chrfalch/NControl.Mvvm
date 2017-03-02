using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class DefaultColorProvider : IColorProvider
	{
		#region Members

		readonly Dictionary<string, Color> _colors = new Dictionary<string, Color>();

		#endregion

		#region Constants

		#endregion

		public Color Get(string name)
		{
			if (_colors.ContainsKey(name))
				return _colors[name];

			return Color.Accent;
		}

		public void Set(string name, Color value)
		{
			_colors[name] = value;
		}
	}
}
