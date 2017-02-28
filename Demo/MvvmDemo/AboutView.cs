using System;
using NControl.Mvvm;
using NControl.Controls;
using NControl.Mvvm.Fluid;
using Xamarin.Forms;

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
					new Label {HorizontalTextAlignment = TextAlignment.Center}.BindTo(Label.TextProperty, nameof(ViewModel.NumberValue), stringFormat:"Number Value: {0}"),
					new VerticalSeparator(),
					new ExtendedButton {Text="Push new about", Command = ViewModel.PushNewAboutCommand },
					new ExtendedButton {Text="Close", Command = ViewModel.CloseCommand },
					new VerticalSeparator(),
				}						
			};
		}
	}
}

