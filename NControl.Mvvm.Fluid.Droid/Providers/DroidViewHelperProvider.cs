using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace NControl.Mvvm.Droid
{
	public class DroidViewHelperProvider: IViewHelperProvider
	{
		public Point GetLocalLocation(VisualElement element, Point point)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
				return Point.Zero;

			var nativeView = renderer.ViewGroup;
			var rootView = nativeView.RootView;

			var fromCoord = new int[2];
			var toCoord = new int[2];
			rootView.GetLocationOnScreen(fromCoord);
			nativeView.GetLocationOnScreen(toCoord);

			var toPoint = new Point(Forms.Context.FromPixels(fromCoord[0] - toCoord[0] + point.X),
			                        Forms.Context.FromPixels(fromCoord[1] - toCoord[1] + point.Y));

			return toPoint;
		}



		public Point GetLocationOnScreen(VisualElement element)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
				return Point.Zero;

			var nativeView = renderer.ViewGroup;
			var rect = new int[2];
			nativeView.GetLocationOnScreen(rect);
			return new Point(
				Forms.Context.FromPixels(rect[0]), Forms.Context.FromPixels(rect[1]));
		}
	}
}
