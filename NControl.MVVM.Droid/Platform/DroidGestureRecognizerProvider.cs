using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.Runtime;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NControl.Mvvm.Droid
{
	public class DroidGestureRecognizerProvider : IGestureRecognizerProvider
	{
		#region Members

		VisualElement _element;
		ViewGroup _view;
		DroidTouchInterceptor _touchInterceptor;
		float _displayDensity = 1.0f;

		#endregion

		#region Events

		/// <summary>
		/// Occurs when touched.
		/// </summary>
		public event EventHandler<GestureRecognizerEventArgs> Touched;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NControl.Mvvm.Droid.DroidGestureRecognizerProvider"/> class.
		/// </summary>
		public DroidGestureRecognizerProvider()
		{
			// Get display density to fix pixel scaling
			using (var metrics = Forms.Context.Resources.DisplayMetrics)
				_displayDensity = metrics.Density;
		}

		#region IGestureRecognizerProvider Implementation 

		/// <summary>
		/// Attachs to an element.
		/// </summary>
		public void AttachTo(VisualElement element)
		{
			_element = element;
			element.PropertyChanged += Element_PropertyChanged;
		}

		/// <summary>
		/// Detachs from.
		/// </summary>
		/// <param name="element">Element.</param>
		public void DetachFrom(VisualElement element)
		{
			if (_view != null && _touchInterceptor != null)
			{
				_touchInterceptor.Touched -= Interceptor_Touched;
				_view.RemoveView(_touchInterceptor);
			}

			if (element != null)
			{
				element.PropertyChanged -= Element_PropertyChanged;
				element = null;
			}

			_touchInterceptor = null;
			_view = null;
		}

		#endregion

		#region Private Members

		/// <summary>
		/// Touch event handler for the interceptor.
		/// </summary>
		void Interceptor_Touched(object sender, GestureRecognizerEventArgs e)
		{
			Touched?.Invoke(this, e);
		}

		/// <summary>
		/// Handles property changes in the element.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Renderer")
			{
				var r = Platform.GetRenderer(_element);
				if (r != null && _touchInterceptor == null)
				{
					_view = r.ViewGroup;
					_touchInterceptor = new DroidTouchInterceptor(_displayDensity);
					_touchInterceptor.Touched += Interceptor_Touched;
					_view.AddView(_touchInterceptor);
				}
			}
			else if (e.PropertyName == nameof(VisualElement.Width) ||
				e.PropertyName == nameof(VisualElement.Height))
			{
				if (_touchInterceptor != null && 
				    !_element.Width.Equals(-1) && 
				    !_element.Height.Equals(-1))

					_touchInterceptor.Layout(0, 0, (int)(_displayDensity * _element.Width), 
					                         (int)(_displayDensity * _element.Height));		
				
			}
		}
		#endregion

	}

	/// <summary>
	/// Droid touch interceptor class.
	/// </summary>
	class DroidTouchInterceptor : ViewGroup
	{
		double _lastTimeStamp;
		Point _lastPosition;
		float _displayDensity = 1.0f;
		bool _isTracking;

		public event EventHandler<GestureRecognizerEventArgs> Touched;

		public DroidTouchInterceptor(float displayDensity): base(Forms.Context)
		{
			_displayDensity = displayDensity;
			// SetBackgroundColor(Android.Graphics.Color.Lime);
		}

		public override bool OnInterceptTouchEvent(MotionEvent ev)
		{
			var touches = GetTouches(ev);

			if (ev.Action == MotionEventActions.Down)
			{
				_lastTimeStamp = Java.Lang.JavaSystem.CurrentTimeMillis();
				_lastPosition = touches.FirstOrDefault();

				var args = new GestureRecognizerEventArgs(TouchType.Start, touches, GetVelocity(touches));
				Touched?.Invoke(this, args);
				_isTracking = !args.Cancel;
			}						

			return _isTracking;
		}

		/// <summary>
		/// Handles touches 
		/// </summary>
		public override bool OnTouchEvent(MotionEvent e)
		{
			if (_isTracking)
			{
				if (e.Action == MotionEventActions.Move)
				{
					var touches = GetTouches(e);
					var args = new GestureRecognizerEventArgs(TouchType.Moving, touches, GetVelocity(touches));
					Touched?.Invoke(this, args);
					_isTracking = !args.Cancel;
				}
				// Handle cancel or up
				else if (e.Action == MotionEventActions.Cancel || e.Action == MotionEventActions.Up)
				{
					var touches = GetTouches(e);
					Touched?.Invoke(this, new GestureRecognizerEventArgs(
						e.Action == MotionEventActions.Cancel? TouchType.Cancelled : TouchType.Ended,
						touches, GetVelocity(touches)));
					
					_isTracking = false;
				}

				return _isTracking;
			}

			return false;
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{			
		}

		IEnumerable<Point> GetTouches(MotionEvent ev)
		{
			var touches = new List<Point>();
			for (var i = 0; i<ev.PointerCount; i++)
			{
				var coord = new MotionEvent.PointerCoords();
				ev.GetPointerCoords(i, coord);
				touches.Add(new Point(coord.X / _displayDensity, coord.Y / _displayDensity));
			}

			return touches;
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
			var velocity = path_travelled * _displayDensity / elapsedTime;

			_lastPosition = location;

			return velocity * 1000;
		}
	}
}
