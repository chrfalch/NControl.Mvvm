using System;
using NControl.Mvvm;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public abstract class FluidMvvmApp: MvvmApp
	{
		public FluidMvvmApp(IMvvmPlatform platform):base(platform)
		{			
		}

		protected override void RegisterProviders()
		{
			base.RegisterProviders();

			Container.RegisterSingleton(() => Current.Presenter as IActivityIndicatorViewProvider);

			RegisterNavigationContainerProvider();
		}

		/// <summary>
		/// Override to register a provider to return different container and modal container views.
		/// </summary>
		public virtual void RegisterNavigationContainerProvider()
		{
			Container.RegisterSingleton<INavigationContainerProvider, NavigationContainerProvider>();
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
		/// Sets up sizes.
		/// </summary>
		protected override void SetupSizes()
		{
			base.SetupSizes();

			Current.Sizes.Set(FluidConfig.DefaultNavigationBarHeight, Device.OnPlatform(46, 56, 46));
			Current.Sizes.Set(FluidConfig.DefaultStatusbarHeight, Device.OnPlatform(22, 0, 22));
			Current.Sizes.Set(FluidConfig.DefaultToolbarItemWidth, 30);

			Current.Sizes.Set(FluidConfig.DefaultPopupTitleHeight, 34);
			Current.Sizes.Set(FluidConfig.DefaultPopupCornerRadius, 8);
			Current.Sizes.Set(FluidConfig.DefaultBorderRadius, 8);
		}

		/// <summary>
		/// Sets up colors.
		/// </summary>
		protected override void SetupColors()
		{
			base.SetupColors();

			Current.Colors.Set(FluidConfig.DefaultPopupTitleBackgroundColor, Colors.Get(Config.PrimaryColor));
			Current.Colors.Set(FluidConfig.DefaultPopupTitleColor, Colors.Get(Config.NegativeTextColor));
			Current.Colors.Set(FluidConfig.DefaultShadowColor, Color.Black);
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
