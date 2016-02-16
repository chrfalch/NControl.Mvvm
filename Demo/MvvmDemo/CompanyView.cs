using System;
using NControl.MVVM;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class CompanyView: BaseContentsView<CompanyViewModel>
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
				Children = {					
					new ListViewEx{
						ItemsSource = ViewModel.Companies,
						ItemSelectedCommand = ViewModel.SelectCompanyCommand,
						ItemTemplate = new DataTemplate(typeof(TextCell))
							.BindTo(TextCell.TextProperty, NameOf<Company>(cw => cw.Name)),						
					}
				}
			};
		}
	}
}

