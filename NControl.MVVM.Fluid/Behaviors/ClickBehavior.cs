using System;
using System.Windows.Input;
using NControl.Mvvm;
using Xamarin.Forms;

namespace NControl.Mvvm
{

	public class ClickBehavior : Behavior<View>
	{
		readonly ICommand _clickCommand;
		readonly object _clickCommandParameter;
		readonly Func<ICommand> _commandFunc;
		readonly Func<object> _commandParameterFunc;

		public ClickBehavior(Func<ICommand> commandFunc)
		{
			_commandFunc = commandFunc;
		}

		public ClickBehavior(Func<ICommand> commandFunc, Func<object> commandParameterFunc)
		{
			_commandFunc = commandFunc;
			_commandParameterFunc = commandParameterFunc;
		}

		public ClickBehavior(ICommand clickCommand)
		{
			_clickCommand = clickCommand;
		}

		public ClickBehavior(ICommand clickCommand, object clickCommandParameter)
		{
			_clickCommand = clickCommand;
			_clickCommandParameter = clickCommandParameter;
		}

		protected override void OnAttachedTo(View bindable)
		{
			base.OnAttachedTo(bindable);

			// Perform setup
			bindable.AddGestureRecognizerTo(new TapGestureRecognizer
			{
				Command = new Command((obj) =>
				{
					if (_clickCommand != null && _clickCommand.CanExecute(_clickCommandParameter))
					{
						_clickCommand.Execute(_clickCommandParameter);
					}
					else if (_commandFunc != null)
					{
						var command = _commandFunc();
						object parameter = null;
						if(_commandParameterFunc != null)
							parameter = _commandParameterFunc();
						
						if (command != null && command.CanExecute(parameter))
							command.Execute(parameter);
					}
				})
			});
		}
	}
}
