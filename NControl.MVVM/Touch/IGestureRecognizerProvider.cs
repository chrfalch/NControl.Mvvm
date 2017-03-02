using System;
using System.Collections.Generic;
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

		public GestureRecognizerEventArgs(TouchType touchType)
		{			
			TouchType = touchType;
		}
	}
}
