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

		public override View GetOverlayView()
		{
			if (_overlay == null)
			{
				_overlay = new BoxView
				{
					BackgroundColor = Config.ViewTransparentBackgroundColor,
				};
			}

			return _overlay;
		}

		public override IEnumerable<XAnimationPackage> TransitionIn(
			INavigationContainer fromContainer, PresentationMode presentationMode)
		{
			// Copy from base
			var retVal = new List<XAnimationPackage>(
				base.TransitionIn(fromContainer, presentationMode));
			
			var animation = new XAnimationPackage(_overlay);
			animation.Set((transform) => transform.SetOpacity(0.0));
			animation.Add((transform) => transform.SetOpacity(1.0));
			retVal.Add(animation);
			
			return retVal;
		}

		public override IEnumerable<XAnimationPackage> TransitionOut(
			INavigationContainer toContainer, PresentationMode presentationMode)
		{
			var retVal = new List<XAnimationPackage>(
				base.TransitionOut(toContainer, presentationMode));

			var animation = new XAnimationPackage(_overlay);
			animation.Add((transform) => transform.SetOpacity(0.0));
			retVal.Add(animation);
			
			return retVal;
		}
	}
}
