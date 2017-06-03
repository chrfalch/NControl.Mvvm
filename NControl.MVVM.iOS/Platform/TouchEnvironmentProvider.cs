using System;
using Foundation;
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

		public Xamarin.Forms.Rectangle GetLocationOnScreen(Xamarin.Forms.VisualElement element)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer == null)
				return Xamarin.Forms.Rectangle.Zero;

			var nativeView = renderer.NativeView;
			var frameOnScreen = nativeView.ConvertRectToView(nativeView.Frame, null);
			return new Xamarin.Forms.Rectangle(frameOnScreen.X, frameOnScreen.Y, frameOnScreen.Width, frameOnScreen.Height);
		}
	}
}
