using System;
using NControl.Mvvm;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class BounceBehavior: Behavior<View>
	{
		protected override void OnAttachedTo(View bindable)
		{
			base.OnAttachedTo(bindable);

			// Perform setup
			bindable.AddGestureRecognizerTo(new TapGestureRecognizer
			{
				Command = new AsyncCommand(async (obj) =>
				{
					var animation = new XAnimation.XAnimationPackage(new[] { bindable });
					animation
						.Duration(70)
						.Scale(0.75)
						.Animate()
						.Duration(70)
						.Scale(1.0)
						.Animate();

					await animation.RunAsync();

				})
			});
		}
	}
}
