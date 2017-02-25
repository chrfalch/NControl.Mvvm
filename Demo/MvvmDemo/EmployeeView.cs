using System;
using NControl.Controls;
using NControl.Mvvm;
using NControl.Mvvm.Fluid;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class EmployeeView: BaseFluidContentsView<EmployeeViewModel>
	{
		public EmployeeView ()
		{
			ToolbarItems.Add(new ToolbarItemEx
			{
				MaterialDesignIcon = FontMaterialDesignLabel.MDPlus,
			});
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
					
					new Button{
						Text = "Open Da Thing",
						Command = ViewModel.ShowAboutCommand,
					}
				}
			};
		}
	}
}

