/****************************** Module Header ******************************\
Module Name:  MvvmApp.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace NControl.Mvvm
{
	/// <summary>
	/// Base class for Mvvm apps
	/// </summary>
	public abstract class MvvmApp : Application
	{
		/// <summary>
		/// The app.
		/// </summary>
		public static new MvvmApp Current { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NControl.Mvvm.MvvmApp"/> class.
		/// </summary>
		/// <param name="platform">Platform.</param>
		public MvvmApp(IMvvmPlatform platform)
		{
			// Save static for ease of access
			Current = this;

			// Avoid calling virtual members from the constructor:
			Setup(platform);
		}

		#region Setup

		/// <summary>
		/// Setup the specified platform.
		/// </summary>
		/// <param name="platform">Platform.</param>
		private void Setup(IMvvmPlatform platform)
		{
			// Register container
			Container.Initialize(CreateContainer());

			// Create view container and presenter
			RegisterViewContainer();
			RegisterPresenter();

			// Sets up the messaging service
			RegisterMessagingService();

			// Register providers
			RegisterProviders();

			// Set up services
			RegisterServices();

			// Register configuration providers
			RegisterSizeProvider();
			RegisterColorProvider();

			// Initialize platform app
			platform.Initialize();

			// Set up views
			RegisterViews();

			// Set up colors and sizes
			SetupSizes();
			SetupColors();

			// Set main page
			Presenter.SetMainPage(GetMainPage());
		}

		#endregion

		#region Properties

		/// <summary>
		/// Returns the presenter.
		/// </summary>
		/// <value>The presenter.</value>
		public IPresenter Presenter { get { return Container.Resolve<IPresenter>(); } }

		/// <summary>
		/// Returns the presenter.
		/// </summary>
		/// <value>The presenter.</value>
		public IViewContainer ViewContainer { get { return Container.Resolve<IViewContainer>(); } }

		/// <summary>
		/// Gets the activity indicator.
		/// </summary>
		/// <value>The view container.</value>
		public IActivityIndicator ActivityIndicator { get { return Container.Resolve<IActivityIndicator>(); } }

		/// <summary>
		/// Gets the messaging service.
		/// </summary>
		/// <value>The messaging service.</value>
		public IMessageHub MessageHub { get { return Container.Resolve<IMessageHub>(); } }

		/// <summary>
		/// Returns the environment information
		/// </summary>
		/// <value>The environment.</value>
		public IEnvironmentProvider Environment { get { return Container.Resolve<IEnvironmentProvider>(); } }

		/// <summary>
		/// Returns the default sizes for the app
		/// </summary>
		public ISizeProvider Sizes { get { return Container.Resolve<ISizeProvider>(); } }

		/// <summary>
		/// Gets the color constants
		/// </summary>
		public IColorProvider Colors { get { return Container.Resolve<IColorProvider>(); } }

		#endregion

		#region Protected Members

		/// <summary>
		/// Creates the container. Override and DONT call base to serve other implementations
		/// </summary>
		/// <returns>The container.</returns>
		protected virtual IContainer CreateContainer()
		{
			return new SimpleInjectContainer();
		}

		/// <summary>
		/// Registers the presenter.
		/// </summary>
		protected virtual void RegisterPresenter()
		{
			Container.RegisterSingleton<IPresenter, DefaultPresenter>();
		}

		/// <summary>
		/// Registers the size provider.
		/// </summary>
		protected virtual void RegisterSizeProvider()
		{
			Container.RegisterSingleton<ISizeProvider, DefaultSizeProvider>();
		}

		/// <summary>
		/// Set up default sizes
		/// </summary>
		protected virtual void SetupSizes()
		{
			Current.Sizes.Set(Config.DefaultPadding, 8);
			Current.Sizes.Set(Config.DefaultSpacing, 8);
			Current.Sizes.Set(Config.DefaultLargePadding, 24);
			Current.Sizes.Set(Config.DefaultLargeSpacing, 14);
			Current.Sizes.Set(Config.DefaultBorderSize, 0.5 * Current.Environment.DisplayDensity);
			Current.Sizes.Set(Config.DefaultBoldBorderSize, 1.0 * Current.Environment.DisplayDensity);
			Current.Sizes.Set(Config.DefaultActivityIndicatorSize, 44);
		}

		/// <summary>
		/// Registers the color provider.
		/// </summary>
		protected virtual void RegisterColorProvider()
		{
			Container.RegisterSingleton<IColorProvider, DefaultColorProvider>();
		}

		/// <summary>
		/// Sets up colors.
		/// </summary>
		protected virtual void SetupColors()
		{
			Current.Colors.Set(Config.PrimaryColor, Color.FromHex("#2196F3"));
			Current.Colors.Set(Config.PrimaryDarkColor, Color.FromHex("#1976D2"));
			Current.Colors.Set(Config.AccentColor, Color.Accent);

			Current.Colors.Set(Config.TextColor, Color.Black);
			Current.Colors.Set(Config.NegativeTextColor, Color.White);
			Current.Colors.Set(Config.AccentTextColor, Color.Accent);
			Current.Colors.Set(Config.LightTextColor, Color.FromHex("CECECE"));

			Current.Colors.Set(Config.BorderColor, Color.FromHex("CECECE"));

			Current.Colors.Set(Config.ViewBackgroundColor, Color.FromHex("FEFEFE"));
			Current.Colors.Set(Config.ViewTransparentBackgroundColor, Color.Black.MultiplyAlpha(0.75));
            
		}

		/// <summary>
		/// Registers the view container
		/// </summary>
		protected virtual void RegisterViewContainer()
		{
			Container.RegisterSingleton<IViewContainer, DefaultViewContainer>();
		}

		/// <summary>
		/// Override to register providers
		/// </summary>
		protected virtual void RegisterProviders()
		{ 
		}

		/// <summary>
		/// Registers the messaging service
		/// </summary>
		protected virtual void RegisterMessagingService()
		{
			Container.RegisterSingleton<IMessageHub, MessageHub> ();
		}

		/// <summary>
		/// Registers the views.
		/// </summary>
		protected abstract void RegisterViews();

		/// <summary>
		/// Registers the services.
		/// </summary>
		protected virtual void RegisterServices()
		{
			
		}

		/// <summary>
		/// Gets the main page.
		/// </summary>
		/// <returns>The main page.</returns>
		protected abstract Page GetMainPage();

		#endregion

	}
}

