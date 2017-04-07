using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public enum TouchType
	{
		Start,
		Moving,
		Ended,
		Cancelled,
	}

	public interface IGestureRecognizerProvider
	{
		event EventHandler<GestureRecognizerEventArgs> Touched;

		void AttachTo(VisualElement element);
		void DetachFrom(VisualElement element);
	}

	public class GestureRecognizerEventArgs : EventArgs
	{
		public TouchType TouchType { get; private set; }
		public IEnumerable<Point> Points { get; private set; }

		/// <summary>
		/// Units moved pr milliseconds
		/// </summary>
		/// <value>The velocity.</value>
		public double Velocity { get; private set; }

		public GestureRecognizerEventArgs(TouchType touchType, IEnumerable<Point> points, double velocity)
		{
			TouchType = touchType;
			Points = points;
			Velocity = velocity;
		}

		public Point FirstPoint { get { return Points.FirstOrDefault(); } }	
	}
}
