using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.Linq;
using Android.Views;
using Android.Animation;
using Android.App;
using NControl.Mvvm;
using NControl.Mvvm.Droid;

// [assembly: ExportRenderer(typeof(FluidNavigationContainer), typeof(FluidNavigationContainerRenderer))]
namespace NControl.Mvvm.Droid
{
	public class FluidNavigationContainerRenderer: ViewRenderer<FluidNavigationContainer, Android.Views.View>
	{
		#region Private Members
		float _displayDensity = 1.0f;
		bool _isScrolling;
		float _startX;
		float _startY;
		int _touchSlop;
		VelocityTracker _velocityTracker;
		#endregion

		public FluidNavigationContainerRenderer()
		{
			var vc = ViewConfiguration.Get(Forms.Context);
			_touchSlop = vc.ScaledTouchSlop;

			// Get display density to fix pixel scaling
			using (var metrics = Forms.Context.Resources.DisplayMetrics)
				_displayDensity = metrics.Density;
		}

		public override bool OnInterceptTouchEvent(MotionEvent ev)
		{
			if (!Element.BackButtonVisible)
				return false;

			var action = ev.Action;

			if (action == MotionEventActions.Down)
			{
				// Save starting point
				_startX = ev.GetX();
				_startY = ev.GetY();
				return false;
			}

			if(action == MotionEventActions.Move)
			{
				if (_isScrolling)
				{
					// We're currently scrolling, so yes, intercept the
					// touch event
					return true;
				}

				// If the user has dragged her finger horizontally more than
				// the touch slop, start the scroll
				var distanceX = Math.Abs(ev.GetX() - _startX);
				var distanceY = Math.Abs(ev.GetY() - _startY);
				if (distanceX > distanceY && distanceX > _touchSlop * _displayDensity)
				{
					_isScrolling = true;
					_velocityTracker = VelocityTracker.Obtain();

					//Element.UpdateFromGestureRecognizer(
					//	ev.GetX() / _displayDensity, -1, PanState.Started);
					
					return true;
				}
        	}

			return false;
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (_isScrolling)
			{
				var action = e.Action;

				if (action == MotionEventActions.Cancel || action == MotionEventActions.Up)
				{
					// Release the scroll.
					_isScrolling = false;

					_velocityTracker.ComputeCurrentVelocity(1000);
					var velX = _velocityTracker.XVelocity;
					_velocityTracker.Recycle();
					_velocityTracker = null;

					//Element.UpdateFromGestureRecognizer(
					//	e.GetX() / _displayDensity, velX, PanState.Ended); 
					
					return true;
				}

				if (action == MotionEventActions.Move)
				{
					// Add to velocity tracker
					_velocityTracker.AddMovement(e);

					// Update element
					//Element.UpdateFromGestureRecognizer(
					//	e.GetX() / _displayDensity, -1, PanState.Moving);
				}
			}
			
			return base.OnTouchEvent(e);
		}
	}
}
