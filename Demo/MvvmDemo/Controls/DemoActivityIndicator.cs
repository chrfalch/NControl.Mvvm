using System;
using NControl.Mvvm;
using Xamarin.Forms;
using Lottie.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics.Contracts;

namespace MvvmDemo
{
	public class DemoActivityIndicator: BaseFluidActivityIndicator
	{
		AnimationView _animationView;

		protected override View CreateActivityControl()
		{
			Contract.Ensures(Contract.Result<View>() != null);
			_animationView = new AnimationView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,

				Animation = "loading_animation.json",
				Loop = true,
				HeightRequest = 120,
			};

			return _animationView;		
		}

		protected override void RunningChanged(bool isRunning)
		{
			if (_animationView == null)
				return;

			if (isRunning)
				_animationView.Play();
			else
				_animationView.Pause();
		}
	}
}
