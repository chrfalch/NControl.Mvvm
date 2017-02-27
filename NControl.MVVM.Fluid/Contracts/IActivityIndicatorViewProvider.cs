using System;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public interface IActivityIndicatorViewProvider
	{
		void RemoveFromParent(View view);
		void AddToParent(View view);
	}
}
