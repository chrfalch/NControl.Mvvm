using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace NControl.Mvvm.iOS
{
	public class TouchEnvironmentProvider: IEnvironmentProvider
	{
		public TouchEnvironmentProvider()
		{
		}

		public string AppVersion
		{
			get
			{
				return NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleShortVersionString")] +
					"." + NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersion")];
			}
		}

		public float DisplayDensity
		{
			get
			{
				return 1.0f;
			}
		}

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
