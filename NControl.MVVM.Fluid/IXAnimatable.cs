using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public interface IXAnimatable
	{
		IEnumerable<XAnimation.XAnimation> TransitionIn(View view, PresentationMode presentationMode);		
		IEnumerable<XAnimation.XAnimation> TransitionOut(View view, PresentationMode presentationMode);
	}
}
