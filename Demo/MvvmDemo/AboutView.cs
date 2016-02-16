using System;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class AboutView: BaseContentsView<AboutViewModel>
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
					new Button { Text = "Close" }.BindTo(Button.CommandProperty, NameOf(vm => vm.CloseCommand)),
				}
			};
		}
	}
}

