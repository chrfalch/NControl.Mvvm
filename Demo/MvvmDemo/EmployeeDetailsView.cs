using System;
using NControl.Mvvm;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class EmployeeDetailsView: BaseFluidContentsView<EmployeeDetailsViewModel>
	{
		public EmployeeDetailsView ()
		{				 		
		}

		protected override Xamarin.Forms.View CreateContents ()
		{
			return new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Children = {
					new StackLayout {
						Orientation = StackOrientation.Vertical,
						Padding = 15,
						Spacing = 8,
						Children = {
							new Label{TextColor =Color.Blue}.BindTo(Label.TextProperty, NameOf(mn => mn.Employee.Id)),
							new Label{TextColor =Color.Blue}.BindTo(Label.TextProperty, NameOf(mn => mn.Employee.Name)),
							new ExtendedButton {
								Text = "Close",
							}.BindTo(Button.CommandProperty, NameOf(vm => vm.CloseCommand)),
						}
					}
				}
			};
		}
	}
}

