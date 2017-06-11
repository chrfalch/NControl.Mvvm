using System;
using NControl.Mvvm;
using NControl.Mvvm.iOS;
using NControl.XAnimation.iOS;

namespace MvvmDemo.iOS
{
	public class DemoFluidTouchPlatform : FluidTouchPlatform
	{
		public override void RegisterActivityIndicator()
		{
			Container.RegisterSingleton<IActivityIndicator,
				FluidActivityIndicatorProvider<DemoActivityIndicator>>();
		}
	}
}
