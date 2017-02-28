using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public interface IXAnimatable
	{
		IEnumerable<XAnimation.XAnimationPackage> TransitionIn(View view, PresentationMode presentationMode);		
		IEnumerable<XAnimation.XAnimationPackage> TransitionOut(View view, PresentationMode presentationMode);
	}
}
