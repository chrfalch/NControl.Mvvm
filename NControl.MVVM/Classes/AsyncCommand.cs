/*
 * Based on the article: Patterns for Asynchronous MVVM Applications: Commands
 * http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
 * 
 * Modified by Scott Chamberlain 11-19-2014
 * - Added parameter support 
 * - Added the ability to shut off the single invocation restriction.
 * - Made a non-generic version of the class that called the generic version with a <object> return type.
 */
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NControl.Mvvm
{
	public class AsyncCommand : AsyncCommand<object>
	{
		public AsyncCommand(Func<object, Task> command)
			: base(async (parameter, token) => { await command(parameter);  }, null)
		{
		}

		public AsyncCommand(Func<object, Task> command, Func<object, bool> canExecute)
			: base(async (parameter, token) => { await command(parameter);  }, canExecute)
		{
		}

		public AsyncCommand(Func<object, CancellationToken, Task> command)
			: base(async (parameter, token) => { await command(parameter, token); }, null)
		{
		}

		public AsyncCommand(Func<object, CancellationToken, Task> command, Func<object, bool> canExecute)
			: base(async (parameter, token) => { await command(parameter, token);  }, canExecute)
		{
		}
	}

	public class AsyncCommand<T> : AsyncCommandBase, INotifyPropertyChanged where T : class
	{
		private readonly Func<T, CancellationToken, Task> _command;
		private readonly CancelAsyncCommand _cancelCommand;
		private readonly Func<T, bool> _canExecute;
		private NotifyTaskCompletion _execution;
		private bool _allowMultipleInvocations;

		public AsyncCommand(Func<T, Task> command)
			: this((parameter, token) => command(parameter as T), null)
		{
		}

		public AsyncCommand(Func<T, Task> command, Func<T, bool> canExecute)
			: this((parameter, token) => command(parameter), canExecute)
		{
		}

		public AsyncCommand(Func<T, CancellationToken, Task> command)
			: this(command, null)
		{
		}

		public AsyncCommand(Func<T, CancellationToken, Task> command, Func<T, bool> canExecute)
		{
			_command = command;
			_canExecute = canExecute;
			_cancelCommand = new CancelAsyncCommand();
		}


		public override bool CanExecute(object parameter)
		{
			var canExecute = _canExecute == null || _canExecute(parameter as T);
			var executionComplete = (Execution == null || Execution.IsCompleted);

			return canExecute && (AllowMultipleInvocations || executionComplete);
		}

		public override async Task ExecuteAsync(object parameter)
		{
			_cancelCommand.NotifyCommandStarting();
			Execution = new NotifyTaskCompletion(_command(parameter as T, _cancelCommand.Token));
			RaiseCanExecuteChanged();
			await Execution.TaskCompletion;
			_cancelCommand.NotifyCommandFinished();
			RaiseCanExecuteChanged();
		}

		public bool AllowMultipleInvocations
		{
			get { return _allowMultipleInvocations; }
			set
			{
				if (_allowMultipleInvocations == value)
					return;

				_allowMultipleInvocations = value;
				OnPropertyChanged();
			}
		}

		public ICommand CancelCommand
		{
			get { return _cancelCommand; }
		}

		public NotifyTaskCompletion Execution
		{
			get { return _execution; }
			private set
			{
				_execution = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}

		private sealed class CancelAsyncCommand : ICommand
		{
			private CancellationTokenSource _cts = new CancellationTokenSource();
			private bool _commandExecuting;

			public CancellationToken Token { get { return _cts.Token; } }

			public void NotifyCommandStarting()
			{
				_commandExecuting = true;
				if (!_cts.IsCancellationRequested)
					return;
				_cts = new CancellationTokenSource();
				RaiseCanExecuteChanged();
			}

			public void NotifyCommandFinished()
			{
				_commandExecuting = false;
				RaiseCanExecuteChanged();
			}

			bool ICommand.CanExecute(object parameter)
			{
				return _commandExecuting && !_cts.IsCancellationRequested;
			}

			void ICommand.Execute(object parameter)
			{
				_cts.Cancel();
				RaiseCanExecuteChanged();
			}

			public event EventHandler CanExecuteChanged;

			private void RaiseCanExecuteChanged()
			{
				CanExecuteChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
