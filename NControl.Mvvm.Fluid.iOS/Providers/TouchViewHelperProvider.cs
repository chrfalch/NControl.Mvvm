using System;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace NControl.Mvvm.iOS
{
	public class TouchViewHelperProvider: IViewHelperProvider
	{
		public Point GetLocationOnScreen(VisualElement element)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
			return Point.Zero;

			var nativeView = renderer.NativeView;
			var pt = UIApplication.SharedApplication.KeyWindow.ConvertPointFromView(
				nativeView.Frame.Location, nativeView);

			return new Point(pt.X, pt.Y);
		}

		public Point GetLocalLocation(VisualElement element, Point point)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
				return Point.Zero;

			var nativeView = renderer.NativeView;
			var cgpt = new CGPoint(point.X, point.Y);
			var pt = UIApplication.SharedApplication.KeyWindow.ConvertPointToView(
				cgpt, nativeView);

			return new Point(pt.X, pt.Y);
		}
	}
}
