using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NControl.Mvvm.Droid
{
	public class DroidViewHelperProvider: IViewHelperProvider
	{
		IEnvironmentProvider _environmentProvider;
		public DroidViewHelperProvider(IEnvironmentProvider environmentProvider)
		{
			_environmentProvider = environmentProvider;
		}

		public Point GetLocalLocation(VisualElement element, Point point)
		{
			// Get native view
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
				return Point.Zero;
			
			// Convert to Android 
			var screenX = Forms.Context.ToPixels(point.X);
			var screenY = Forms.Context.ToPixels(point.Y);

			// Convert to position in parent view
			var parentView = renderer.ViewGroup.Parent as Android.Views.ViewGroup;
			parentView.SetClipChildren(false);

			while (parentView != null && !(parentView is PageRenderer))
			{
				screenX -= parentView.Left;
				screenY -= parentView.Top;
				parentView = parentView.Parent as Android.Views.ViewGroup;
			}

			var myPt = new Point(Forms.Context.FromPixels(screenX),
							 Forms.Context.FromPixels(screenY));

			//var elementsRect = GetLocationOnScreen(element);
			//var pt = new Point(Forms.Context.FromPixels(elementsRect.X),	                                           
			//                   Forms.Context.FromPixels(elementsRect.Y));

			//var anpt = new Point(point.X - pt.X, point.Y - pt.Y);

			return myPt;

			//var location = new int[2];
			//renderer.ViewGroup.GetLocationOnScreen(location);
			//var screenX = Forms.Context.ToPixels(point.X);
			//var screenY = Forms.Context.ToPixels(point.Y);

			//var viewX = screenX - location[0];
			//var viewY = screenY - location[1];

			//return new Point(Forms.Context.FromPixels(viewX), 
			//                 Forms.Context.FromPixels(viewY));

			//var nativeView = renderer.ViewGroup;
			//var rootView = nativeView.RootView as Android.Views.ViewGroup;

			//var fromCoord = new int[2];
			//var toCoord = new int[2];
			//rootView.GetLocationOnScreen(fromCoord);
			//nativeView.GetLocationOnScreen(toCoord);

			//var toPoint = new Point(
			//	((fromCoord[0] - toCoord[0]) / _environmentProvider.DisplayDensity) + point.X,
			//	((fromCoord[1] - toCoord[1]) / _environmentProvider.DisplayDensity) + point.Y);
			
			//return toPoint;
		}

		public Point GetLocationOnScreen(VisualElement element)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
				return Point.Zero;

			// Convert to Android 
			var viewX = Forms.Context.ToPixels(element.X);
			var viewY = Forms.Context.ToPixels(element.Y);

			// Convert to position in parent view
			var parentView = renderer.ViewGroup.Parent as Android.Views.ViewGroup;

			while (parentView != null && !(parentView is PageRenderer))
			{
				viewX += parentView.Left;
				viewY += parentView.Top;

				parentView = parentView.Parent as Android.Views.ViewGroup;
			}

			var myPt = new Point(Forms.Context.FromPixels(viewX),
							 Forms.Context.FromPixels(viewY));

			//var nativeView = renderer.ViewGroup;
			//var rect = new int[2];
			//nativeView.GetLocationOnScreen(rect);
			//var anpt = new Point(Forms.Context.FromPixels(rect[0]), 			                 
			//                     Forms.Context.FromPixels(rect[1]));
			//if (!myPt.Equals(anpt))
			//	System.Diagnostics.Debugger.Break();

			return myPt;

		}
	}
}
