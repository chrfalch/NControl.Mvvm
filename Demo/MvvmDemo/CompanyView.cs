using System;
using NControl.Mvvm;
using NControl.Mvvm.Fluid;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class CompanyView: BaseFluidContentsView<CompanyViewModel>
	{
		public CompanyView ()
		{
			ToolbarItems.Add (new ToolbarItem {
				Text = "About",
				Command = ViewModel.ShowAboutCommand,
			});

			ToolbarItems.Add (new ToolbarItem {
				Text = "Search",
				Command = ViewModel.SearchCommand,
			});
		}

		protected override View CreateContents ()
		{
			return new StackLayout {
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Children = {
					new ListViewEx{
						ItemsSource = ViewModel.Companies,
						ItemSelectedCommand = ViewModel.SelectCompanyCommand,
						ItemTemplate = new DataTemplate(typeof(TextCell))
							.BindTo(TextCell.TextProperty, NameOf<Company>(cw => cw.Name)),						
					},

					new Button{
						Text = "Open Da Thing",
						Command = ViewModel.ShowAboutCommand,
					}
				}
			};
		}
	}
}

