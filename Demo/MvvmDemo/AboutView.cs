using System;
using NControl.Mvvm;
using NControl.Mvvm.Fluid;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class AboutView: BaseFluidContentsView<AboutViewModel>
	{
		public AboutView ()
		{
			ToolbarItems.Add (new ToolbarItem {
				Text = "Close",
				Command = ViewModel.CloseCommand,
			});
		}

		protected override Xamarin.Forms.View CreateContents ()
		{
			return new StackLayout {
				Orientation = StackOrientation.Vertical,
				Padding = 15,
				Spacing = 8,
				Children = {
					new Label{Text = "Demo Application for NControl.Mvvm" },
					new Button {Text="Call Command through message.", Command = ViewModel.ClickMeCommand },
					new Button {Text="Run async command", Command = ViewModel.CountAsyncCommand },
					new Label {HorizontalTextAlignment = TextAlignment.Center}.BindTo(Label.TextProperty, nameof(ViewModel.NumberValue), stringFormat:"Number Value: {0}"),
					new Button {Text="Close", Command = ViewModel.CloseCommand },
				}
			};
		}
	}
}

