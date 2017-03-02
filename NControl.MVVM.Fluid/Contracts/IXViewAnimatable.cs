using System;
using System.Collections.Generic;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public interface IXViewAnimatable
	{
		IEnumerable<XAnimationPackage> TransitionIn(View view, INavigationContainer container, IEnumerable<XAnimationPackage> animations, PresentationMode presentationMode);
		IEnumerable<XAnimationPackage> TransitionOut(View view, INavigationContainer container, IEnumerable<XAnimationPackage> animations, PresentationMode presentationMode);
	}
}
