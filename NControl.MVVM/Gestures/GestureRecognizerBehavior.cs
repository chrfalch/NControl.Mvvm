using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class GestureRecognizerBehavior: Behavior<View>
	{
		readonly IGestureRecognizerProvider _provider;

		/// <summary>
		/// Occurs when touched.
		/// </summary>
		public event EventHandler<GestureRecognizerEventArgs> Touched
		{
			add { _provider.Touched += value; }
			remove { _provider.Touched -= value; }
		}

		public GestureRecognizerBehavior()
		{
			_provider = Container.Resolve<IGestureRecognizerProvider>();
		}

		protected override void OnAttachedTo(View bindable)
		{
			base.OnAttachedTo(bindable);
			_provider.AttachTo(bindable);
		}

		protected override void OnDetachingFrom(View bindable)
		{
			base.OnDetachingFrom(bindable);
			_provider.DetachFrom(bindable);
		}

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
