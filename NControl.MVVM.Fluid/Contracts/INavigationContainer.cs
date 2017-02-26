using System;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public interface INavigationContainer
	{
		void AddChild(View view, PresentationMode presentationMode);
		void RemoveChild(View view, PresentationMode presentationMode);
		int Count { get; }
		View GetRootView();
	}
}
