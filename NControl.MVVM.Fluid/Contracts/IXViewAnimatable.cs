using System;
using System.Collections.Generic;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface IXViewAnimatable
	{
		IEnumerable<XAnimationPackage> TransitionIn(INavigationContainer fromContainer, INavigationContainer toContainer, IEnumerable<XAnimationPackage> animations, PresentationMode presentationMode);
		IEnumerable<XAnimationPackage> TransitionOut(View view, INavigationContainer container, IEnumerable<XAnimationPackage> animations, PresentationMode presentationMode);
	}
}
