using System;
using System.Collections.Generic;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public interface IXAnimatable
	{
		IEnumerable<XAnimationPackage> TransitionIn(View view, PresentationMode presentationMode);		
		IEnumerable<XAnimationPackage> TransitionOut(View view, PresentationMode presentationMode);
	}
}
