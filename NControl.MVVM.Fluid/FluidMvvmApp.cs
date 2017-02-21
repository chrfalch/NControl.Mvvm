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
		/// Override to provide information about main view
		/// </summary>
		/// <returns>The main view type.</returns>
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
