using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xamarin.Forms;
using NControl.Controls;
using System.Windows.Input;

namespace NControl.Mvvm.Fluid
{
	public class FluidNavigationBar: StackLayout
	{
		readonly Grid _contentGrid;
		readonly StackLayout _leftViewContainer;
		readonly StackLayout _rightViewContainer;
		readonly ContentView _titleContentView;
		readonly FontMaterialDesignLabel _backButton;
		readonly ObservableCollectionWithAddRange<ToolbarItem> _toolbarItems =
			new ObservableCollectionWithAddRange<ToolbarItem>();

		public FluidNavigationBar()
		{
			BackgroundColor = Color.Accent;
			Spacing = 0;
			Padding = 0;

			_toolbarItems.CollectionChanged += HandleToolbarItemsCollectionChanged;

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
			_titleContentView = new ContentView
			{
				Content = new Label
				{
					HeightRequest = 44,
					InputTransparent = true,
					BindingContext = this,
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
					TextColor = Color.Accent,

				}.BindTo(Label.TextProperty, nameof(Title))
				 .BindTo(Label.TextColorProperty, nameof(TintColor))
			};

			_contentGrid.Children.Add(_titleContentView);
			_contentGrid.Children.Add(_leftViewContainer);
			_contentGrid.Children.Add(_rightViewContainer);

			Children.Add(_contentGrid);
			Children.Add(new BoxView
			{
				HeightRequest = 0.5,
				BackgroundColor = Color.Gray,
			});

			_backButton = new FontMaterialDesignLabel
			{
				Text = FontMaterialDesignLabel.MDArrowLeft,
				Opacity = BackButtonVisible ? 1.0 : 0.0,
				BindingContext = this,
				HorizontalTextAlignment = TextAlignment.Start,

			}.BindTo(Label.TextColorProperty, nameof(TintColor))
			 .AddBehaviorTo(new BounceAndClickBehavior(new Command(_ =>
			 {
				 if (BackButtonCommand != null && BackButtonCommand.CanExecute(null))
					 BackButtonCommand.Execute(null);
			 })));

			_leftViewContainer.Children.Add(new FluidToolbarControl(_backButton));
		}

		#region Properties

		public static BindableProperty TintColorProperty = BindableProperty.Create(
			nameof(TintColor), typeof(Color), typeof(FluidNavigationBar), 
			Color.White, BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the tint color.
		/// </summary>
		public Color TintColor
		{
			get { return (Color)GetValue(TintColorProperty); }
			set { SetValue(TintColorProperty, value); }
		}

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

		public static BindableProperty BackButtonCommandProperty = BindableProperty.Create(
			nameof(BackButtonCommand), typeof(ICommand), typeof(FluidNavigationBar), null,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the backbutton command.
		/// </summary>
		/// <value>The title.</value>
		public ICommand BackButtonCommand
		{
			get { return (ICommand)GetValue(BackButtonCommandProperty); }
			set { SetValue(BackButtonCommandProperty, value); }
		}

		/// <summary>
		/// The title property.
		/// </summary>
		public static BindableProperty BackButtonVisibleProperty = BindableProperty.Create(
			nameof(BackButtonVisible), typeof(bool), typeof(FluidNavigationBar),
			false, BindingMode.OneWay, null, (bindable, oldValue, newValue) => {
			var ctrl = bindable as FluidNavigationBar;

			if (newValue != oldValue)
			{
				if((bool)newValue)
					new XAnimation.XAnimation(ctrl._backButton)
					  .Opacity(1.0)
		              .Animate()
		              .Run();
				else
					new XAnimation.XAnimation(ctrl._backButton)
					  .Opacity(0.0)
					  .Animate()
		              .Run();
			}
		});

		/// <summary>
		/// Gets or sets if the back button is visible
		/// </summary>
		public bool BackButtonVisible
		{
			get { return (bool)GetValue(BackButtonVisibleProperty); }
			set { SetValue(BackButtonVisibleProperty, value); }
		}

		/// <summary>
		/// Gets or sets the ToolbarItems of the FluidNavigationBar instance.
		/// </summary>
		public ObservableCollectionWithAddRange<ToolbarItem> ToolbarItems
		{
			get { return _toolbarItems; }
		}

		#endregion

		#region Events

		/// <summary>
		/// Toolbar items changed
		/// </summary>
		void HandleToolbarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			_rightViewContainer.Children.Clear();
			if (ToolbarItems == null)
				return;
			
			foreach (var item in ToolbarItems)
			{
				FluidToolbarControl toolbarControl = null;

				if (item is ToolbarItemEx)
				{
					// Handle custom toolbar item
					if (((ToolbarItemEx)item).View != null)
						toolbarControl = new FluidToolbarControl(((ToolbarItemEx)item).View);
					else if (!string.IsNullOrEmpty(((ToolbarItemEx)item).FontAwesomeIcon))
						toolbarControl = new FluidToolbarControl(new FontAwesomeLabel
						{
							Text = ((ToolbarItemEx)item).FontAwesomeIcon,
							TextColor = TintColor,
						});

					else if (!string.IsNullOrEmpty(((ToolbarItemEx)item).MaterialDesignIcon))
						toolbarControl = new FluidToolbarControl(new FontMaterialDesignLabel
						{
							Text = ((ToolbarItemEx)item).MaterialDesignIcon,
							TextColor = TintColor,
						});
				}
				else
				{
					// Icon
					if (!string.IsNullOrEmpty(item.Icon))
						toolbarControl = new FluidToolbarControl(new Image
						{
							Source = item.Icon,
						});

					// Text
					else
						toolbarControl = new FluidToolbarControl(new Label {
							Text = item.Text,
							TextColor = TintColor,
						});
				}

				if (toolbarControl != null)
				{
					toolbarControl.AddBehaviorTo(new BounceAndClickBehavior(
						item.Command, item.CommandParameter));
					
					_rightViewContainer.Children.Add(toolbarControl);
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
