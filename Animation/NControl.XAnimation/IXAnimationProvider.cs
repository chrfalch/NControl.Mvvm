using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NControl.XAnimation
{

	public interface IXAnimationProvider
	{
		void Initialize(XAnimationPackage animation);
		bool GetHasViewsToAnimate(XAnimationInfo animationinfo);
		void Animate(XAnimationInfo animationInfo, Action completed);
		void Set(XAnimationInfo animationInfo);
		void Set(VisualElement element, XAnimationInfo animationInfo);
	}
}
