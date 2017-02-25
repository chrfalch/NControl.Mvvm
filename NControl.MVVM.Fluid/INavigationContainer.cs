using System;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public interface INavigationContainer
	{
		void AddChild(View view);
		void RemoveChild(View view);
		int Count { get; }
	}
}
