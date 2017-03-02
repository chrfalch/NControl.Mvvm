using System;
using System.Windows.Input;
using NControl.Mvvm;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public class BounceAndClickBehavior: ClickBehavior
	{
		BounceBehavior _bouncer = new BounceBehavior(); 

		public BounceAndClickBehavior(Func<ICommand> commandFunc) : base(commandFunc) {}
		public BounceAndClickBehavior(Func<ICommand> commandFunc, Func<object> commandParameterFunc): base(commandFunc, commandParameterFunc) {}
		public BounceAndClickBehavior(ICommand clickCommand) : base(clickCommand) {}
		public BounceAndClickBehavior(ICommand clickCommand, object clickCommandParameter) : base(clickCommand, clickCommandParameter) { }

		protected override void OnAttachedTo(View bindable)
		{						
			base.OnAttachedTo(bindable);
			_bouncer.AttachTo(bindable);
			_bouncer.Tapped += _bouncer_Tapped;
		}		

		protected override void OnDetachingFrom(View bindable)
		{			
			base.OnDetachingFrom(bindable);
			_bouncer.DetachingFrom(bindable);
			_bouncer.Tapped -= _bouncer_Tapped;
		}

		void _bouncer_Tapped(object sender, EventArgs e)
		{
			ExecuteCommand();
		}
	}
}
