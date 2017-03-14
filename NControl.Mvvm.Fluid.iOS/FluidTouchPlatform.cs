using System;
using NControl.Mvvm;
using NControl.Mvvm.iOS;

namespace NControl.Mvvm.Fluid.iOS
{
	public class FluidTouchPlatform: TouchPlatform
	{
		public FluidTouchPlatform()
		{
			var animDummy = new NControl.XAnimation.iOS.TouchXAnimationProvider();
		}

		public override void RegisterActivityIndicator()
		{
			Container.RegisterSingleton<IActivityIndicator, FluidActivityIndicatorView>();
		}
	}
}
