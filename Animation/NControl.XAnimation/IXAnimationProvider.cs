using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NControl.XAnimation
{

	public interface IXAnimationProvider
	{
		void Initialize(XAnimation animation);
		void Animate(XAnimationInfo animationInfo, Action completed);
		void Set(XAnimationInfo animationInfo);
	}
}
