using System;
using System.Collections.Generic;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface IXAnimatable
	{
		IEnumerable<XTransform> TransitionIn(INavigationContainer fromContainer, PresentationMode presentationMode);		
		IEnumerable<XTransform> TransitionOut(INavigationContainer toContainer, PresentationMode presentationMode);
	}
}
