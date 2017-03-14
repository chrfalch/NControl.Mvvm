using System;
using NControl.Mvvm;
using NControl.Controls;
using NControl.Mvvm.Fluid;
using Xamarin.Forms;
using System.Collections.Generic;
using NControl.XAnimation;

namespace MvvmDemo
{
	public class AboutView: BaseFluidContentsView<AboutViewModel>
	{
		public AboutView ()
		{
			ToolbarItems.Add (new ToolbarItemEx {
				MaterialDesignIcon = FontMaterialDesignLabel.MDClose,
				Command = ViewModel.CloseCommand,
			});
		}

		protected override Xamarin.Forms.View CreateContents ()
		{
			return new VerticalStackLayoutWithPadding {								
				Children = {
					new Label{Text = "Demo Application for NControl.Mvvm" ,HorizontalTextAlignment = TextAlignment.Center},
					new ExtendedButton {Text="Call Command through message.", Command = ViewModel.ClickMeCommand },
					new VerticalSeparator(),
					new ExtendedButton {Text="Run async command", Command = ViewModel.CountAsyncCommand },
					new FluidActivityIndicator{Color = MvvmApp.Current.Colors.Get(Config.AccentColor)}
						.BindTo(FluidActivityIndicator.IsRunningProperty, nameof(ViewModel.IsRunningAsyncCommand)),

					new Label {HorizontalTextAlignment = TextAlignment.Center}.BindTo(Label.TextProperty, nameof(ViewModel.NumberValue), stringFormat:"Number Value: {0}"),
					new VerticalSeparator(),
					new ExtendedButton {Text="Push new about", Command = ViewModel.PushNewAboutCommand },
					new ExtendedButton {Text="Close", Command = ViewModel.CloseCommand },
					new VerticalSeparator(),
				}						
			};
		}

		protected override IEnumerable<XAnimationPackage> ModalTransitionIn(
			INavigationContainer container, IEnumerable<XAnimationPackage> animations)
		{
			// Swap out original animations
			return new[] { 
				new XAnimationPackage(container.GetNavigationBarView())					
					.Translate(0, -container.GetNavigationBarView().Height)
					.Rotate(360)
					.Set()
					.Duration(1500)
					.Easing(EasingFunction.EaseOut)
					.Translate(0, 0)
					.Rotate(0)
					.Animate(),

				new XAnimationPackage(container.GetContainerView())
					.Translate(0, container.GetContainerView().Height)
					.Set()
					.Duration(1500)
					.Easing(EasingFunction.EaseOut)
					.Translate(0, 0).Animate()
			};
		}

		protected override IEnumerable<XAnimationPackage> ModalTransitionOut(
			INavigationContainer container, IEnumerable<XAnimationPackage> animations)
		{
			// Swap out original animations
			return new[] {
				new XAnimationPackage(container.GetNavigationBarView())					
					.Easing(EasingFunction.EaseOut)
					.Translate(0, -container.GetNavigationBarView().Height)
					.Duration(1500)
					.Rotate(360)
					.Easing(EasingFunction.EaseIn)
					.Animate(),

				new XAnimationPackage(container.GetContainerView())
					.Easing(EasingFunction.EaseOut)
					.Translate(0, container.GetContainerView().Height)
					.Duration(350)
					.Easing(EasingFunction.EaseIn)
					.Animate()
			};
		}
	}
}

