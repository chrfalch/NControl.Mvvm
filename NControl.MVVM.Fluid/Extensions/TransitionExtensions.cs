using System;
using Xamarin.Forms;
namespace NControl.Mvvm
{
	public static class TransitionExtensions
	{
		public static View SetTransitionIdentifier(this View view, string identifier)
		{
			view.SetValue(TransitionIdentifierProperty, identifier);	
			return view;	
		}

		public static string GetTransitionIdentifier(this View view)
		{
			return (string)view.GetValue(TransitionIdentifierProperty);
		}

		public static BindableProperty TransitionIdentifierProperty = BindableProperty.Create(
			"TransitionIdentifier", typeof(string), typeof(TransitionExtensions));
	}
}
