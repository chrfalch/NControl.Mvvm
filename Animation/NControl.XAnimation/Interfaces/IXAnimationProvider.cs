using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NControl.XAnimation
{

	public interface IXAnimationProvider
	{
        void Initialize(XTransformationContainer transformationContainer);
		bool GetHasViewsToAnimate(XTransform animationinfo);
		void Animate(XTransform animationInfo, Action completed, long duration, EasingFunctionBezier easing);
		void Set(XTransform animationInfo);
		void Set(VisualElement element, XTransform animationInfo);
	}
}
