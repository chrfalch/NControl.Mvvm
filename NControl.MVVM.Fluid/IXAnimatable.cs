using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public interface IXAnimatable
	{
		IEnumerable<XAnimation.XAnimation> TransitionIn(View fromView, View overlay, PresentationMode presentationMode);
		IEnumerable<XAnimation.XAnimation> TransitionOut(View toView, View overlay, PresentationMode presentationMode);
	}
}
