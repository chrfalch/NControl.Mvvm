/****************************** Module Header ******************************\
Module Name:  SimpleInjectContainer.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;

namespace NControl.Mvvm
{
	/// <summary>
	/// Simple inject container.
	/// </summary>
	public class SimpleInjectContainer: IContainer
	{
		#region Private Members

		/// <summary>
		/// The container.
		/// </summary>
		protected SimpleInjector.Container _container;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="NControl.MVVM.SimpleInjectContainer"/> class.
		/// </summary>
		public SimpleInjectContainer ()
		{
			_container = new SimpleInjector.Container ();
		}

		#region IContainer implementation

		/// <summary>
		/// Resolves the given type into an instance
		/// </summary>
		/// <typeparam name="TTypeToResolve">The 1st type parameter.</typeparam>
		public TTypeToResolve Resolve<TTypeToResolve> () 
			where TTypeToResolve : class
		{
			return _container.GetInstance<TTypeToResolve> ();
		}

		/// <summary>
		/// Resolves the given type into an instance
		/// </summary>
		/// <typeparam name="TTypeToResolve">The 1st type parameter.</typeparam>
		/// <param name="typeToResolve">Type to resolve.</param>
		public object Resolve (Type typeToResolve)
		{
			return _container.GetInstance (typeToResolve);
		}

		/// <summary>
		/// Register a type in the container.
		/// </summary>
		/// <typeparam name="RegisterType">The 1st type parameter.</typeparam>
		/// <typeparam name="RegisterImplementation">The 2nd type parameter.</typeparam>
		public void Register<RegisterType, RegisterImplementation> () 
			where RegisterType : class 
			where RegisterImplementation : class, RegisterType
		{
			_container.Register<RegisterType, RegisterImplementation> ();
		}

		/// <summary>
		/// Register a type in the container.
		/// </summary>
		/// <typeparam name="RegisterType">The 1st type parameter.</typeparam>
		/// <typeparam name="RegisterImplementation">The 2nd type parameter.</typeparam>
		/// <param name="registerType">Register type.</param>
		/// <param name="registerImplementation">Register implementation.</param>
		public void Register (Type registerType, Type registerImplementation)
		{
			_container.Register (registerType, registerImplementation);
		}

		/// <summary>
		/// Register the specified registerImplementation.
		/// </summary>
		/// <param name="registerImplementation">Register implementation.</param>
		/// <typeparam name="RegisterType">The 1st type parameter.</typeparam>
		/// <typeparam name="RegisterImplementation">The 2nd type parameter.</typeparam>
		public void Register<RegisterType, RegisterImplementation> (RegisterImplementation registerImplementation) 
			where RegisterType : class 
			where RegisterImplementation : class, RegisterType
		{
			_container.Register<RegisterType> (()=> registerImplementation);
		}

		/// <summary>
		/// Register a type as a singleton in the container.
		/// </summary>
		/// <typeparam name="RegisterType">The 1st type parameter.</typeparam>
		/// <typeparam name="RegisterImplementation">The 2nd type parameter.</typeparam>
		public void RegisterSingleton<RegisterType, RegisterImplementation> () 
			where RegisterType : class 
			where RegisterImplementation : class, RegisterType
		{
			_container.RegisterSingleton<RegisterType, RegisterImplementation> ();
		}

		/// <summary>
		/// Register a type as a singleton in the container.
		/// </summary>
		/// <typeparam name="RegisterType">The 1st type parameter.</typeparam>
		/// <typeparam name="RegisterImplementation">The 2nd type parameter.</typeparam>
		/// <param name="RegisterType">Register type.</param>
		/// <param name="RegisterImplementation">Register implementation.</param>
		public void RegisterSingleton (Type RegisterType, Type RegisterImplementation)
		{
			_container.RegisterSingleton (RegisterType, RegisterImplementation);
		}

		/// <summary>
		/// Registers the singleton using a callback
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <typeparam name="RegisterType">The 1st type parameter.</typeparam>
		public void RegisterSingleton<RegisterType>(Func<RegisterType> callback)
			where RegisterType : class
		{
			_container.RegisterSingleton<RegisterType>(callback);
		}

		#endregion
	}
}

