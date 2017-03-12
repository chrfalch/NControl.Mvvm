using System;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;
using NControl.XAnimation;

namespace NControl.Mvvm
{
	public enum CollectionState
	{
		NotLoaded,
		Loading,
		Loaded,
		Reloading
	}

	public class ListViewControl : Grid
	{
		#region Members
		readonly ListViewEx _listView;
		readonly ContentView _emptyMessageView;
		readonly ContentView _loadingView;
		readonly Command _refreshCommand;

		#endregion

		public ListViewControl()
		{
			// Create refresh command
			_refreshCommand = new Command((arg) => {

				if (State != CollectionState.Loading && 
				    RefreshCommand != null && 
				    RefreshCommand.CanExecute(null))
					RefreshCommand.Execute(null);

			}, (arg) => State != CollectionState.Loading);

			// Setup listview
			_listView = new ListViewEx { 
				BindingContext = this, 
				RefreshCommand = _refreshCommand,
				SeparatorVisibility= SeparatorVisibility.None,
			}
				.BindTo(ItemsView<Cell>.ItemsSourceProperty, nameof(ItemsSource))
				.BindTo(ListView.IsRefreshingProperty, nameof(IsRefreshing))
				.BindTo(ListViewEx.ItemSelectedCommandProperty, nameof(ItemSelectedCommand))
				.BindTo(ListView.IsPullToRefreshEnabledProperty, nameof(IsPullToRefreshEnabled));

			// Setup default empty message view
			_emptyMessageView = new ContentView { Opacity = 0.0 };

			// Setup default loading view
			_loadingView = new ContentView { Opacity = 0.0 };

			// Add default values to the loading view and empty view
			_loadingView.Content = new ActivityIndicator { IsRunning = true };

			// Set up empty message
			_emptyMessageView.Content = new VerticalWizardStackLayout
			{
				Children = {
					new Label
					{
						BindingContext = this,
						HorizontalTextAlignment = TextAlignment.Center,
						VerticalTextAlignment = TextAlignment.Center,
					}.BindTo(Label.TextProperty, nameof(EmptyMessageText)),

					new ExtendedButton{
						BindingContext = this,
						Command = new Command((obj) => {
							if (State != CollectionState.Loading &&
								RefreshCommand != null &&
								RefreshCommand.CanExecute(null))
							{
								AnimateVisibility(true, _loadingView);
								RefreshCommand.Execute(null);
							}
						}),
					}.BindTo(Button.TextProperty, nameof(ReloadButtonText)),
				}
			};

			// Add to self
			Children.Add(_listView);
			Children.Add(_emptyMessageView);
			Children.Add(_loadingView);
		}

		#region Properties

		public View EmptyListView
		{
			get { return _emptyMessageView.Content; }
			set { _emptyMessageView.Content = value; }
		}

		public View LoadingView
		{
			get { return _loadingView.Content; }
			set { _loadingView.Content = value; }
		}

		public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
			nameof(ItemsSource), typeof(IEnumerable), typeof(ListViewControl), null,
			BindingMode.OneWay);

		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
			nameof(ItemTemplate), typeof(DataTemplate), typeof(ListViewControl), null,
			BindingMode.OneWay);

		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		public static readonly BindableProperty IsPullToRefreshEnabledProperty = BindableProperty.Create(
			nameof(IsPullToRefreshEnabled), typeof(bool), typeof(ListViewControl), false, BindingMode.TwoWay);

		public bool IsPullToRefreshEnabled
		{
			get { return (bool)GetValue(IsPullToRefreshEnabledProperty); }
			set { SetValue(IsPullToRefreshEnabledProperty, value); }
		}

		public static readonly BindableProperty RefreshCommandProperty = BindableProperty.Create(
			nameof(RefreshCommand), typeof(ICommand), typeof(ListViewControl), null, BindingMode.OneWay);

		public ICommand RefreshCommand
		{
			get { return (ICommand)GetValue(RefreshCommandProperty); }
			set { SetValue(RefreshCommandProperty, value); }
		}

		public static readonly BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(
			nameof(ItemSelectedCommand), typeof(ICommand), typeof(ListViewControl), null, BindingMode.OneWay);

		public ICommand ItemSelectedCommand
		{
			get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
			set { SetValue(ItemSelectedCommandProperty, value); }
		}

		public static readonly BindableProperty ActionCommandProperty = BindableProperty.Create(
			nameof(ActionCommand), typeof(ICommand), typeof(ListViewControl), null, BindingMode.OneWay);

		public ICommand ActionCommand
		{
			get { return (ICommand)GetValue(ActionCommandProperty); }
			set { SetValue(ActionCommandProperty, value); }
		}

		public static readonly BindableProperty ActionTextProperty = BindableProperty.Create(
			nameof(ActionText), typeof(string), typeof(ListViewControl), null, BindingMode.OneWay);

		public string ActionText
		{
			get { return (string)GetValue(ActionTextProperty); }
			set { SetValue(ActionTextProperty, value); }
		}

		public static BindableProperty EmptyMessageTextProperty =
			BindableProperty.Create(nameof(EmptyMessageText), typeof(string), typeof(ListViewControl), "No Items Found",
				BindingMode.OneWay);

		public string EmptyMessageText
		{
			get { return (string)GetValue(EmptyMessageTextProperty); }
			set { SetValue(EmptyMessageTextProperty, value); }
		}

		public static BindableProperty ReloadButtonTextProperty =
			BindableProperty.Create(nameof(ReloadButtonText), typeof(string), typeof(ListViewControl), "Reload",
				BindingMode.OneWay);

		public string ReloadButtonText
		{
			get { return (string)GetValue(ReloadButtonTextProperty); }
			set { SetValue(ReloadButtonTextProperty, value); }
		}

		public static BindableProperty StateProperty =
			BindableProperty.Create(nameof(State), typeof(CollectionState), typeof(ListViewControl), CollectionState.Loading,
			BindingMode.OneWay, propertyChanged: (bindable, oldValue, newValue) =>
			{

				var ctrl = (ListViewControl)bindable;
				ctrl.UpdateState((CollectionState)oldValue, (CollectionState)newValue);

			});

		public CollectionState State
		{
			get { return (CollectionState)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		#endregion

		#region Private Members

		/// <summary>
		/// Update collection state
		/// </summary>
		void UpdateState(CollectionState oldValue, CollectionState newValue)
		{
			if (oldValue == newValue)
				return;

			switch (newValue)
			{
				case CollectionState.NotLoaded:
					AnimateVisibility(true, _loadingView);
					AnimateVisibility(false, _emptyMessageView);
					break;
					
				case CollectionState.Loaded:

					AnimateVisibility(
						ItemsSource == null || 
						(ItemsSource is ICollection && (ItemsSource as ICollection).Count == 0),
						_emptyMessageView);

					AnimateVisibility(false, _loadingView);

					IsRefreshing = false;
					_refreshCommand.ChangeCanExecute();

					break;

				case CollectionState.Loading:	
					AnimateVisibility(false, _emptyMessageView);
					break;
					
				case CollectionState.Reloading:
					break;
					
			}
		}
		#endregion

		#region Private Properties

		public static readonly BindableProperty IsRefreshingProperty = BindableProperty.Create(
			nameof(IsRefreshing), typeof(bool), typeof(ListViewControl), false, BindingMode.TwoWay);

		public bool IsRefreshing
		{
			get { return (bool)GetValue(IsRefreshingProperty); }
			set { SetValue(IsRefreshingProperty, value); }
		}

		void AnimateVisibility(bool setVisibleTo, VisualElement element)
		{
			if (setVisibleTo && element.Opacity.Equals(0.0))
			{
				new XAnimationPackage(element)
					.Duration(50)
					.Opacity(1.0)
					.Animate()
					.Run();
			}
			else if (!setVisibleTo && element.Opacity.Equals(1.0))
			{ 
				new XAnimationPackage(element)
					.Duration(50)
					.Opacity(0.0)
					.Animate()
					.Run();
			}
		}
		#endregion
	}
}
