using System;
using NControl.Mvvm;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public abstract class FluidMvvmApp: MvvmApp
	{
		public FluidMvvmApp(IMvvmPlatform platform):base(platform)
		{
		}

		/// <summary>
		/// Implement to provide type information about main view
		/// </summary>
		public abstract Type GetMainViewType();

		/// <summary>
		/// Register presenter
		/// </summary>
		protected override void RegisterPresenter()
		{
			Container.RegisterSingleton<IPresenter, FluidPresenter>();
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		protected override Page GetMainPage()
		{
			return null;
		} 
	}
}
