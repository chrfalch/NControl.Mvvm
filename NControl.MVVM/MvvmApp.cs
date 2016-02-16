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

namespace NControl.MVVM
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
		/// Initializes a new instance of the <see cref="Test.NewSolution.App"/> class.
		/// </summary>
		/// <param name="typeResolveProvider">Type resolve provider.</param>
		public MvvmApp (IMvvmPlatform platformApp)
		{	
			// Save static for ease of access
			Current = this;

			// Register container
			Container.Initialize (CreateContainer());

			// Set up presenter and view container
			Container.RegisterSingleton<IViewContainer, DefaultViewContainer>();
			Container.RegisterSingleton<IPresenter, DefaultPresenter> ();
			//Container.RegisterSingleton<IActivityIndicator, DefaultActivityIndicator> ();

			// Initialize platform app
			platformApp.Initialize();

			// Set up services
			RegisterServices();

			// Set up views
			RegisterViews();

			// Set main page
			Presenter.SetMainPage (GetMainPage());
		}

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

