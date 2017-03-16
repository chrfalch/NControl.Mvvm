using System;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class EmployeeDetailsView : BasePopupFluidContentsView<EmployeeDetailsViewModel>
	{
		protected override View CreatePopupContents()
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

		public override Size ContentSize {
			get {return new Size(0, 180);}
		}
	}
}

