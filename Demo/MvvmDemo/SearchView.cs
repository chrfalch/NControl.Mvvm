using System;
using NControl.MVVM;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class SearchView: BaseContentsView<SearchViewModel>
	{
		public SearchView ()
		{
			ToolbarItems.Add (new ToolbarItem {
				Text = "Close",
				Command = ViewModel.CloseCommand,
			});
		}

		protected override View CreateContents ()
		{
			return new StackLayout {
				Padding = 0,
				Spacing = 4,
				Orientation = StackOrientation.Vertical,
				Children = {
					new SearchBar()
						.BindTo(SearchBar.TextProperty, NameOf(vm => vm.Query)),

					new ListViewEx{
						ItemsSource = ViewModel.Employees,
						ItemSelectedCommand = ViewModel.EmployeSelectedCommand,
						ItemTemplate = new DataTemplate(typeof(TextCell))
							.BindTo(TextCell.TextProperty, NameOf<Employee>(mn => mn.Name))
					}
				}
			};
		}
	}
}

