using System;
using System.Collections;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;
using NControl.XAnimation;
using NControl.Mvvm;
using System.Threading.Tasks;

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
		readonly BaseFluidActivityIndicator _activityIndicator;
		readonly List<XAnimationPackage> _animationQueue = new List<XAnimationPackage>();

		Action _animationDoneCallback;
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public ListViewControl()
		{
			// Create refresh command
			_refreshCommand = new Command((arg) =>
			{
				if (State != CollectionState.Loading &&
					RefreshCommand != null &&
					RefreshCommand.CanExecute(null))
					// Execute command!
					RefreshCommand.Execute(null);

			}, (arg) => State != CollectionState.Loading);

			// Setup listview
			_listView = new ListViewEx
			{
				BindingContext = this,
				RefreshCommand = _refreshCommand,
			}
				.BindTo(ItemsView<Cell>.ItemsSourceProperty, nameof(ItemsSource))
				.BindTo(ItemsView<Cell>.ItemTemplateProperty, nameof(ItemTemplate))
				.BindTo(ListView.IsRefreshingProperty, nameof(IsRefreshing))
				.BindTo(ListViewEx.ItemSelectedCommandProperty, nameof(ItemSelectedCommand))
				.BindTo(ListView.IsPullToRefreshEnabledProperty, nameof(IsPullToRefreshEnabled))
				.BindTo(ListView.RowHeightProperty, nameof(RowHeight))
				.BindTo(ListView.HasUnevenRowsProperty, nameof(HasUnevenRows))
				.BindTo(ListView.SeparatorVisibilityProperty, nameof(SeparatorVisibility));

			// Setup default empty message view
			_emptyMessageView = new ContentView()
			{
			};

			_activityIndicator = Container.Resolve<IActivityIndicator>()
			                              .CreateActivityIndicator() as BaseFluidActivityIndicator;

			// Setup default loading view
			_loadingView = new ContentView
			{
				InputTransparent = true,
				BackgroundColor = Color.Transparent,
				Content = new VerticalStackLayoutWithPadding
				{
					VerticalOptions = LayoutOptions.CenterAndExpand,
					Children =  { _activityIndicator }
				}
			};

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

			UpdateVisibleControls();
		}

		#region Properties

		public static BindableProperty SeparatorVisibilityProperty = BindableProperty.Create(
			nameof(SeparatorVisibility), typeof(SeparatorVisibility), typeof(ListViewControl), SeparatorVisibility.None,
			BindingMode.OneWay);

		public SeparatorVisibility SeparatorVisibility
		{
			get { return (SeparatorVisibility)GetValue(SeparatorVisibilityProperty); }
			set { SetValue(SeparatorVisibilityProperty, value); }
		}

		public static BindableProperty RowHeightProperty = BindableProperty.Create(
			nameof(RowHeight), typeof(int), typeof(ListViewControl), ListView.RowHeightProperty.DefaultValue,
			BindingMode.OneWay);

		public int RowHeight
		{
			get { return (int)GetValue(RowHeightProperty); }
			set { SetValue(RowHeightProperty, value); }
		}

		public static BindableProperty HasUnevenRowsProperty = BindableProperty.Create(
			nameof(HasUnevenRows), typeof(bool), typeof(ListViewControl), false,
			BindingMode.OneWay);

		public bool HasUnevenRows
		{
			get { return (bool)GetValue(HasUnevenRowsProperty); }
			set { SetValue(HasUnevenRowsProperty, value); }
		}
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

		/// <summary>
		/// The CollectionState property.
		/// </summary>
		public static BindableProperty StateProperty = BindableProperty.Create(
			nameof(State), typeof(CollectionState), typeof(ListViewControl), 
			CollectionState.NotLoaded, BindingMode.OneWay, null, (bindable, oldValue, newValue) =>
			{				
				var ctrl = (ListViewControl)bindable;
				ctrl.UpdateState((CollectionState)oldValue, (CollectionState)newValue);
			}
		);

		/// <summary>
		/// Gets or sets the CollectionState of the ListViewControl instance.
		/// </summary>
		public CollectionState State
		{
			get { return (CollectionState)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}

		/// <summary>
		/// Updates the state and waits for any animations and 
		/// transitions.
		/// </summary>
		public Task SetCollectionStateAsync(CollectionState state)
		{
			State = state;
			return Task.FromResult(true);
		}

		public static readonly BindableProperty IsRefreshingProperty = BindableProperty.Create(
			nameof(IsRefreshing), typeof(bool), typeof(ListViewControl), false, BindingMode.TwoWay);

		public bool IsRefreshing
		{
			get { return (bool)GetValue(IsRefreshingProperty); }
			set { SetValue(IsRefreshingProperty, value); }
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

			UpdateVisibleControls();
		}

		void UpdateVisibleControls()
		{
			switch (State)
			{
				case CollectionState.NotLoaded:

					// Show nothing
					_activityIndicator.IsRunning =  false;
					SetVisibility(_emptyMessageView, false);
					break;

				case CollectionState.Loading:

					_activityIndicator.IsRunning = !IsRefreshing;
					AnimateVisibility(_emptyMessageView, false);

					break;
					
				case CollectionState.Loaded:

					// Do we have any items in the list?
					var listIsEmpty = ItemsSource == null ||
						(ItemsSource is ICollection &&
						(ItemsSource as ICollection).Count == 0);

					_activityIndicator.IsRunning = false;
					AnimateVisibility(_emptyMessageView, listIsEmpty);

					IsRefreshing = false;
					_refreshCommand.ChangeCanExecute();

					break;
					
				case CollectionState.Reloading:
					break;

			}
		}
		#endregion

		#region Private Properties

		void SetVisibility(VisualElement element, bool setVisibleTo)
		{
			element.IsVisible = setVisibleTo;
			element.Opacity = setVisibleTo ? 1.0 : 0.0;
		}

		void AnimateVisibility(VisualElement element, bool setVisibleTo)
		{
			var animation = new XAnimationPackage(element);

			if (setVisibleTo)
				animation.Add((transform) => transform.SetOpacity(1.0));
			else 
				animation.Add((transform) => transform.SetOpacity(0.0));

			ProcessQueue(animation);
		}
		#endregion

		#region Private Members

		void ProcessQueue(XAnimationPackage animation = null)
		{
			XAnimationPackage nextAnimation = null;

			lock (_animationQueue)
			{
				if (animation != null)
					_animationQueue.Add(animation);

				// Any animations running?
				if (_animationQueue.Any(a => a.IsRunning))
				{
					return;
				}
				// Any animation in list?
				if (!_animationQueue.Any())
				{
					if (_animationDoneCallback != null)
					{
						var tmp = _animationDoneCallback;
						_animationDoneCallback = null;
						tmp?.Invoke();
					}
					return;
				}

				// Nope, none running, one or more waiting. Lets start first in list
				nextAnimation = _animationQueue.First();
			}

			var el = nextAnimation.GetElement(0);
			var op = nextAnimation.GetTransform(0).Opacity;

			var elName = el == _listView ? "listview" :
				el == _loadingView ? "loadingview" :
				el == _emptyMessageView ? "emptymessage" : "unknown";

			if (nextAnimation.HasViewsToAnimate)
			{
				if (op.Equals(1.0) && el.Opacity.Equals(0.0))
					el.IsVisible = true;
				
				// Start it
				nextAnimation.SetDuration(150).Animate(() =>
				{
					lock (_animationQueue)
					{
						el.IsVisible = op.Equals(1.0);

						// Remove the animation 
						_animationQueue.Remove(nextAnimation);

						// Process
						ProcessQueue();
					}
				});
			}
			else
			{
				el.Opacity = op;
				el.IsVisible = op.Equals(1.0);

				lock (_animationQueue)
				{
					// Remove the animation 
					_animationQueue.Remove(nextAnimation);

					// Process
					ProcessQueue();
				}
			}
		}

		#endregion
	}
}
