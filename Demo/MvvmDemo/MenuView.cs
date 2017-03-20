using System;
using NControl.Mvvm;
using NControl.Controls;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class MenuView: BaseFluidMenuView<MenuViewModel>
	{
		protected override void SetupMenuView()
		{
			Header = new Label { Text = "Hello Menu" };
			MenuContent = new VerticalWizardStackLayout { 
				Children = {
					new IconLabel { Text = "Login", Icon = FontMaterialDesignLabel.MDLock},
					new IconLabel { Text = "Something", Icon = FontMaterialDesignLabel.MDVk},
					new IconLabel { Text = "Abba", Icon = FontMaterialDesignLabel.MDVpn},
				}
			};
			Footer = new Button { Text = "Close", Command = ViewModel.CloseCommand };
		}
	}
}

