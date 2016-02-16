using System;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class EmployeeView: BaseContentsView<EmployeeViewModel>
	{
		public EmployeeView ()
		{
		}

		protected override View CreateContents ()
		{
			return new StackLayout {
				Orientation = StackOrientation.Vertical,
				Spacing = 8,
				Children = {
					new ListViewEx{
						ItemsSource = ViewModel.Employees,
						ItemSelectedCommand = ViewModel.SelectEmployeeCommand,
						ItemTemplate = new DataTemplate(typeof(TextCell))
							.BindTo(TextCell.TextProperty, NameOf<Employee>(e => e.Name))
					},
				}
			};
		}
	}
}

