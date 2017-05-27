using System;
using System.Collections.Generic;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface IXViewAnimatable
	{
		IEnumerable<XTransform> TransitionIn(INavigationContainer fromContainer, 
                                            INavigationContainer toContainer, 
                                            IEnumerable<XTransform> animations, 
                                            PresentationMode presentationMode);
		
		IEnumerable<XTransform> TransitionOut(INavigationContainer toContainer, 
		                                             INavigationContainer fromContainer, 
		                                             IEnumerable<XTransform> animations, 
		                                             PresentationMode presentationMode);
	}
}
