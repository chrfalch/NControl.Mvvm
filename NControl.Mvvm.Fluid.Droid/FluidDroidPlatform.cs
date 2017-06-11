using System;
using Android.App;
using NControl.Mvvm;
using NControl.Mvvm.Droid;

namespace NControl.Mvvm.Droid
{
	/// <summary>
	/// Droid mvvm app.
	/// </summary>
	public class FluidDroidPlatform : DroidPlatform
	{
		public FluidDroidPlatform(Activity activity) : base(activity)
		{
			using(PerformanceTimer.Current.BeginTimer(this))
				XAnimation.Droid.XAnimation.Init();
		}

		public override void Initialize()
		{
			base.Initialize();
			Container.Register<IViewHelperProvider, DroidViewHelperProvider>();
		}

		public override void RegisterActivityIndicator()
		{
			using(PerformanceTimer.Current.BeginTimer(this))
				Container.RegisterSingleton<IActivityIndicator, 
				FluidActivityIndicatorView<FluidActivityIndicator>>();
		}
	}
}
