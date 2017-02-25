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

		public FluidNavigationBar()
		{
			BackgroundColor = Color.White;
			Spacing = 0;
			Padding = 0;

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
				TextColor = Color.Accent,
				BindingContext = this,
				WidthRequest = 44,
				HorizontalTextAlignment = TextAlignment.Start,

			}//.BindTo(IsVisibleProperty, nameof(BackButtonVisible))
			 .AddBehaviorTo(new BounceAndClickBehavior(new Command(_ =>
			 {
				 if (BackButtonCommand != null && BackButtonCommand.CanExecute(null))
					 BackButtonCommand.Execute(null);
			 })));

			_leftViewContainer.Children.Add(_backButton);
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
