using System;
using Xamarin.Forms;
using NControl.XAnimation;

namespace NControl.Mvvm
{
	public interface IXViewTransitionable
	{
		XInterpolationPackage OverrideTransition(
			string transitionIdentifier, VisualElement fromElement, VisualElement toElement, Rectangle fromRect, 
			Rectangle toRectangle, XInterpolationPackage package);
	}
}
