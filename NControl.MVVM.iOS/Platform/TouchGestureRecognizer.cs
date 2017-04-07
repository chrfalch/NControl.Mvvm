using System;
using Xamarin.Forms;
using UIKit;
using System.Linq;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using CoreAnimation;
using Foundation;
using System.Collections.Generic;

namespace NControl.Mvvm.iOS
{

	public class TouchGestureRecognizer : UIGestureRecognizer
	{
		public event EventHandler<GestureRecognizerEventArgs> Touched;
		double _lastTimeStamp;
		CGPoint _lastPosition;

		public override void Reset()
		{
			base.Reset();
			_lastTimeStamp = 0;
			_lastPosition = CGPoint.Empty;
		}

		public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);

			_lastTimeStamp = CAAnimation.CurrentMediaTime();
			_lastPosition = ((UITouch)touches.FirstOrDefault()).LocationInView(View);

			Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Start, GetTouchPoints(touches), -1));
		}

		public override void TouchesMoved(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesMoved(touches, evt);

			Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Moving, GetTouchPoints(touches), GetVelocity(touches)));
		}

		public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded(touches, evt);

			Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Ended, GetTouchPoints(touches), GetVelocity(touches)));

			base.State = UIGestureRecognizerState.Failed;
		}

		public override void TouchesCancelled(Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled(touches, evt);

			Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Cancelled, GetTouchPoints(touches), GetVelocity(touches)));

			// we fail the recognizer so that there isn't unexpected behavior
			// if the application comes back into view
			base.State = UIGestureRecognizerState.Failed;
		}

		double GetVelocity(NSSet touches)
		{
			var currentTime = CAAnimation.CurrentMediaTime();
			var elapsedTime = currentTime - _lastTimeStamp;
			_lastTimeStamp = currentTime;

			var location = ((UITouch)touches.FirstOrDefault()).LocationInView(View);

			var dx = location.X - _lastPosition.X;
			var dy = location.Y - _lastPosition.Y;

			var path_travelled = Math.Sqrt(dx * dx + dy * dy);
			var velocity = path_travelled / elapsedTime;

			_lastPosition = location;

			return velocity;
		}

		IEnumerable<Point> GetTouchPoints(Foundation.NSSet touches)
		{
			var points = new List<Point>((int)touches.Count);
			foreach (UITouch touch in touches)
			{
				CGPoint touchPoint = touch.LocationInView(View);
				points.Add(new Point(touchPoint.X, touchPoint.Y));
			}

			return points;
		}	
	}
}
