using System;
using NControl.Controls;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class CompanyView: BaseFluidItemListView<CompanyViewModel, Company>
	{
		public CompanyView ()
		{
			ToolbarItems.Add(new ToolbarItemEx
			{
				MaterialDesignIcon = FontMaterialDesignLabel.MDRefresh,
				Command = ViewModel.RefreshEmptyCommand,
			});

			ToolbarItems.Add (new ToolbarItemEx {
				MaterialDesignIcon = FontMaterialDesignLabel.MDMagnify,
				Command = ViewModel.SearchCommand,
			});
		}

		protected override View CreateContents()
		{
			return new Grid()
				.AddChildTo(base.CreateContents(), 0, 0)
				.AddChildTo(new ExpandingButtonPanel{

					VerticalOptions =LayoutOptions.End,
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
							Icon = FontMaterialDesignLabel.MDMenu,
							Command = ViewModel.ShowCityListCommand
						
					}
				}
			}, 0, 0);
		}
			
		public override DataTemplate GetDataTemplate()
		{
			return new DataTemplate(typeof(TextCell)).BindTo(
				TextCell.TextProperty, NameOf<Company>(c => c.Name));
		}
	}
}

