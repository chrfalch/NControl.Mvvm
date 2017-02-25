using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public static class BehaviorExtensions
	{
		public static T AddBehaviorTo<T>(this T view, Behavior behavior) where T : View
		{
			view.Behaviors.Add(behavior);
			return view;
		}
	}
}
