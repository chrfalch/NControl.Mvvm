using System;
using Android.App;
using NControl.Mvvm;
using NControl.Mvvm.Droid;

namespace NControl.Mvvm.Fluid.Droid
{
	/// <summary>
	/// Droid mvvm app.
	/// </summary>
	public class FluidDroidPlatform : DroidPlatform
	{
		public FluidDroidPlatform(Activity activity) : base(activity)
		{
			var animDummy = new NControl.XAnimation.Droid.DroidXAnimationProvider();
		}
	}
}
