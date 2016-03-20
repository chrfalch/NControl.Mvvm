using System;
using NControl.Mvvm;

namespace MvvmDemo
{
	public class MenuView: BaseContentsView<MenuViewModel>
	{
		public MenuView ()
		{
			Icon = "MenuButton";
		}

		protected override Xamarin.Forms.View CreateContents ()
		{
			return new ListViewEx ();
		}
	}
}

