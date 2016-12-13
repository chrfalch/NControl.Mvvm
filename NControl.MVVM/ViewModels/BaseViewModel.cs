﻿/****************************** Module Header ******************************\
Module Name:  BaseViewModel.cs
Copyright (c) Christian Falch
All rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Windows.Input;
using System.Linq;

namespace NControl.Mvvm
{
	/// <summary>
	/// Base view model.
	/// </summary>
	public abstract class BaseViewModel : BaseModel, IViewModel
	{
		#region Private Members

		/// <summary>
		/// Command dependencies - key == property, value = list of commands
		/// </summary>
		private readonly Dictionary<string, List<Command>> _commandDependencies =
			new Dictionary<string, List<Command>>();

		/// <summary>
		/// Command cache
		/// </summary>
		readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>();

		/// <summary>
		/// Async command cache
		/// </summary>
		readonly Dictionary<string, AsyncCommandBase> _asyncCommands = new Dictionary<string, AsyncCommandBase>();

		/// <summary>
		/// The property change listeners.
		/// </summary>
		private readonly List<PropertyChangeListener> _propertyChangeListeners = new List<PropertyChangeListener>();

		/// <summary>
		/// The command execute dependencies.
		/// </summary>
		private readonly Dictionary<string, List<Command>> _commandExecuteDependencies =
			new Dictionary<string, List<Command>>();

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the BaseViewModel class.
		/// </summary>
		public BaseViewModel()
		{
			// Title
			Title = this.GetType().Name;

			ResolveCommandExecuteDependencies();
		}

		#endregion

		#region Protected Members

		/// <summary>
		/// Creates or returns the 
		/// </summary>
		/// <returns>The command.</returns>
		protected Command GetOrCreateCommand(Action commandAction, Func<bool> canExecuteFunc = null,
			[CallerMemberName] string commandName = null)
		{
			if (string.IsNullOrEmpty(commandName))
				throw new ArgumentException("commandname");

			if (!_commands.ContainsKey(commandName))
			{
				if (canExecuteFunc == null)
					_commands.Add(commandName, new Command(commandAction));
				else
					_commands.Add(commandName, new Command(commandAction, canExecuteFunc));
			}

			return _commands[commandName];
		}

		/// <summary>
		/// Creates or returns the 
		/// </summary>
		/// <returns>The command.</returns>
		protected Command GetOrCreateCommand(Action<object> commandAction, Func<object, bool> canExecuteFunc = null,
			[CallerMemberName] string commandName = null)
		{
			if (string.IsNullOrEmpty(commandName))
				throw new ArgumentException("commandname");

			if (!_commands.ContainsKey(commandName))
			{
				if (canExecuteFunc == null)
					_commands.Add(commandName, new Command(commandAction));
				else
					_commands.Add(commandName, new Command(commandAction, canExecuteFunc));
			}

			return _commands[commandName];
		}

		/// <summary>
		/// Creates or returns the 
		/// </summary>
		/// <returns>The command.</returns>
		protected Command<T> GetOrCreateCommand<T>(Action<T> commandAction, Func<T, bool> canExecuteFunc = null,
			[CallerMemberName] string commandName = null)
		{
			if (string.IsNullOrEmpty(commandName))
				throw new ArgumentException("commandname");

			if (!_commands.ContainsKey(commandName))
			{
				if (canExecuteFunc == null)
					_commands.Add(commandName, new Command<T>(commandAction));
				else
					_commands.Add(commandName, new Command<T>(commandAction, canExecuteFunc));
			}

			return _commands[commandName] as Command<T>;
		}

		/// <summary>
		/// Creates or returns the 
		/// </summary>
		/// <returns>The command.</returns>
		protected AsyncCommandBase GetOrCreateCommandAsync(Func<object, Task> commandAction, Func<object, bool> canExecuteFunc = null,
			[CallerMemberName] string commandName = null)
		{
			if (string.IsNullOrEmpty(commandName))
				throw new ArgumentException("commandname");

			if (!_asyncCommands.ContainsKey(commandName))
			{
				if (canExecuteFunc == null)
					_asyncCommands.Add(commandName, new AsyncCommand(commandAction));
				else
					_asyncCommands.Add(commandName, new AsyncCommand(commandAction, canExecuteFunc));
			}

			return _asyncCommands[commandName];
		}

		/// <summary>
		/// Creates or returns the 
		/// </summary>
		/// <returns>The command.</returns>
		protected AsyncCommand<T> GetOrCreateCommandAsync<T>(Func<T, Task> commandAction, Func<T, bool> canExecuteFunc = null,
			[CallerMemberName] string commandName = null) where T : class
		{
			if (string.IsNullOrEmpty(commandName))
				throw new ArgumentException("commandname");

			if (!_asyncCommands.ContainsKey(commandName))
			{
				if (canExecuteFunc == null)
					_asyncCommands.Add(commandName, new AsyncCommand<T>(commandAction));
				else
					_asyncCommands.Add(commandName, new AsyncCommand<T>(commandAction, canExecuteFunc));
			}

			return _asyncCommands[commandName] as AsyncCommand<T>;
		}

		/// <summary>
		/// Listens for property change.
		/// </summary>
		/// <param name="property">Property.</param>
		/// <typeparam name="TViewModel">The 1st type parameter.</typeparam>
		protected void ListenForPropertyChange<TObject>(Expression<Func<TObject, object>> property, TObject obj, Action callback)
		{
			var changeListener = new PropertyChangeListener();
			changeListener.Listen<TObject>(property, obj, callback);
			_propertyChangeListeners.Add(changeListener);
		}

		/// <summary>
		/// Checks the dependant properties and commands.
		/// </summary>
		protected override void CheckDependantProperties(string propertyName)
		{
			base.CheckDependantProperties(propertyName);

			// Dependent commands
			if (_commandDependencies.ContainsKey(propertyName))
			{
				foreach (var dependentCommand in _commandDependencies[propertyName])
					RaiseCommandStateChangedEvent(dependentCommand);
			}

			// Execute commands
			if (_commandExecuteDependencies.ContainsKey(propertyName))
			{
				var property = this.GetType().GetRuntimeProperty(propertyName);
				var propValue = property.GetValue(this);
				foreach (var dependantCommand in _commandExecuteDependencies[propertyName])
					ExecuteCommand(dependantCommand, propValue);
			}
		}

		#endregion

		#region Private Members

		/// <summary>
		/// Adds a dependency between a command and a property. Whenever the property changes, the command's 
		/// state will be updated
		/// </summary>
		void AddCommandDependency(string propertyName, Command command)
		{
			if (!_commandDependencies.ContainsKey(propertyName))
				_commandDependencies.Add(propertyName, new List<Command>());

			var list = _commandDependencies[propertyName];
			list.Add(command);
		}

		/// <summary>
		/// Raises the command state changed event.
		/// </summary>
		/// <param name="command">Command.</param>
		void RaiseCommandStateChangedEvent(Command command)
		{
			command.ChangeCanExecute();
		}

		/// <summary>
		/// Adds the command execute dependency.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <param name="command">Command.</param>
		void AddCommandExecuteDependency(string propertyName, Command command)
		{
			if (!_commandExecuteDependencies.ContainsKey(propertyName))
				_commandExecuteDependencies.Add(propertyName, new List<Command>());

			var list = _commandExecuteDependencies[propertyName];
			list.Add(command);
		}

		/// <summary>
		/// Raises the command state changed event.
		/// </summary>
		/// <param name="command">Command.</param>
		void ExecuteCommand(Command command, object commandParameter)
		{
			if (command.CanExecute(commandParameter))
				command.Execute(commandParameter);
		}

		/// <summary>
		/// Handles the property dependency.
		/// </summary>
		protected override bool HandlePropertyDependency(PropertyInfo dependantPropertyInfo, string sourcePropertyName)
		{			
			if (dependantPropertyInfo.PropertyType == typeof(ICommand) ||
			    dependantPropertyInfo.PropertyType.GetTypeInfo().ImplementedInterfaces.Any(intf => intf == typeof(ICommand)))
			{
				// Add a dependency between command and property
				AddCommandDependency(sourcePropertyName, dependantPropertyInfo.GetValue(this) as Command);

				return true;
			}

			return base.HandlePropertyDependency(dependantPropertyInfo, sourcePropertyName);
		}

		/// <summary>
		/// Resolves the command execute dependencies.
		/// </summary>
		void ResolveCommandExecuteDependencies()
		{
			foreach (var prop in this.GetType().GetRuntimeProperties())
			{
				foreach (var dependantPropertyInfo in this.GetType().GetRuntimeProperties())
				{
					var attribute = dependantPropertyInfo.GetCustomAttribute<ExecuteOnChangeAttribute>();
					if (attribute == null)
						continue;

					foreach (var property in attribute.SourceProperties)
					{

						// Get the command instance
						var commandInstance = (Command)dependantPropertyInfo.GetValue(this);

						// Add a dependency between command and property
						AddCommandExecuteDependency(property, commandInstance);
					}
				}
			}
		}

		#endregion

		#region ViewModel LifeCycle

		/// <summary>
		/// Initializes the viewmodel
		/// </summary>
		public virtual Task InitializeAsync()
		{
			return Task.FromResult(true);
		}

		/// <summary>
		/// Override to implement logic when the view has been set up on screen
		/// </summary>
		public virtual async Task OnAppearingAsync()
		{
			System.Diagnostics.Debug.WriteLine(GetType().Name + " showing");
			IsShowing = true;

			// Call initialize
			if (!IsOnAppearingCalled)
			{
				IsOnAppearingCalled = true;
				await InitializeAsync();
			}
		}

		/// <summary>
		/// Called whenever the view is hidden
		/// </summary>
		public virtual Task OnDisappearingAsync()
		{
			System.Diagnostics.Debug.WriteLine(GetType().Name + " not showing");
			IsShowing = false;
			return Task.FromResult(true);
		}

		/// <summary>
		/// Called when the viewmodel is being dismissed
		/// </summary>
		public virtual void ViewModelDismissed()
		{
			System.Diagnostics.Debug.WriteLine(GetType().Name + " popped.");
			UnsubscribeToOnMessageProperties();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get { return GetValue<string>(); }
			set { SetValue<string>(value); }
		}

		/// <summary>
		/// Flag that is set when OnAppearing was called.
		/// </summary>
		/// <value><c>true</c> if this instance is on appearing called; otherwise, <c>false</c>.</value>
		public bool IsOnAppearingCalled
		{
			get { return GetValue<bool>(); }
			set { SetValue<bool>(value); }
		}

		/// <summary>
		/// Returns true if the viewmodel is current being displayed not covered by other views.
		/// </summary>
		/// <value><c>true</c> if is showing; otherwise, <c>false</c>.</value>
		public bool IsShowing
		{
			get { return GetValue<bool>(); }
			private set { SetValue<bool>(value); }
		}

		/// <summary>
		/// Gets the back button command.
		/// </summary>
		/// <value>The back button command.</value>
		public virtual Command BackButtonCommand { get { return null; } }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is busy.
		/// </summary>
		/// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
		public bool IsBusy { 
			get{ return GetValue(() => false); }
			set{ SetValue(value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is busy.
		/// </summary>
		/// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
		public string IsBusyText 
		{ 
			get{ return GetValue<string>(); }
			set{ SetValue<string> (value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is busy.
		/// </summary>
		/// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
		public string IsBusySubTitle 
		{ 
			get{ return GetValue<string>(); }
			set{ SetValue<string> (value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is busy.
		/// </summary>
		/// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
		[DependsOn(nameof(IsBusyText))]
		public bool IsBusyTextVisible 
		{ 
			get { return !String.IsNullOrEmpty (IsBusyText); }
		}

		#endregion

		#region Commands

		/// <summary>
		/// The default close command
		/// </summary>
		/// <value>The close command.</value>
		public ICommand CloseCommand
		{
			get {
				return GetOrCreateCommandAsync (async(obj) => {
					await MvvmApp.Current.Presenter.DismissViewModelAsync(PresentationMode);
				});
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the presentation mode.
		/// </summary>
		/// <value>The presentation mode.</value>
		public PresentationMode PresentationMode { get; set; }

		#endregion
	}

	/// <summary>
	/// Base view model with typed parameter for navigation
	/// </summary>
	public abstract class BaseViewModel<TParameter> : BaseViewModel
		where TParameter: class
	{
		/// <summary>
		/// Initializes the viewmodel with a parameter. Called from the navigation manager through the view 
		/// as a IViewWithParameter instance
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="parameter">Parameter.</param>
		/// <typeparam name="TParameter">The 1st type parameter.</typeparam>
		public virtual Task InitializeAsync (TParameter parameter)
		{
			return Task.FromResult(true);
		}
	}
}

