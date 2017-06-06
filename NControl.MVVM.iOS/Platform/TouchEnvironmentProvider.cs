using System;
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

		public Rectangle GetLocationOnScreen(VisualElement element)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
				return Rectangle.Zero;
			
			var nativeView = renderer.NativeView;
			var frameOnScreen = nativeView.ConvertRectToView(nativeView.Bounds, null);
			return new Rectangle(frameOnScreen.X, frameOnScreen.Y, frameOnScreen.Width, frameOnScreen.Height);
		}

		public Rectangle GetLocalLocation(VisualElement element, Rectangle rect)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
				return Rectangle.Zero;

			var nativeView = renderer.NativeView;
			var locationForView = UIApplication.SharedApplication.KeyWindow.ConvertPointToView(
				new CGPoint(rect.X, rect.Y), nativeView);

			return new Rectangle(locationForView.X, locationForView.Y, rect.Width, rect.Height);
		}
	}
}
