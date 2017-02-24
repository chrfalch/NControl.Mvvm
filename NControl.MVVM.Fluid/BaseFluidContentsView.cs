﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Windows.Input;
using NControl.Mvvm;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public interface IToolbarItemsContainer: IView
	{
		IList<ToolbarItem> ToolbarItems { get; }
	}

	public abstract class BaseFluidContentsView<TViewModel> : ContentView, IView<TViewModel>, IToolbarItemsContainer, IXAnimatable
		where TViewModel : BaseViewModel
	{

		#region Private Members

		readonly RelativeLayout _layout;
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
			BackgroundColor = Color.White;

			// Main layout
			_layout = new RelativeLayout();

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
		/// Returns the toolbar items
		/// </summary>
		/// <value>The toolbar items.</value>
		public IList<ToolbarItem> ToolbarItems {get{return _toolbarItems;}}

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

		#region Transitions

		public virtual IEnumerable<XAnimation.XAnimation> TransitionIn(View fromView, View overlay, PresentationMode presentationMode)
		{
			if (presentationMode == PresentationMode.Default)
			{
				var animateContentsIn = new XAnimation.XAnimation(new[] { this });
				animateContentsIn
					.Translate(Width, 0)
					.Set()
					.Translate(0, 0);

				var animatePreviousOut = new XAnimation.XAnimation(new[] { fromView });
				animatePreviousOut
					.Translate(-(Width / 4), 0);

				return new[] { animateContentsIn, animatePreviousOut };
			}

			if (presentationMode == PresentationMode.Modal)
			{
				var animateContentsIn = new XAnimation.XAnimation(new[] { this });
				animateContentsIn
					.Translate(0, Height)
					.Set()
					.Translate(0, 0);

				var animatePreviousOut = new XAnimation.XAnimation(new[] { fromView });
				animatePreviousOut
					.Scale(0.75);

				return new[] { animateContentsIn, animatePreviousOut };

			}

			if (presentationMode == PresentationMode.Popup)
			{
				var animateContentsIn = new XAnimation.XAnimation(new[] { this });
				animateContentsIn					
					.Translate(0, Height)
					.Set()
					.Translate(0, 0);

				var animateOverlay = new XAnimation.XAnimation(new VisualElement[] { overlay });
				animateOverlay.Opacity(1.0);

				return new[] { animateContentsIn, animateOverlay }; 
			}

			return null;
		}

		public virtual IEnumerable<XAnimation.XAnimation> TransitionOut(View toView, View overlay, PresentationMode presentationMode)
		{
			if (presentationMode == PresentationMode.Default)
			{
				// Animate
				return new[] 
				{ 
					new XAnimation.XAnimation(new[] { this }).Translate(Width, 0),
					new XAnimation.XAnimation(new[] { toView }).Translate(0, 0)
				};
			}

			if (presentationMode == PresentationMode.Modal)
			{
				// Animate
				return new[]
				{
					new XAnimation.XAnimation(new[] { this }).Translate(0, Height),
					new XAnimation.XAnimation(new[] { toView }).Scale(1.0)
				};
			}

			if (presentationMode == PresentationMode.Popup)
			{
				// Animate
				return new[]
				{
					new XAnimation.XAnimation(new[] { this }).Translate(0, Height),
					new XAnimation.XAnimation(new[] { overlay }).Opacity(0.0)
				};
			}

			return null;
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
	}
}