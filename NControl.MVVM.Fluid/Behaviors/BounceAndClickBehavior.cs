using System;
using System.Windows.Input;
using NControl.Mvvm;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class BounceAndClickBehavior: Behavior<View>
	{
		readonly ICommand _clickCommand;
		readonly object _clickCommandParameter;
		readonly Func<ICommand> _commandFunc;

		public BounceAndClickBehavior(ICommand clickCommand)
		{
			_clickCommand = clickCommand;
		}

		public BounceAndClickBehavior(ICommand clickCommand, object clickCommandParameter)
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
				Command = new AsyncCommand(async (obj) => {

					if (_clickCommand != null && _clickCommand.CanExecute(_clickCommandParameter))
					{
						var animation = new XAnimationPackage(new[] { bindable });
						animation
							.Duration(70)
							.Scale(0.75)
							.Animate()
							.Duration(70)
							.Scale(1.0)
							.Animate();

						_clickCommand.Execute(_clickCommandParameter);
						await animation.RunAsync();
					}
				})
			});
		}
	}

	
}
