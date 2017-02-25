using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public static class GestureRecognizerExtensions
	{
		public static T AddGestureRecognizerTo<T>(this T view, IGestureRecognizer recognizer) where T : View
		{
			view.GestureRecognizers.Add(recognizer);
			return view;
		}
	}
}
