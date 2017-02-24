using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public class FluidNavigationBar: StackLayout
	{
		readonly Grid _contentGrid;
		readonly StackLayout _leftViewContainer;
		readonly StackLayout _rightViewContainer;

		public FluidNavigationBar()
		{
			_leftViewContainer = new StackLayout { 
				Orientation = StackOrientation.Horizontal,
				Spacing = 4,
				Padding = new Thickness(12, 2),
				HorizontalOptions = LayoutOptions.StartAndExpand,
			};

			_rightViewContainer = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Spacing = 4,
				Padding = new Thickness(12, 2),
				HorizontalOptions = LayoutOptions.EndAndExpand,
			};

			_contentGrid = new Grid();
			_contentGrid.Children.Add(_leftViewContainer);
			_contentGrid.Children.Add(_rightViewContainer);
			_contentGrid.Children.Add(new Label
			{
				HeightRequest = 44,
				BindingContext = this,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
				BackgroundColor = Color.White,
			}.BindTo(Label.TextProperty, nameof(Title)));

			Children.Add(_contentGrid);
			Children.Add(new BoxView
			{
				HeightRequest = 0.5,
				BackgroundColor = Color.Gray,
			});
		}

		#region Properties

		/// <summary>
		/// The title property.
		/// </summary>
		public static BindableProperty TitleProperty =
			BindableProperty.Create(nameof(Title), typeof(string), typeof(FluidNavigationBar), null,
				BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		/// <summary>
		/// The ToolbarItems property.
		/// </summary>
		public static BindableProperty ToolbarItemsProperty =
			BindableProperty.Create(nameof(ToolbarItems), typeof(IList<ToolbarItem>), typeof(FluidNavigationBar), null,
				BindingMode.OneWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (FluidNavigationBar)bindable;

					if (oldValue != null && oldValue is INotifyCollectionChanged)
						(oldValue as INotifyCollectionChanged).CollectionChanged -= ctrl.HandleToolbarItemsCollectionChanged;

					if (newValue != null && newValue is INotifyCollectionChanged)
						(newValue as INotifyCollectionChanged).CollectionChanged += ctrl.HandleToolbarItemsCollectionChanged;
				});

		/// <summary>
		/// Gets or sets the ToolbarItems of the FluidNavigationBar instance.
		/// </summary>
		public IList<ToolbarItem> ToolbarItems
		{
			get { return (IList<ToolbarItem>)GetValue(ToolbarItemsProperty); }
			set { SetValue(ToolbarItemsProperty, value); }
		}
		#endregion

		#region Events

		/// <summary>
		/// Toolbar items changed
		/// </summary>
		void HandleToolbarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			_rightViewContainer.Children.Clear();
			foreach (var item in (Parent as IToolbarItemsContainer).ToolbarItems)
			{
				if (item is ToolbarItemEx)
				{
					// Handle custom toolbar item
				}
			}
		}
		#endregion
	}

	/// <summary>
	/// Extended toolbar item with support for views, icons etc.
	/// </summary>
	public class ToolbarItemEx : ToolbarItem
	{
		public string FontAwesomeIcon { get; set; }
		public string MaterialDesignIcon { get; set; }
		public View View { get; set; }
	}
}
