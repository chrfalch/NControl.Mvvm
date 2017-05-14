using System;
using System.Collections.Generic;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface IXAnimatable
	{
		IEnumerable<IXAnimation> TransitionIn(INavigationContainer fromContainer, PresentationMode presentationMode);		
		IEnumerable<IXAnimation> TransitionOut(INavigationContainer toContainer, PresentationMode presentationMode);
	}
}
