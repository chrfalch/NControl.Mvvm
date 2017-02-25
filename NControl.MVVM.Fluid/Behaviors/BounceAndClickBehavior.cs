using System;
using System.Windows.Input;
using NControl.Mvvm;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class BounceAndClickBehavior: Behavior<View>
	{
		readonly ICommand _clickCommand;

		public BounceAndClickBehavior(ICommand clickCommand)
		{
			_clickCommand = clickCommand;
		}

		protected override void OnAttachedTo(View bindable)
		{
			base.OnAttachedTo(bindable);

			// Perform setup
			bindable.AddGestureRecognizerTo(new TapGestureRecognizer
			{
				Command = new AsyncCommand(async (obj) => {

					if (_clickCommand != null && _clickCommand.CanExecute(null))
					{
						var animation = new XAnimation.XAnimation(new[] { bindable });
						animation
							.Duration(70)
							.Scale(0.75)
							.Animate()
							.Duration(70)
							.Scale(1.0)
							.Animate();

						_clickCommand.Execute(null);
						await animation.RunAsync();
					}
				})
			});
		}
	}

	
}
