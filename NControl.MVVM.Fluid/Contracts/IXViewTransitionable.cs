using System;
using Xamarin.Forms;
using NControl.XAnimation;
using System.Collections.Generic;

namespace NControl.Mvvm
{
	public interface IXViewTransitionable
	{
		void BeforeOverrideTransition(
			string transitionIdentifier, VisualElement fromElement, VisualElement toElement, 
			ref Rectangle fromRect, ref Rectangle toRectangle);
		
		IEnumerable<XInterpolationPackage> OverrideTransition(
			string transitionIdentifier, VisualElement fromElement, VisualElement toElement, 
			Rectangle fromRect, Rectangle toRectangle, IEnumerable<XInterpolationPackage> packages);
	}
}
