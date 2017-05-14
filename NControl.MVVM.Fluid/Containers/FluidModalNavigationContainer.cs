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

		public override IEnumerable<IXAnimation> TransitionIn(
			INavigationContainer fromContainer, PresentationMode presentationMode)
		{
			var retVal = new List<IXAnimation>(base.TransitionIn(fromContainer, presentationMode));
			retVal.Add(new XAnimationPackage(_overlay)
			           .Opacity(0.0)
			           .Set()
			           .Opacity(1.0)
			           .Then());
			
			return retVal;
		}

		public override IEnumerable<IXAnimation> TransitionOut(
			INavigationContainer toContainer, PresentationMode presentationMode)
		{
			var retVal = new List<IXAnimation>(base.TransitionOut(toContainer, presentationMode));
			retVal.Add(new XAnimationPackage(_overlay)
			           .Opacity(0.0)
			           .Then());
			
			return retVal;
		}
	}
}
