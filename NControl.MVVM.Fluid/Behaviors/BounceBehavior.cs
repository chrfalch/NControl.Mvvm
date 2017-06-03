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

				switch (e.TouchType)
				{
					case TouchType.Start:
						
						var animation = new XAnimationPackage(new[] { bindable });
						animation.SetDuration(70).Add((transform) =>
							transform.SetScale(_scaleTo));
						animation.Animate();

						break;
					case TouchType.Ended:

						var animation2 = new XAnimationPackage(new[] { bindable });
						animation2.SetDuration(70).Add((transform) =>
							transform.SetScale(1.0));
						animation2.Animate(()=> Tapped?.Invoke(bindable, EventArgs.Empty));

						break;
					case TouchType.Cancelled:					
											
						var animation3 = new XAnimationPackage(new[] { bindable });
						animation3.SetDuration(70).Add((transform) =>
							transform.SetScale(1.0));
						animation3.Animate();

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
