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
		public static new MvvmApp Current {get; private set;}

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
			Container.Initialize (CreateContainer());

			// Create view container and presenter
			RegisterViewContainer();
			RegisterPresenter();

			// Sets up the messaging service
			RegisterMessagingService ();

			// Register providers
			RegisterProviders();

			// Set up services
			RegisterServices();

			// Initialize platform app
			platform.Initialize();

			// Set up views
			RegisterViews();

			// Set main page
			Presenter.SetMainPage(GetMainPage());
		}

		#endregion

		#region Properties

		/// <summary>
		/// Returns the presenter.
		/// </summary>
		/// <value>The presenter.</value>
		public IPresenter Presenter { get { return Container.Resolve<IPresenter> (); } }

		/// <summary>
		/// Returns the presenter.
		/// </summary>
		/// <value>The presenter.</value>
		public IViewContainer ViewContainer { get { return Container.Resolve<IViewContainer> (); } }

		/// <summary>
		/// Gets the activity indicator.
		/// </summary>
		/// <value>The view container.</value>
		public IActivityIndicator ActivityIndicator { get { return Container.Resolve<IActivityIndicator> (); } }

		/// <summary>
		/// Gets the messaging service.
		/// </summary>
		/// <value>The messaging service.</value>
		public IMessageHub MessageHub { get { return Container.Resolve<IMessageHub>(); } }

		/// <summary>
		/// Returns the environment information
		/// </summary>
		/// <value>The environment.</value>
		public IEnvironmentProvider Environment{ get { return Container.Resolve<IEnvironmentProvider>(); }}

		#endregion

		#region Protected Members

		/// <summary>
		/// Creates the container. Override and DONT call base to serve other implementations
		/// </summary>
		/// <returns>The container.</returns>
		protected virtual IContainer CreateContainer()
		{
			return new SimpleInjectContainer ();
		}

		/// <summary>
		/// Registers the presenter.
		/// </summary>
		protected virtual void RegisterPresenter()
		{
			Container.RegisterSingleton<IPresenter, DefaultPresenter>();
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

