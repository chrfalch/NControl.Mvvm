using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
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

		protected readonly RelativeLayout _layout;
		protected readonly Grid _container;
		protected readonly StackLayout _navigationContainer;
		protected readonly FluidNavigationBar _navigationBar;

		double _statusbarHeight;
		double _navigationBarHeight;

		#endregion

		/// <summary>
		/// Constructs a new instance of the Navigation Container
		/// </summary>
		public FluidNavigationContainer()
		{
			Content = _layout = new RelativeLayout();

			_statusbarHeight = MvvmApp.Current.Sizes.Get(FluidConfig.DefaultStatusbarHeight);
			_navigationBarHeight = MvvmApp.Current.Sizes.Get(FluidConfig.DefaultNavigationBarHeight);

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

			_navigationContainer = new StackLayout
			{
				Padding = 0,
				Spacing = 0,
				Orientation = StackOrientation.Vertical,
				Children = {
					new BoxView {
						BackgroundColor = MvvmApp.Current.Colors.Get(Config.PrimaryColor),
						HeightRequest = _statusbarHeight,
					},

					_navigationBar,

					new FluidShadowView{
						HasShadow = true,
						ShadowRadius = 1,
						ShadowOpacity = 0.9,
						ShadowOffset = new Size(0, 1.0),
						HeightRequest = 0.5,
						BackgroundColor = Color.Transparent,
					}
				},
			};

			_AddViewsToBottomOfStack(_layout);

			// Add contents container
			_layout.Children.Add(_container, () => GetContainerRectangle());

			// Add navigation container
			_layout.Children.Add(_navigationContainer, () => GetNavigationBarRectangle());

			_AddViewsToTopOfStack(_layout);

			// Bindings
			this.BindTo(TitleProperty, nameof(IViewModel.Title));
		}

		#region Protected Members

		void _AddViewsToBottomOfStack(RelativeLayout layout)
		{
			AddViewsToBottomOfStack(layout);
		}

		void _AddViewsToTopOfStack(RelativeLayout layout)
		{
			AddViewsToTopOfStack(layout);
		}

		protected virtual void AddViewsToBottomOfStack(RelativeLayout layout)
		{
		}

		protected virtual void AddViewsToTopOfStack(RelativeLayout layout)
		{
		}

		#endregion

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

		/// <summary>
		/// Callback from gesture recognizer in platform code
		/// </summary>
		public void UpdateFromGestureRecognizer(double x, double velocity, PanState state)
		{
			var view = _container.Children.Last();
			var index = _container.Children.IndexOf(view);
			var fromView = index > 0 ? _container.Children.ElementAt(index - 1) : null;

			var animateNavigation = GetViewHasNavigationBar(view) != GetViewHasNavigationBar(fromView);

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

					if (animateNavigation)
					{
						var distanceToAnimate = (_statusbarHeight + _navigationBarHeight);
						var distanceAnimated = x - _xstart;
						var factor = distanceAnimated / Width;

						if (GetViewHasNavigationBar(fromView))
						{
							_navigationContainer.TranslationY = -distanceToAnimate + (distanceToAnimate * factor);
						}
						else
						{
							_navigationContainer.TranslationY = -(distanceToAnimate * factor);
						}
					}
					
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

		#region Transitions (IXAnimatable)

		public View GetNavigationBarView()
		{
			return _navigationContainer;
		}

		public View GetContainerView()
		{
			return _container;
		}

		public View GetChild(int index)
		{
			return _container.Children.ElementAt(index);
		}

		public virtual View GetOverlayView()
		{
			return null;
		}

		/// <summary>
		/// Transition a new view in 
		/// </summary>
		public virtual IEnumerable<XAnimationPackage> TransitionIn(
			View view, PresentationMode presentationMode)
		{
			if (presentationMode == PresentationMode.Default)
			{
				var animations = new List<XAnimationPackage>();

				var index = _container.Children.IndexOf(view);
				var fromView = index > 0 ? _container.Children.ElementAt(index - 1) : null;

				// Navigation bar?
				if (GetViewHasNavigationBar(fromView) != GetViewHasNavigationBar(view))
				{
					if (GetViewHasNavigationBar(view))
						animations.AddRange(ShowNavigationBar(true));					
					else
						animations.AddRange(HideNavigationbar(true));					
				}

				// Animate the new contents in
				animations.Add(new XAnimationPackage(view)
					.Translate(Width, 0)
					.Set()
	               	.Translate(0, 0));

				// Move previous a litle bit out
				animations.Add(new XAnimationPackage(fromView).Translate(-(Width / 4), 0));

				if (view is IXViewAnimatable)
					return (view as IXViewAnimatable).TransitionIn(view, this, animations, presentationMode);

				return animations;

			}
			else if (presentationMode == PresentationMode.Modal)
			{
				// Animate the new contents in
				var animateContentsIn = new XAnimationPackage(view);
				animateContentsIn
					.Translate(0, Height)
					.Set()
					.Duration(350)
					.Easing(EasingFunction.EaseIn)
					.Translate(0, 0);

				var retVal = new[] { animateContentsIn };

				var child = GetChild(0);
				if (child is IXViewAnimatable)
					return (child as IXViewAnimatable).TransitionIn(child, this, retVal, presentationMode);

				return retVal;
			}
			else if (presentationMode == PresentationMode.Popup)
			{
				// Animate the new contents in
				var animateContentsIn = new XAnimationPackage(view);
				animateContentsIn
					.Translate(0, Height)
					.Set()
					.Translate(0, 0);

				var retVal = new[] { animateContentsIn };

				if (view is IXViewAnimatable)
					return (view as IXViewAnimatable).TransitionIn(view, this, retVal, presentationMode);

				return retVal;
			}

			return null;
		}

		/// <summary>
		/// Transitions the out.
		/// </summary>
		public virtual IEnumerable<XAnimationPackage> TransitionOut(View view, PresentationMode presentationMode)
		{
			if (presentationMode == PresentationMode.Default)
			{
				var animations = new List<XAnimationPackage>();

				var index = _container.Children.IndexOf(view);
				var toView = index > 0 ? _container.Children.ElementAt(index - 1) : null;

				// Navigation bar?
				if (GetViewHasNavigationBar(toView) != GetViewHasNavigationBar(view))
				{
					if (GetViewHasNavigationBar(toView))
						animations.AddRange(ShowNavigationBar(true));					
					else
						animations.AddRange(HideNavigationbar(true));					
				}

				// Animate
				animations.Add(new XAnimationPackage(view).Translate(Width, 0));
				animations.Add(new XAnimationPackage(toView).Translate(0, 0));

				var child = GetChild(0);
				if (child is IXViewAnimatable)
					return (child as IXViewAnimatable).TransitionOut(child, this, animations, presentationMode);

				return animations;
			}

			if (presentationMode == PresentationMode.Modal)
			{
				// Animate
				var retVal = new[]
				{
					new XAnimationPackage(view)
						.Easing(EasingFunction.EaseOut)
						.Translate(0, Height),
				};

				var child = GetChild(0);
				if (child is IXViewAnimatable)
					return (child as IXViewAnimatable).TransitionOut(child, this, retVal, presentationMode);

				return retVal;
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
			var toView = index > 0 ? _container.Children.ElementAt(index - 1) : null;

			double toViewTranslationX = 0.0;
			double fromViewTranslationX = toView != null ? toView.TranslationX : 0;

			var offset = view.TranslationX % (-1 * Width);
			bool resetTransformations = true;

			if (offset > Width * 0.33)
			{
				toViewTranslationX = Width;
				fromViewTranslationX = 0;
				resetTransformations = true;
			}
			
			var distance = toViewTranslationX - view.TranslationX;
			var duration = Math.Min(0.2, Math.Max(0.2, velocity.Equals(-1) ? 0.2f : distance / velocity));

			new XAnimationPackage(view)
				.Duration((long)(duration*1000))
				.Translate(toViewTranslationX, 0)
				.Animate()
				.Run(() => {
				if(fromViewTranslationX.Equals(0))
					MvvmApp.Current.Presenter.DismissViewModelAsync(
						GetViewModel().PresentationMode, false, false);
			});

			if(toView != null)
				new XAnimationPackage(toView)
					.Duration((long)(duration * 1000))
					.Translate(fromViewTranslationX, 0)
					.Animate()
					.Run();

			if (GetViewHasNavigationBar(toView) != GetViewHasNavigationBar(view))
			{
				if (GetViewHasNavigationBar(toView))
				{
					new XAnimationPackage(_navigationContainer)
					   	.Translate(0, resetTransformations ? -(_statusbarHeight + _navigationBarHeight):0)
						.Animate().Run();
				}
				else
				{
					new XAnimationPackage(_navigationContainer)
						.Translate(0, resetTransformations ?  0: -(_statusbarHeight + _navigationBarHeight))
						.Animate().Run();
				}
			}
		}

		void UpdateToolbarItems(View view)
		{
			_navigationBar.ToolbarItems.Clear();
			var toolbarItemsProvider = view as IToolbarItemsContainer;
			if (toolbarItemsProvider != null)
				_navigationBar.ToolbarItems.AddRange(toolbarItemsProvider.ToolbarItems);
		}

		protected bool GetViewHasNavigationBar(View view)
		{
			return view == null || (bool)view.GetValue(NavigationPage.HasNavigationBarProperty);
		}

		protected IEnumerable<XAnimationPackage> HideNavigationbar(bool animated)
		{
			if (animated)
				return new[]{new XAnimationPackage(_navigationContainer)
				   		.Translate(0, -(_statusbarHeight + _navigationBarHeight))
						.Animate()
						.Opacity(0.0)
						.Set()};
			

			_navigationContainer.TranslationY = -(_statusbarHeight + _navigationBarHeight);
			return new XAnimationPackage[0];
		}

		protected IEnumerable<XAnimationPackage> ShowNavigationBar(bool animated)
		{
			if (animated)
				return new[]{new XAnimationPackage(_navigationContainer)
						.Opacity(1.0)
						.Set()
				   		.Translate(0, 0)
						.Animate()};

			_navigationContainer.TranslationY = -(_statusbarHeight + _navigationBarHeight);
			return new XAnimationPackage[0];
		}

		Rectangle GetNavigationBarRectangle()
		{
			return new Rectangle(0, 0, _layout.Width, _statusbarHeight + _navigationBarHeight);
		}

		Rectangle GetContainerRectangle()
		{
			var topView = _container.Children.LastOrDefault();
			var hasNavigationBar = GetViewHasNavigationBar(topView);

			return new Rectangle(
				0, _statusbarHeight + (hasNavigationBar ? _navigationBarHeight : 0), _layout.Width,
				_layout.Height - (_statusbarHeight + (hasNavigationBar ? _navigationBarHeight : 0)));
		}

		#endregion

		/// <summary>
		/// Helper
		/// </summary>
		public override string ToString()
		{
			if (_container == null)
				return "Empty";

			return "FluidNavigationContainer: " + string.Join(", ", _container.Children.Select(v => v.GetType().Name));
		}
	}
}
