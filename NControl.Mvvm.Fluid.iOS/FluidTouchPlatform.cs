using System;
using NControl.Mvvm;
using NControl.Mvvm.iOS;
using NControl.XAnimation.iOS;

namespace NControl.Mvvm.iOS
{
	public class FluidTouchPlatform: TouchPlatform
	{
		public FluidTouchPlatform()
		{
			XAnimation.iOS.XAnimation.Init();
		}
	
		public override void RegisterActivityIndicator()
		{
			Container.RegisterSingleton<IActivityIndicator, FluidActivityIndicatorView>();
		}
	}
}
