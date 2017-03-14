using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class BaseViewBehavior: Behavior<View>
	{
		public void AttachTo(View bindable)
		{
			OnAttachedTo(bindable);
		}

		public void DetachingFrom(View bindable)
		{
			OnDetachingFrom(bindable);
		}
	}
}
