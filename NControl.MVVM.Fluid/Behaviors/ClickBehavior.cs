using System;
using System.Windows.Input;
using NControl.Mvvm;
using Xamarin.Forms;

namespace NControl.Mvvm
{

	public class ClickBehavior : Behavior<View>
	{
		readonly ICommand _clickCommand;
		readonly Func<ICommand> _commandFunc;

		public ClickBehavior(Func<ICommand> commandFunc)
		{
			_commandFunc = commandFunc;
		}

		public ClickBehavior(ICommand clickCommand)
		{
			_clickCommand = clickCommand;
		}

		protected override void OnAttachedTo(View bindable)
		{
			base.OnAttachedTo(bindable);

			// Perform setup
			bindable.AddGestureRecognizerTo(new TapGestureRecognizer
			{
				Command = new Command((obj) =>
				{
					if (_clickCommand != null && _clickCommand.CanExecute(obj))
					{
						_clickCommand.Execute(obj);
					}
					else if (_commandFunc != null)
					{
						var command = _commandFunc();
						if (command != null && command.CanExecute(obj))
							command.Execute(obj);
					}
				})
			});
		}
	}
}
