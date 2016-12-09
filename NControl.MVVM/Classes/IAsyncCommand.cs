using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NControl.Mvvm
{
	public interface IAsyncCommand : ICommand
	{
		Task ExecuteAsync(object parameter);
	}
}
