using System;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public abstract class BasePopupFluidContentsView<TViewModel>: BaseFluidContentsView<TViewModel>,
		IView<TViewModel>, IToolbarItemsContainer, ILeftBorderProvider, IXViewAnimatable, IContentSizeProvider
		where TViewModel : BaseViewModel
	{
		Label _titleLabel;
		ContentView _container;

		protected sealed override View CreateContents()
		{
			_titleLabel = new Label
			{
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
				TextColor = MvvmApp.Current.Colors.Get(FluidConfig.DefaultPopupTitleColor),
				BackgroundColor = MvvmApp.Current.Colors.Get(FluidConfig.DefaultPopupTitleBackgroundColor),
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,

			}.BindTo(Label.TextProperty, nameof(ViewModel.Title));

			_container = new ContentView();
			var layout = new RelativeLayout();
			var titleHeight = MvvmApp.Current.Sizes.Get(FluidConfig.DefaultPopupTitleHeight);

			layout.Children.Add(_titleLabel, () => new Rectangle(0, 0, layout.Width, titleHeight));
			layout.Children.Add(_container, () => new Rectangle(0, titleHeight, layout.Width, layout.Height - titleHeight));

			_container.Content = CreatePopupContents();

			return layout;
		}

		/// <summary>
		/// Creates the popup contents.
		/// </summary>
		/// <returns>The popup contents.</returns>
		protected abstract View CreatePopupContents();
	}
}
