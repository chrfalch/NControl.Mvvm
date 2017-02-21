using System;
using NControl.Mvvm;
using NControl.Mvvm.Fluid;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class EmployeeView: BaseFluidContentsView<EmployeeViewModel>
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
					new Button {
						HorizontalOptions = LayoutOptions.FillAndExpand,
						BackgroundColor = Color.Accent,
						HeightRequest = 44,
						TextColor = Color.White,
						Text = "Back",
						Command = ViewModel.CloseCommand,
					},
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

