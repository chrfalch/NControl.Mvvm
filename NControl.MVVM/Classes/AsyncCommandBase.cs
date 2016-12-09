/*
 * Based on the article: Patterns for Asynchronous MVVM Applications: Commands
 * http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
 */
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NControl.Mvvm
{
	public abstract class AsyncCommandBase : IAsyncCommand
	{
		public abstract bool CanExecute(object parameter);

		public abstract Task ExecuteAsync(object parameter);

		public async void Execute(object parameter)
		{
			await ExecuteAsync(parameter);
		}

		public event EventHandler CanExecuteChanged;

		protected void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}