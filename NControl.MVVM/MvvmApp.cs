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
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NControl.Mvvm
{
	/// <summary>
	/// Base class for Mvvm apps
	/// </summary>
	public abstract class MvvmApp : Application
	{
		#region Private Members

		/// <summary>
		/// Cache for elements
		/// </summary>
		readonly Dictionary<string, object> _storage = new Dictionary<string, object>();

		#endregion

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
			using (PerformanceTimer.Current.BeginTimer(this))
			{
				// Save static for ease of access
				Current = this;

				// Avoid calling virtual members from the constructor:
				Setup(platform);
			}
		}

		#region Setup

		/// <summary>
		/// Setup the specified platform.
		/// </summary>
		/// <param name="platform">Platform.</param>
		void Setup(IMvvmPlatform platform)
		{
			using (PerformanceTimer.Current.BeginTimer(this))
			{
				// Register container
				using (PerformanceTimer.Current.BeginTimer(this, "Setting up container"))
					Container.Initialize(CreateContainer());

				// Create view container and presenter
				using (PerformanceTimer.Current.BeginTimer(this, "Registering Views"))
					RegisterViewContainer();

				using (PerformanceTimer.Current.BeginTimer(this, "Registering Presenter"))
					RegisterPresenter();

				// Sets up the messaging service
				using (PerformanceTimer.Current.BeginTimer(this, "Registering Messaging Service"))
					RegisterMessagingService();

				// Register providers
				using (PerformanceTimer.Current.BeginTimer(this, "Registering Providers"))
					RegisterProviders();

				// Set up services
				using (PerformanceTimer.Current.BeginTimer(this, "Registering Services"))
					RegisterServices();
				
				// Initialize platform app
				using (PerformanceTimer.Current.BeginTimer(this, "Platform Initialize"))
					platform.Initialize();

				// Set up views
				using (PerformanceTimer.Current.BeginTimer(this, "Registering Views"))
					RegisterViews();

				// Set up colors and sizes
				using (PerformanceTimer.Current.BeginTimer(this, "Settings up settings"))
				{
					SetupSizes();
					SetupColors();
				}

				// Set main page
				using (PerformanceTimer.Current.BeginTimer(this, "Setting Main Page"))
					Presenter.SetMainPage(GetMainPage());

			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Get the specified key and callback.
		/// </summary>
		public T Get<T>(Func<T> callback = null, [CallerMemberName] string key = null)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentException("key");

			if (!_storage.ContainsKey(key))
			{
				if (callback == null)
					_storage.Add(key, default(T));
				else
					_storage.Add(key, callback());
			}

			return (T)_storage[key];
		}

		/// <summary>
		/// Set the specified key and value.
		/// </summary>
		public void Set<T>(T value, [CallerMemberName] string key = null)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentException("key");
			
			if (!_storage.ContainsKey(key))
				_storage.Add(key, value);
			else
				_storage[key] = value;
		}

		/// <summary>
		/// Returns the presenter.
		/// </summary>
		/// <value>The presenter.</value>
		public IPresenter Presenter { get { return Get(()=>Container.Resolve<IPresenter>()); } }

		/// <summary>
		/// Returns the presenter.
		/// </summary>
		/// <value>The presenter.</value>
		public IViewContainer ViewContainer { get { return Get(()=>Container.Resolve<IViewContainer>()); } }

		/// <summary>
		/// Gets the activity indicator.
		/// </summary>
		/// <value>The view container.</value>
		public IActivityIndicator ActivityIndicator { get { return Get(()=>Container.Resolve<IActivityIndicator>()); } }

		/// <summary>
		/// Gets the messaging service.
		/// </summary>
		/// <value>The messaging service.</value>
		public IMessageHub MessageHub { get { return Get(()=>Container.Resolve<IMessageHub>()); } }

		/// <summary>
		/// Returns the environment information
		/// </summary>
		/// <value>The environment.</value>
        public IEnvironmentProvider Environment { get { return Get(()=>Container.Resolve<IEnvironmentProvider>()); } }

		#endregion

		#region Protected Members

		/// <summary>
		/// Creates the container. Override and DONT call base to serve other implementations
		/// </summary>
		/// <returns>The container.</returns>
		protected virtual IContainer CreateContainer()
		{
			return new TinyIOCContainer();
		}

		/// <summary>
		/// Registers the presenter.
		/// </summary>
		protected virtual void RegisterPresenter()
		{
			Container.RegisterSingleton<IPresenter, DefaultPresenter>();
		}

		/// <summary>
		/// Set up default sizes
		/// </summary>
		protected virtual void SetupSizes()
		{
            
		}

		/// <summary>
		/// Sets up colors.
		/// </summary>
		protected virtual void SetupColors()
		{
			
            
		}

		/// <summary>
		/// Registers the view container
		/// </summary>
		protected virtual void RegisterViewContainer()
		{
			using(PerformanceTimer.Current.BeginTimer(this))
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

