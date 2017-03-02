using System;
using Xamarin.Forms;
using UIKit;
using System.Linq;
using Xamarin.Forms.Platform.iOS;

namespace NControl.Mvvm.iOS
{

	public class TouchGestureRecognizer : UIGestureRecognizer
	{
		public event EventHandler<GestureRecognizerEventArgs> Touched;

		public override void Reset()
		{
			base.Reset();
		}

		public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Start));
		}

		public override void TouchesMoved(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);

			Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Moving));
		}

		public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Ended));

			base.State = UIGestureRecognizerState.Recognized;
		}

		public override void TouchesCancelled(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);

			Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Cancelled));

			// we fail the recognizer so that there isn't unexpected behavior
			// if the application comes back into view
			base.State = UIGestureRecognizerState.Failed;
		}
	}
}
