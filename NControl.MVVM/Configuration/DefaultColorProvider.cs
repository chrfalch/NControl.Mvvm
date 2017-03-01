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

		public DefaultColorProvider()
		{
			_colors[Config.PrimaryColor] = Color.FromHex("#2196F3");
			_colors[Config.PrimaryDarkColor] = Color.FromHex("#1976D2");
			_colors[Config.AccentColor] = Color.Accent;
			_colors[Config.TextColor] = Color.White;
			_colors[Config.BorderColor] = Color.FromHex("CECECE");
			_colors[Config.ViewBackgroundColor] = Color.FromHex("FEFEFE");
			_colors[Config.ViewTransparentBackgroundColor] = Color.Black.MultiplyAlpha(0.75);
			_colors[Config.AccentTextColor] = Color.Accent;
		}

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
