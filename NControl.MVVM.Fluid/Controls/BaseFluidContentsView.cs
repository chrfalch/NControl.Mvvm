using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Windows.Input;
using NControl.Mvvm;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public interface IToolbarItemsContainer: IView
	{
		IList<ToolbarItem> ToolbarItems { get; }
	}

	public abstract class BaseFluidContentsView<TViewModel> : ContentView, IView<TViewModel>,
		IToolbarItemsContainer, ILeftBorderProvider, IXViewAnimatable
		where TViewModel : BaseViewModel
	{

		#region Private Members

		readonly RelativeLayout _layout;
		readonly BoxView _leftBorder;
		readonly ObservableCollectionWithAddRange<ToolbarItem> _toolbarItems = new ObservableCollectionWithAddRange<ToolbarItem>();
		readonly ICommand _onAppearingCommand;
		readonly ICommand _onDisappearingCommand;
		readonly List<PropertyChangeListener> _propertyChangeListeners = new List<PropertyChangeListener>();

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor
		/// </summary>
		public BaseFluidContentsView()
		{
			BackgroundColor = MvvmApp.Current.Colors.Get(Config.ViewBackgroundColor);

			// Main layout
			_layout = new RelativeLayout();
			_leftBorder = new BoxView { BackgroundColor = MvvmApp.Current.Colors.Get(Config.BorderColor) };

			// OnAppearing/OnDisappearing
			_onAppearingCommand = new AsyncCommand(async (obj) => await ViewModel.OnAppearingAsync());
			_onDisappearingCommand = new AsyncCommand(async (obj) => await ViewModel.OnDisappearingAsync());

			_toolbarItems.CollectionChanged += ToolbarItems_CollectionChanged;

			Setup();
		}

		/// <summary>
		/// Setup this instance.
		/// </summary>
		private void Setup()
		{
			// Image provider
			ImageProvider = Container.Resolve<IImageProvider>();

			// Set up viewmodel and viewmodel values
			ViewModel = Container.Resolve<TViewModel>();
			BindingContext = ViewModel;

			// Bind title
			this.SetBinding(Page.TitleProperty, NameOf(vm => vm.Title));

			// Loading/Progress overlay

			var contents = CreateContents();
			_layout.Children.Add(contents, () => _layout.Bounds);

			// Set our content to be the relative layout with progress overlays etc.
			Content = _layout;

			// Add left border
			_layout.Children.Add(_leftBorder, () => new Rectangle(0, 0, 0.5, _layout.Height));

			// Listen for changes to busy
			ListenForPropertyChange(mn => mn.IsBusy, () =>
			{
				MvvmApp.Current.ActivityIndicator.UpdateProgress(ViewModel.IsBusy, ViewModel.IsBusyText, ViewModel.IsBusySubTitle);
			});

			ListenForPropertyChange(mn => mn.IsBusyText, () =>
			{
				MvvmApp.Current.ActivityIndicator.UpdateProgress(ViewModel.IsBusy, ViewModel.IsBusyText, ViewModel.IsBusySubTitle);
			});

			ListenForPropertyChange(mn => mn.IsBusySubTitle, () =>
			{
				MvvmApp.Current.ActivityIndicator.UpdateProgress(ViewModel.IsBusy, ViewModel.IsBusyText, ViewModel.IsBusySubTitle);
			});
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Hide/Shows the left border
		/// </summary>
		/// <value><c>true</c> if left border is visible; otherwise, <c>false</c>.</value>
		public bool IsLeftBorderVisible
		{
			get { return _leftBorder.IsVisible; }
			set { _leftBorder.IsVisible = value; }
		}

		/// <summary>
		/// Returns the toolbar items
		/// </summary>
		/// <value>The toolbar items.</value>
		public IList<ToolbarItem> ToolbarItems { get { return _toolbarItems; } }

		/// <summary>
		/// Returns the ViewModel
		/// </summary>
		public TViewModel ViewModel { get; private set; }

		/// <summary>
		/// Gets the view model.
		/// </summary>
		/// <returns>The view model.</returns>
		public IViewModel GetViewModel()
		{
			return ViewModel as IViewModel;
		}

		#endregion

		#region View LifeCycle

		/// <summary>
		/// Raised when the view has appeared on screen.
		/// </summary>
		public void OnAppearing()
		{
			_onAppearingCommand.Execute(null);
		}

		/// <summary>
		/// Raises the disappearing event.
		/// </summary>
		public void OnDisappearing()
		{
			_onDisappearingCommand.Execute(null);
		}

		/// <summary>
		/// Application developers can override this method to provide behavior when the back button is pressed.
		/// </summary>
		protected bool OnBackButtonPressed()
		{
			if (DefaultBackButtonBehaviour)
				return true;

			if (ViewModel.BackButtonCommand != null &&
			   ViewModel.BackButtonCommand.CanExecute(null))
				ViewModel.BackButtonCommand.Execute(null);

			return true;
		}

		#endregion

		#region Protected Members

		/// <summary>
		/// Gets or sets a value indicating whether this
		/// BaseContentsView has default back button behaviour.
		/// </summary>
		/// <value><c>true</c> if default back button behaviour; otherwise, <c>false</c>.</value>
		protected bool DefaultBackButtonBehaviour { get; set; }

		/// <summary>
		/// Gets the image provide.
		/// </summary>
		/// <value>The image provide.</value>
		protected IImageProvider ImageProvider { get; private set; }

		/// <summary>
		/// Implement to create the layout on the page
		/// </summary>
		/// <returns>The layout.</returns>
		protected abstract View CreateContents();

		/// <summary>
		/// Sets the background image.
		/// </summary>
		/// <param name="imageName">Image name.</param>
		protected void SetBackgroundImage(string imageName)
		{
			// Background image
			var image = new Image
			{
				Source = imageName,
				Aspect = Aspect.AspectFill,
			};

			_layout.Children.Add(image, () => _layout.Bounds);
		}

		/// <summary>
		/// Returns the name of the member
		/// </summary>
		/// <returns>The of.</returns>
		public string NameOf(Expression<Func<TViewModel, object>> property)
		{
			return PropertyNameHelper.GetPropertyName<TViewModel>(property);
		}

		/// <summary>
		/// Returns the name of the member
		/// </summary>
		/// <returns>The of.</returns>
		public string NameOf<TModel>(Expression<Func<TModel, object>> property)
		{
			return PropertyNameHelper.GetPropertyName<TModel>(property);
		}

		/// <summary>
		/// Listens for property change.
		/// </summary>
		/// <param name="property">Property.</param>
		protected void ListenForPropertyChange(Expression<Func<TViewModel, object>> property, Action callback)
		{
			var changeListener = new PropertyChangeListener();
			changeListener.Listen<TViewModel>(property, ViewModel, callback);
			_propertyChangeListeners.Add(changeListener);
		}
		#endregion

		#region Toolbar Items

		/// <summary>
		/// Toolbar items collection changed.
		/// </summary>
		void ToolbarItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action != NotifyCollectionChangedAction.Add)
				return;

		}

		#endregion

		#region IXViewAnimatable

		public IEnumerable<XAnimationPackage> TransitionIn(View view, INavigationContainer container, IEnumerable<XAnimationPackage> animations, PresentationMode presentationMode)
		{
			switch (presentationMode)
			{				
				case PresentationMode.Modal:
					return ModalTransitionIn(container, animations);
				case PresentationMode.Popup:
					return PopupTransitionIn(container, animations);
				
				case PresentationMode.Default:
				default:
					return DefaultTransitionIn(container, animations);
			}
		}

		public IEnumerable<XAnimationPackage> TransitionOut(View view, INavigationContainer container, IEnumerable<XAnimationPackage> animations, PresentationMode presentationMode)
		{
			switch (presentationMode)
			{
				case PresentationMode.Modal:
					return ModalTransitionOut(container, animations);
				case PresentationMode.Popup:
					return PopupTransitionOut(container, animations);

				case PresentationMode.Default:
				default:
					return DefaultTransitionOut(container, animations);
			}
		}

		/// <summary>
		/// Override to provide custom default transition code for the view or its container
		/// </summary>
		protected virtual IEnumerable<XAnimationPackage> DefaultTransitionIn(INavigationContainer container, IEnumerable<XAnimationPackage> animations)
		{
			return animations;
		}

		/// <summary>
		/// Override to provide custom modal transition code for the view or its container
		/// </summary>
		protected virtual IEnumerable<XAnimationPackage> ModalTransitionIn(INavigationContainer container, IEnumerable<XAnimationPackage> animations)
		{
			return animations;
		}

		/// <summary>
		/// Override to provide custom popup transition code for the view or its container
		/// </summary>
		protected virtual IEnumerable<XAnimationPackage> PopupTransitionIn(INavigationContainer container, IEnumerable<XAnimationPackage> animations)
		{
			return animations;
		}

		/// <summary>
		/// Override to provide custom default transition code for the view or its container
		/// </summary>
		protected virtual IEnumerable<XAnimationPackage> DefaultTransitionOut(INavigationContainer container, IEnumerable<XAnimationPackage> animations)
		{
			return animations;
		}

		/// <summary>
		/// Override to provide custom modal transition code for the view or its container
		/// </summary>
		protected virtual IEnumerable<XAnimationPackage> ModalTransitionOut(INavigationContainer container, IEnumerable<XAnimationPackage> animations)
		{
			return animations;
		}

		/// <summary>
		/// Override to provide custom popup transition code for the view or its container
		/// </summary>
		protected virtual IEnumerable<XAnimationPackage> PopupTransitionOut(INavigationContainer container, IEnumerable<XAnimationPackage> animations)
		{
			return animations;
		}

		#endregion
	}
}