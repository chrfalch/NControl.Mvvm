using System;
using System.Windows.Input;
using NControl.Mvvm;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class BounceAndClickBehavior: ClickBehavior
	{
		readonly BounceBehavior _bouncer; 

		public BounceAndClickBehavior(Func<ICommand> commandFunc, double scaleTo = 0.75) : base(commandFunc) 
		{
			_bouncer = new BounceBehavior(scaleTo);
		}

		public BounceAndClickBehavior(Func<ICommand> commandFunc, Func<object> commandParameterFunc, 
		                              double scaleTo = 0.75): base(commandFunc, commandParameterFunc) 
		{
			_bouncer = new BounceBehavior(scaleTo);
		}

		public BounceAndClickBehavior(ICommand clickCommand, double scaleTo = 0.75) : base(clickCommand)
		{
			_bouncer = new BounceBehavior(scaleTo);
		}

		public BounceAndClickBehavior(ICommand clickCommand, object clickCommandParameter, 
		                              double scaleTo = 0.75) : base(clickCommand, clickCommandParameter) 
		{
			_bouncer = new BounceBehavior(scaleTo);
		}

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
