using System;
using System.Collections.Generic;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface IXViewAnimatable
	{
		IEnumerable<IXAnimation> TransitionIn(INavigationContainer fromContainer, 
                                            INavigationContainer toContainer, 
                                            IEnumerable<IXAnimation> animations, 
                                            PresentationMode presentationMode);
		
		IEnumerable<IXAnimation> TransitionOut(INavigationContainer toContainer, 
		                                             INavigationContainer fromContainer, 
		                                             IEnumerable<IXAnimation> animations, 
		                                             PresentationMode presentationMode);
	}
}
