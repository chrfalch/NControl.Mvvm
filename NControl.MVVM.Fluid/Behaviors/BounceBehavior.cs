using System;
using NControl.Mvvm;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class BounceBehavior: BaseViewBehavior
	{
		readonly GestureRecognizerBehavior _behavior = new GestureRecognizerBehavior();
		public event EventHandler Tapped;

		readonly double _scaleTo;

		public BounceBehavior(): this(0.75)
		{			
		}

		public BounceBehavior(double scaleTo)
		{
			_scaleTo = scaleTo;
		}

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
						animation.Add((transform)=>
							transform.SetDuration(70)
			              		.SetScale(_scaleTo));
						break;
					case TouchType.Ended:
						Tapped?.Invoke(bindable, EventArgs.Empty);
						animation.Add((transform) =>
							transform.SetDuration(70)
								.SetScale(1.0));
						break;
					case TouchType.Cancelled:
						animation.Add((transform) =>
							transform.SetDuration(70)
								.SetScale(1.0));
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
