using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface IActivityIndicatorViewProvider
	{
		void RemoveFromParent(View view);
		void AddToParent(View view);
	}
}
