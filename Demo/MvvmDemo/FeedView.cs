using System;
using NControl.Controls;
using NControl.Mvvm;
using Xamarin.Forms;

namespace MvvmDemo
{
	public class FeedView : BaseFluidItemListView<FeedViewModel, FeedItem>
	{
		public FeedView()
		{
			ToolbarItems.Add(new ToolbarItemEx
			{
				MaterialDesignIcon = FontMaterialDesignLabel.MDClose,
				Command = ViewModel.CloseCommand,
			});
		}
		public override int GetRowHeight()
		{
			return 180;
		}

		protected override void CustomizeListView(ListViewEx listView)
		{
			base.CustomizeListView(listView);

			listView.BackgroundColor = Color.Silver;
		}

		public override DataTemplate GetDataTemplate()
		{
			return new DataTemplateSelectorEx((obj) => {
				if ((ViewModel.Items.IndexOf(obj as FeedItem) % 2 == 0))
				   return new DataTemplate(typeof(FeedCell));

				return new DataTemplate(typeof(OddFeedCell));
			});
		}
	}
}
