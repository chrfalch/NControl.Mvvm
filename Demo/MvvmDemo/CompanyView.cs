using System;
using NControl.Controls;
using NControl.Mvvm;
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

		protected override View CreateContents()
		{
			return new Grid
			{				
				Children = {
					new VerticalStackLayout
					{				
						
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
											FontSize = 66,
											TextColor = MvvmApp.Current.Colors.Get(Config.LightTextColor),
										},

										new Label{
											Text = "No Items Found",
											HorizontalTextAlignment = TextAlignment.Center,
										}
									},
								},

								//LoadingView = new Grid{
								//	Children = {
								//		new ActivityIndicator{
								//			IsRunning = true,
								//			VerticalOptions = LayoutOptions.Center,
								//			HorizontalOptions = LayoutOptions.Center
								//		}
								//	}
								//}
							}
							.BindTo(ListViewControl.RefreshCommandProperty, NameOf(vm => vm.RefreshCommand))
							.BindTo(ListViewControl.StateProperty, NameOf(vm => vm.CollectionState)),

							new ExpandingButtonPanel{
								VerticalOptions = LayoutOptions.End,
								Buttons = {
									new ButtonBarItem{
										Icon = FontMaterialDesignLabel.MDInformationVariant,
										Command = ViewModel.ShowAboutCommand,
									},

									new ButtonBarItem{
										Command = ViewModel.ShowFeedCommand,
										Icon = FontMaterialDesignLabel.MDPot,
									},

									new ButtonBarItem{
										Icon = FontMaterialDesignLabel.MDKey,
									}
								}
							}
						}
					},
				}
			};
		}
	}
}

