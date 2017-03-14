using System;
using NControl.Controls;
using NControl.Mvvm;
using NControl.Mvvm.Fluid;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class CompanyView: BaseFluidContentsView<CompanyViewModel>
	{
		public CompanyView ()
		{
			ToolbarItems.Add (new ToolbarItemEx {
				MaterialDesignIcon = FontMaterialDesignLabel.MDRefresh,
				Command = ViewModel.RefreshCommand,
			});

			ToolbarItems.Add (new ToolbarItemEx {
				MaterialDesignIcon = FontMaterialDesignLabel.MDMagnify,
				Command = ViewModel.SearchCommand,
			});
		}

		protected override View CreateContents ()
		{
			return new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Children = {
					new ListViewControl{
						ItemsSource = ViewModel.Companies,
						IsPullToRefreshEnabled = true,
						ItemSelectedCommand = ViewModel.SelectCompanyCommand,
						ItemTemplate = new DataTemplate(typeof(TextCell))
							.BindTo(TextCell.TextProperty, NameOf<Company>(cw => cw.Name)),

						EmptyListView = new VerticalWizardStackLayout{
							Children = {
								new FontMaterialDesignLabel{
									Text = FontMaterialDesignLabel.MDNaturePeople,
									FontSize = 36,
									TextColor = MvvmApp.Current.Colors.Get(Config.LightTextColor),
								},

								new Label{
									Text = "No Items Found",
									HorizontalTextAlignment = TextAlignment.Center,
								}
							},
						},
					}
					.BindTo(ListViewControl.RefreshCommandProperty, NameOf(vm => vm.RefreshCommand))
					.BindTo(ListViewControl.StateProperty, NameOf(vm => vm.CollectionState)),

					new ExtendedButton{
						Text = "Open Da Thing",
						Command = ViewModel.ShowAboutCommand,
					}
				}
			};
		}
	}
}

