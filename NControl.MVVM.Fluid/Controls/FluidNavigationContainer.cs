using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public enum PanState
	{
		Started,
		Moving,
		Ended,
		Cancelled,
	}

	public class FluidNavigationContainer : ContentView,
		INavigationContainer, IXAnimatable
	{
		#region Private Members

		double _xstart;

		readonly RelativeLayout _layout;
		readonly Grid _container;
		readonly FluidNavigationBar _navigationBar;

		#endregion

		/// <summary>
		/// Constructs a new instance of the Navigation Container
		/// </summary>
		public FluidNavigationContainer()
		{
			Content = _layout = new RelativeLayout();

			var statusbarHeight = Device.OnPlatform(22, 0, 22);
			var navigationBarHeight = 44;

			_navigationBar = new FluidNavigationBar { 
				BindingContext = this, 
			}
			.BindTo(FluidNavigationBar.TitleProperty, nameof(Title))
			.BindTo(FluidNavigationBar.BackButtonVisibleProperty, nameof(BackButtonVisible));

			// Back button command
			_navigationBar.BackButtonCommand = new AsyncCommand(async _ =>
			{
				if (BackButtonVisible)
					await MvvmApp.Current.Presenter.DismissViewModelAsync(
						GetViewModel().PresentationMode);
			});

			_container = new Grid();

			// Add statusbar container if necessary
			_layout.Children.Add(
				new BoxView { BackgroundColor = MvvmApp.Current.Colors.Get(Config.PrimaryDarkColor) },
			 	() => new Rectangle(0, 0, _layout.Width, statusbarHeight));

			// Add navigation bar
			_layout.Children.Add(_navigationBar, () => new Rectangle(
				0, statusbarHeight, _layout.Width, navigationBarHeight));

			// Add contents container
			_layout.Children.Add(_container, () => new Rectangle(
				0, statusbarHeight + navigationBarHeight, _layout.Width,
				_layout.Height - (statusbarHeight + navigationBarHeight)));

			// Bindings
			this.BindTo(TitleProperty, nameof(IViewModel.Title));
		}

		#region Properties

		/// <summary>
		/// The title property.
		/// </summary>
		public static BindableProperty TitleProperty = BindableProperty.Create(
			nameof(Title), typeof(string), typeof(FluidNavigationContainer),
			null, BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		#endregion

		#region Public Members

		public void UpdateFromGestureRecognizer(double x, double velocity, PanState state)
		{
			var view = _container.Children.Last();
			var index = _container.Children.IndexOf(view);
			var fromView = index > 0 ? _container.Children.ElementAt(index - 1) : null;

			switch (state)
			{
				case PanState.Started:					
					_xstart = x;
					break;

				case PanState.Moving:
					view.TranslationX = Math.Max(0, x - _xstart);

					if (fromView != null)
						fromView.TranslationX = Math.Max(-(Width / 4), -(Width / 4) +
							(x - _xstart)/4);
					
					break;

				case PanState.Ended:

					CheckTranslationAndSnap(velocity);

					break;
				case PanState.Cancelled:

					view.TranslationX = 0;

					if (fromView != null)
						fromView.TranslationX = -(Width / 4);
					
					break;
			}
		}
		#endregion

		#region INavigationContainer

		/// <summary>
		/// Add a new child to the container
		/// </summary>
		public void AddChild(View view, PresentationMode presentationMode)
		{
			_container.Children.Add(view);

			if (view is ILeftBorderProvider)
				(view as ILeftBorderProvider).IsLeftBorderVisible = _container.Children.Count > 1;

			BindingContext = GetViewModel();
			UpdateToolbarItems(view);
			OnPropertyChanged(nameof(BackButtonVisible));
		}

		/// <summary>
		/// Helper
		/// </summary>
		public override string ToString()
		{
			if (_container == null)
				return "Empty";

			return string.Join(", ", _container.Children.Select(v => v.GetType().Name));
		}

		/// <summary>
		/// Remove a view from the container
		/// </summary>
		public void RemoveChild(View view, PresentationMode presentationMode)
		{			
			if (!_container.Children.Contains(view))
				throw new ArgumentException("View not part of child collection.");

			_container.Children.Remove(view);

			BindingContext = GetViewModel();
			UpdateToolbarItems(_container.Children.LastOrDefault());
			OnPropertyChanged(nameof(BackButtonVisible));		
		}

		/// <summary>
		/// Returns number of children in container
		/// </summary>
		public int Count { get { return _container == null ? 0 : _container.Children.Count; } }

		/// <summary>
		/// Gets the root view.
		/// </summary>
		/// <returns>The root view.</returns>
		public View GetRootView()
		{
			return this;
		}

		/// <summary>
		/// Returns true if backbutton should be visible
		/// </summary>
		public bool BackButtonVisible { get { return Count > 1; } }

		#endregion

		#region Transitions

		/// <summary>
		/// Transition a new view in 
		/// </summary>
		public virtual IEnumerable<XAnimationPackage> TransitionIn(
			View view, PresentationMode presentationMode)
		{
			if (presentationMode == PresentationMode.Default)
			{
				var index = _container.Children.IndexOf(view);
				var fromView = index > 0 ? _container.Children.ElementAt(index - 1) : null;

				// Animate the new contents in
				var animateContentsIn = new XAnimation.XAnimationPackage(new[] { view });
				animateContentsIn
					.Translate(Width, 0)
					.Set()
					.Translate(0, 0);

				// Move previous a litle bit out
				var animatePreviousOut = new XAnimation.XAnimationPackage(new[] { fromView });
				animatePreviousOut
					.Translate(-(Width / 4), 0);

				return new[] { animateContentsIn, animatePreviousOut };
			}
			else if (presentationMode == PresentationMode.Modal)
			{
				// Animate the new contents in
				var animateContentsIn = new XAnimation.XAnimationPackage(new[] { view });
				animateContentsIn
					.Translate(0, Height)
					.Set()
					.Translate(0, 0);

				return new[] { animateContentsIn };
			}
			else if (presentationMode == PresentationMode.Popup)
			{
				// Animate the new contents in
				var animateContentsIn = new XAnimation.XAnimationPackage(new[] { view });
				animateContentsIn
					.Translate(0, Height)
					.Set()
					.Translate(0, 0);

				return new[] { animateContentsIn };
			}

			return null;
		}

		/// <summary>
		/// Transitions the out.
		/// </summary>
		public virtual IEnumerable<XAnimation.XAnimationPackage> TransitionOut(View view, PresentationMode presentationMode)
		{
			if (presentationMode == PresentationMode.Default)
			{
				var index = _container.Children.IndexOf(view);
				var toView = index > 0 ? _container.Children.ElementAt(index - 1) : null;

				// Animate
				return new[]
				{
					new XAnimation.XAnimationPackage(new[] { view }).Translate(Width, 0),
					new XAnimation.XAnimationPackage(new[] { toView }).Translate(0, 0)
				};
			}

			if (presentationMode == PresentationMode.Modal)
			{
				// Animate
				return new[]
				{
					new XAnimation.XAnimationPackage(new[] { view }).Translate(0, Height),
				};
			}

			return null;
		}

		#endregion

		#region Private Members

		IViewModel GetViewModel()
		{
			var current = _container.Children.LastOrDefault() as IView;
			if (current != null)
				return current.GetViewModel();

			return null;
		}

		void CheckTranslationAndSnap(double velocity)
		{
			var view = _container.Children.Last();
			var index = _container.Children.IndexOf(view);
			var fromView = index > 0 ? _container.Children.ElementAt(index - 1) : null;

			double toViewTranslationX = 0.0;
			double fromViewTranslationX = fromView != null ? fromView.TranslationX : 0;

			var offset = view.TranslationX % (-1 * Width);

			if (offset > Width * 0.33)
			{
				toViewTranslationX = Width;
				fromViewTranslationX = 0;
			}
			
			var distance = toViewTranslationX - view.TranslationX;
			var duration = Math.Min(0.2, Math.Max(0.2, velocity.Equals(-1) ? 0.2f : distance / velocity));

			new XAnimation.XAnimationPackage(view)
				.Duration((long)(duration*1000))
				.Translate(toViewTranslationX, 0)
				.Animate()
				.Run(() => {
				if(fromViewTranslationX.Equals(0))
					MvvmApp.Current.Presenter.DismissViewModelAsync(
						GetViewModel().PresentationMode, false, false);
			});

			if(fromView != null)
				new XAnimation.XAnimationPackage(fromView)
					.Duration((long)(duration * 1000))
					.Translate(fromViewTranslationX, 0)
					.Animate()
					.Run();
		}

		void UpdateToolbarItems(View view)
		{
			_navigationBar.ToolbarItems.Clear();
			var toolbarItemsProvider = view as IToolbarItemsContainer;
			if (toolbarItemsProvider != null)
				_navigationBar.ToolbarItems.AddRange(toolbarItemsProvider.ToolbarItems);
		}

		#endregion
	}
}
