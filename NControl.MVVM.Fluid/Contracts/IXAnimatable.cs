using System;
using System.Collections.Generic;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public interface IXAnimatable
	{
		IEnumerable<XAnimationPackage> TransitionIn(INavigationContainer fromContainer, PresentationMode presentationMode);		
		IEnumerable<XAnimationPackage> TransitionOut(View view, View toView, PresentationMode presentationMode);
	}
}
