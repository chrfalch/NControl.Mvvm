using System;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NControl.Mvvm.Droid
{
	public class DroidGestureRecognizerProvider: IGestureRecognizerProvider
	{
		public event EventHandler<GestureRecognizerEventArgs> Touched;

		public void AttachTo(VisualElement element)
		{
			element.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "Renderer")
				{
					var r = Platform.GetRenderer(element);
					if (r != null)
					{
						r.ViewGroup.Touch += ViewGroup_Touch;
					}
				}
			};
		}

		public void DetachFrom(VisualElement element)
		{
			var r = Platform.GetRenderer(element);
			if (r != null)
				r.ViewGroup.Touch -= ViewGroup_Touch;			
		}

		void ViewGroup_Touch(object sender, Android.Views.View.TouchEventArgs e)
		{
			switch (e.Event.Action)
			{
				case MotionEventActions.Down:
					Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Start));
					break;

				case MotionEventActions.Move:
					Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Moving));
					break;

				case MotionEventActions.Up:
					Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Ended));
					break;

				case MotionEventActions.Cancel:
					Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Cancelled));
					break;
			}
		}
	}
}
