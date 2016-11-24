using System;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class MenuView: BaseContentsView<MenuViewModel>
	{
		public MenuView ()
		{
			Device.OnPlatform(() => Icon = "MenuButton");
		}

		protected override Xamarin.Forms.View CreateContents ()
		{
			return new ListViewEx ();
		}
	}
}

