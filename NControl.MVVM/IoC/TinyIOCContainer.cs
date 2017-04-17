/****************************** Module Header ******************************\
Module Name:  SimpleInjectContainer.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using TinyIoC;

namespace NControl.Mvvm
{
	/// <summary>
	/// Simple inject container.
	/// </summary>
	public class TinyIOCContainer: IContainer
	{
		#region Private Members

		/// <summary>
		/// The container.
		/// </summary>
		protected TinyIoCContainer _container;

		#endregion

		/// <summary>
		/// Initializes a new instance of the container class.
		/// </summary>
		public TinyIOCContainer ()
		{
			_container = new TinyIoCContainer ();
		}

		#region IContainer implementation

		/// <summary>
		/// Resolves the given type into an instance
		/// </summary>
		/// <typeparam name="TTypeToResolve">The 1st type parameter.</typeparam>
		public TTypeToResolve Resolve<TTypeToResolve> () 
			where TTypeToResolve : class
		{
			return _container.Resolve<TTypeToResolve> ();
		}

		/// <summary>
		/// Resolves the given type into an instance
		/// </summary>
		/// <typeparam name="TTypeToResolve">The 1st type parameter.</typeparam>
		/// <param name="typeToResolve">Type to resolve.</param>
		public object Resolve (Type typeToResolve)
		{
			return _container.Resolve (typeToResolve);
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
			_container.Register<RegisterType, RegisterImplementation> ().AsMultiInstance();
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
			_container.Register (registerType, registerImplementation).AsMultiInstance();
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
			_container.Register<RegisterType>(registerImplementation).AsMultiInstance();
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
			_container.Register<RegisterType, RegisterImplementation> ().AsSingleton();
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
			_container.Register(RegisterType, RegisterImplementation).AsSingleton();
		}

		/// <summary>
		/// Registers the singleton using a callback
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <typeparam name="RegisterType">The 1st type parameter.</typeparam>
		public void RegisterSingleton<RegisterType>(Func<RegisterType> callback)
			where RegisterType : class
		{
			_container.Register(typeof(RegisterType), (container, overlads) => callback());
		}

		#endregion
	}
}

