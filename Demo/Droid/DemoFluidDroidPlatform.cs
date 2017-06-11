using System;
using Android.App;
using NControl.Mvvm;
using NControl.Mvvm.Droid;

namespace MvvmDemo.Droid
{
	public class DemoFluidDroidPlatform: FluidDroidPlatform
	{
		public DemoFluidDroidPlatform(Activity activity) : base(activity)
		{			
		}

		public override void RegisterActivityIndicator()
		{
			Container.RegisterSingleton<IActivityIndicator,
				FluidActivityIndicatorProvider<DemoActivityIndicator>>();
		}
	}
}
