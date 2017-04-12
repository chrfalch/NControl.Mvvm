using System;
using System.Collections.Generic;
using System.Linq;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidModalNavigationContainer: FluidNavigationContainer
	{
		View _overlay;

		protected override void AddViewsToBottomOfStack(RelativeLayout layout)
		{
			base.AddViewsToBottomOfStack(layout);

			_overlay = new BoxView
			{
				BackgroundColor = MvvmApp.Current.Colors.Get(Config.ViewTransparentBackgroundColor),
			};

			layout.Children.Add(_overlay, () => layout.Bounds);
		}

		public override View GetOverlayView()
		{
			return _overlay;
		}

		//public override IEnumerable<XAnimationPackage> TransitionIn(
		//	INavigationContainer fromContainer, PresentationMode presentationMode)
		//{
		//	var retVal = new List<XAnimationPackage>(base.TransitionIn(fromContainer, presentationMode));
		//	retVal.Add(new XAnimationPackage(_overlay).Opacity(0.0).Set().Opacity(1.0).Animate());
		//	return retVal;
		//}

		//public override IEnumerable<XAnimationPackage> TransitionOut(
		//	INavigationContainer toContainer, PresentationMode presentationMode)
		//{
		//	// TODO: This is a hack - dont know why...
		//	_navigationContainer.IsVisible = false;
		//	var retVal = new List<XAnimationPackage>(base.TransitionOut(toContainer, presentationMode));
		//	retVal.Add(new XAnimationPackage(_overlay).Opacity(0.0).Animate());
		//	return retVal;
		//}
	}
}
