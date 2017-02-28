using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface IColorProvider
	{
		Color Get(string name);
		void Set(string name, Color value);
	}
}
