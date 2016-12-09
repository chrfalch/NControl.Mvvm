/****************************** Module Header ******************************\
Module Name:  BaseModel.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections;
using System.Linq;
using System.Windows.Input;

namespace NControl.Mvvm
{
    /// <summary>
    /// Base model.
    /// </summary>
    public class BaseModel: BaseNotifyPropertyChangedObject
    {
        #region Private Members

        /// <summary>
        /// The storage.
        /// </summary>
        readonly Dictionary<string, object> _storage = new Dictionary<string, object>();

        /// <summary>
        /// Command dependencies - key == property, value = list of property names
        /// </summary>
        readonly Dictionary<string, List<string>> _propertyDependencies = 
            new Dictionary<string, List<string>>();

		/// <summary>
		/// property message dependencies dict
		/// </summary>
		readonly Dictionary<Type, List<PropertyInfo>> _propertyMessageDependencies = 
			new Dictionary<Type, List<PropertyInfo>>();

        /// <summary>
        /// The notify change for same values.
        /// </summary>
        readonly List<string> _notifyChangeForSameValues = new List<string>();

        #endregion

        /// <summary>
        /// Initializes a new instance of the BaseModel class.
        /// </summary>
        public BaseModel()
        {
            // Update property dependencies
            ResolvePropertyDependencies();

			// Update property message dependencies
			ResolvePropertyMessageDependencies();
        }

		~BaseModel()
		{
			UnsubscribeToOnMessageProperties();
			System.Diagnostics.Debug.WriteLine(GetType().Name + " finalizer");
		}

        #region Protected Members

        /// <summary>
        /// Sets a value in viewmodel storage and raises property changed if value has changed
        /// </summary>
        /// <param name="propertyName">Name.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="TValueType">The 1st type parameter.</typeparam>
        protected bool SetValue<TValueType>(TValueType value, [CallerMemberName] string propertyName = null) 
        {
			if (value is IComparer) {
				
				var existingValue = GetValue<TValueType> (propertyName);

				// Check for equality
				if (!_notifyChangeForSameValues.Contains (propertyName) &&
		            EqualityComparer<TValueType>.Default.Equals (existingValue, value))
					return false;
			}

            SetObjectForKey<TValueType> (propertyName, value);

            RaisePropertyChangedEvent (propertyName);

            return true;
        }

        /// <summary>
        /// Adds a dependency between a property and another property. Whenever the property changes, the command's 
        /// state will be updated
        /// </summary>
        /// <param name="property">Source property.</param>
        /// <param name="dependantProperty">Target property.</param>
        private void AddPropertyDependency(Expression<Func<object>> property, 
            Expression<Func<object>> dependantProperty)
        {
            AddPropertyDependency(PropertyNameHelper.GetPropertyName <BaseModel>(property), 
                PropertyNameHelper.GetPropertyName<BaseModel>(dependantProperty));
        }

        /// <summary>
        /// Adds a dependency between a property and another property. Whenever the property changes, the command's 
        /// state will be updated
        /// </summary>
        /// <param name="sourceProperty">Source property.</param>
        /// <param name="dependantProperty">Target property.</param>
        protected void AddPropertyDependency(string sourceProperty, string dependantProperty)
        {
            if (!_propertyDependencies.ContainsKey (sourceProperty))
                _propertyDependencies.Add (sourceProperty, new List<string> ());

            var list = _propertyDependencies [sourceProperty];
            if (!list.Contains(dependantProperty))                
                list.Add (dependantProperty);
        }

        /// <summary>
        /// Adds the raise notify changed for property when value is the same.
        /// </summary>
        protected void AddRaiseNotifyChangedForPropertyWhenValueIsTheSame(string propertyName)
        {
            _notifyChangeForSameValues.Add(propertyName);
        }

        /// <summary>
        /// Calls the notify property changed event if it is attached. By using some
        /// Expression/Func magic we get compile time type checking on our property
        /// names by using this method instead of calling the event with a string in code.
        /// </summary>
        /// <param name="property">Property.</param>
        protected override void RaisePropertyChangedEvent (Expression<Func<object>> property)
        {
            base.RaisePropertyChangedEvent (property);
            var propertyName = PropertyNameHelper.GetPropertyName<BaseModel>(property);
            CheckDependantProperties (propertyName);
        }

        /// <summary>
        /// Calls the notify property changed event if it is attached. By using some
        /// Expression/Func magic we get compile time type checking on our property
        /// names by using this method instead of calling the event with a string in code.
        /// </summary>
        /// <param name="propertyName">Property.</param>
        protected override void RaisePropertyChangedEvent (string propertyName)
        {
            base.RaisePropertyChangedEvent (propertyName);
            CheckDependantProperties (propertyName);
        }

        /// <summary>
        /// Checks the dependant properties and commands.
        /// </summary>
        protected virtual void CheckDependantProperties (string propertyName)
        {
            // Dependent properties?
            if (_propertyDependencies.ContainsKey (propertyName)) {
                foreach (var dependentProperty in _propertyDependencies[propertyName])
                    RaisePropertyChangedEvent (dependentProperty);
            }
        }

        /// <summary>
        /// Returns a value from the viewmodel storage
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="property">Name.</param>
        /// <typeparam name="TValueType">The 1st type parameter.</typeparam>
        protected TValueType GetValue<TValueType>([CallerMemberName] string property = null) 
        {
            return GetValue<TValueType> (() => default(TValueType), propertyName:property);
        }

		/// <summary>
        /// Returns a value from the viewmodel storage
        /// </summary>
        protected TValueType GetValue<TValueType>(Func<TValueType> defaultValueFunc, 
		                                          [CallerMemberName] string propertyName = null) 
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("propertyName");

            return GetObjectForKey<TValueType> (propertyName, defaultValueFunc());
        }

        /// <summary>
        /// Sets the object for key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected void SetObjectForKey<T>(string key, T value)
        {
            if (_storage.ContainsKey (key))
                _storage [key] = value;
            else
                _storage.Add (key, value);      
        }

        /// <summary>
        /// Gets the object for key.
        /// </summary>
        /// <returns>The object for key.</returns>
        /// <param name="key">Key.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected T GetObjectForKey<T>(string key, T defaultValue)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key");

            if (!_storage.ContainsKey (key)) {
                if (defaultValue == null)
                    return defaultValue;

                SetObjectForKey (key, defaultValue);
            }

			try
			{
				return (T)Convert.ChangeType(_storage[key], typeof(T));
			}
			catch
			{
				return (T)_storage[key];
			}
        }

        /// <summary>
        /// Handles the property dependency.
        /// </summary>
        /// <param name="dependantPropertyInfo">Dependant property info.</param>
        protected virtual bool HandlePropertyDependency(PropertyInfo dependantPropertyInfo, string sourcePropertyName)
        {
            return false;   
        }

		/// <summary>
		/// Unsubscribes to on message properties.
		/// </summary>
		protected void UnsubscribeToOnMessageProperties()
		{
			foreach (var messageType in _propertyMessageDependencies.Keys)
			{
				MvvmApp.Current.MessageHub.Unsubscribe(messageType, this);
			}

			_propertyMessageDependencies.Clear();
		}

		/// <summary>
		/// Subscribes to on message properties.
		/// </summary>
		protected void SubscribeToOnMessageProperties()
		{
			ResolvePropertyMessageDependencies();
		}

        #endregion

        #region Private Members

		void ResolvePropertyMessageDependencies()
		{
			foreach (var prop in this.GetType().GetRuntimeProperties())
			{
				// Check for OnMessageAttribute
				var attribute = prop.GetCustomAttribute<OnMessageAttribute>();
				if (attribute == null)
					continue;

				// Verify that the property type is ICommand
				if (prop.PropertyType.GetTypeInfo().ImplementedInterfaces.Any(intf => intf == typeof(ICommand)))
				{
					// Do we have a subscription?
					if (!_propertyMessageDependencies.ContainsKey(attribute.MessageType))
					{
						// Remember
						_propertyMessageDependencies.Add(attribute.MessageType, new List<PropertyInfo>());

						// Weak this
						var weakReference = new WeakReference(this);

						// Listen
						MvvmApp.Current.MessageHub.Subscribe(attribute.MessageType, this, (object obj) =>
						{
							if (!weakReference.IsAlive)
								return;
							
							object instance = weakReference.Target;

							if (_propertyMessageDependencies.ContainsKey(attribute.MessageType))
							{
								foreach (var p in _propertyMessageDependencies[attribute.MessageType])
								{
									var command = p.GetValue(instance) as ICommand;
									if (command == null)
										continue;

									if (command.CanExecute(obj))
										command.Execute(obj);
								}
							}
						});
					}

					// Add proeprty
					if (!_propertyMessageDependencies[attribute.MessageType].Contains(prop))
						_propertyMessageDependencies[attribute.MessageType].Add(prop);
				}
			}
		}

        /// <summary>
        /// Reads the property dependencies.
        /// </summary>
        void ResolvePropertyDependencies()
        {
            foreach (var prop in this.GetType().GetRuntimeProperties())
            {				
                foreach (var dependantPropertyInfo in this.GetType().GetRuntimeProperties())
                {                
					// Check for DependsOnAttribute
					var dependsOnAttribute = dependantPropertyInfo.GetCustomAttribute<DependsOnAttribute>();
                    if (dependsOnAttribute == null)
                        continue;

					foreach (var property in dependsOnAttribute.SourceProperties) {

						var handled = HandlePropertyDependency (dependantPropertyInfo, property);
						if (!handled) {
							
							// Add a dependency between two properties
							AddPropertyDependency (property, dependantPropertyInfo.Name);

							if (dependsOnAttribute.RaisePropertyChangeForEqualValues)
								AddRaiseNotifyChangedForPropertyWhenValueIsTheSame (property);
						}
					}
                }
            }
        } 
        #endregion
    }
}

