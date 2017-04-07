using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NControl.Mvvm.Droid
{
	public class DroidGestureRecognizerProvider : IGestureRecognizerProvider
	{
		VisualElement _element;
		ViewGroup _view;

		double _lastTimeStamp;
		Point _lastPosition;

		public event EventHandler<GestureRecognizerEventArgs> Touched;

		public void AttachTo(VisualElement element)
		{
			_element = element;
			element.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "Renderer")
				{
					var r = Platform.GetRenderer(element);
					if (r != null)
					{
						r.ViewGroup.Touch += ViewGroup_Touch;
						_view = r.ViewGroup;
					}
				}
			};
		}

		public void DetachFrom(VisualElement element)
		{
			_element = null;
			_view = null;

			var r = Platform.GetRenderer(element);
			if (r != null)
				r.ViewGroup.Touch -= ViewGroup_Touch;
		}

		void ViewGroup_Touch(object sender, Android.Views.View.TouchEventArgs e)
		{

			var scale = _element.Width / _view.Width;

			var touches = new List<Point>();
			for (var i = 0; i < e.Event.PointerCount; i++)
			{
				var coord = new MotionEvent.PointerCoords();
				e.Event.GetPointerCoords(i, coord);
				touches.Add(new Point(coord.X * scale, coord.Y * scale));
			}

			switch (e.Event.Action)
			{
				case MotionEventActions.Down:

					_lastTimeStamp = Java.Lang.JavaSystem.CurrentTimeMillis();
					_lastPosition = touches.FirstOrDefault();

					Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Start, touches, GetVelocity(touches)));
					break;

				case MotionEventActions.Move:
					Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Moving, touches, GetVelocity(touches)));
					break;

				case MotionEventActions.Up:
					Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Ended, touches, GetVelocity(touches)));
					break;

				case MotionEventActions.Cancel:
					Touched?.Invoke(this, new GestureRecognizerEventArgs(TouchType.Cancelled, touches, GetVelocity(touches)));
					break;
			}
		}

		double GetVelocity(IEnumerable<Point> touches)
		{
			var currentTime = Java.Lang.JavaSystem.CurrentTimeMillis();
			var elapsedTime = currentTime - _lastTimeStamp;
			_lastTimeStamp = currentTime;

			var location = touches.FirstOrDefault();

			var dx = location.X - _lastPosition.X;
			var dy = location.Y - _lastPosition.Y;

			var path_travelled = Math.Sqrt(dx * dx + dy * dy);
			var velocity = path_travelled / elapsedTime;

			_lastPosition = location;

			return velocity;
		}
	}
}
