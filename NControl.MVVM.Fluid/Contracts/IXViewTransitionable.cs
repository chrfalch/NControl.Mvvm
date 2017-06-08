using System;
using Xamarin.Forms;
using NControl.XAnimation;
using System.Collections.Generic;

namespace NControl.Mvvm
{
	public interface IXViewTransitionable
	{
		IEnumerable<XInterpolationPackage> OverrideTransition(
			string transitionIdentifier, VisualElement fromElement, VisualElement toElement, Rectangle fromRect, 
			Rectangle toRectangle, XInterpolationPackage package);
	}
}
