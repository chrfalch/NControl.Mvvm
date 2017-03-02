using System;
using NControl.Mvvm;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public class BounceBehavior: BaseViewBehavior
	{
		readonly GestureRecognizerBehavior _behavior = new GestureRecognizerBehavior();
		public event EventHandler Tapped;

		protected override void OnAttachedTo(View bindable)
		{
			base.OnAttachedTo(bindable);

			_behavior.AttachTo(bindable);
			_behavior.Touched += (sender, e) => {

				var animation = new XAnimationPackage(new[] { bindable });
				switch (e.TouchType)
				{
					case TouchType.Start:
						// Animate press
						animation
							.Duration(70)
							.Scale(0.75)
							.Animate()
							.Run();
						break;
					case TouchType.Ended:
						Tapped?.Invoke(bindable, EventArgs.Empty);
						animation
							.Duration(70)
							.Scale(1.0)
							.Animate()
							.Run();
						break;
					case TouchType.Cancelled:
						animation
							.Duration(70)
							.Scale(1.0)
							.Animate()
							.Run();
						break;						
				}
			};
		}

		protected override void OnDetachingFrom(View bindable)
		{
			base.OnDetachingFrom(bindable);
			_behavior.DetachingFrom(bindable);
		}
	}
}
