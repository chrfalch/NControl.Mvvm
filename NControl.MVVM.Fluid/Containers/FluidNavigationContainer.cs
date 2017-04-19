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

	public class FluidNavigationContainer : ContentView, INavigationContainer, IXAnimatable
	{
		#region Private Members

		double _xstart;

		protected readonly RelativeLayout _layout;
		protected readonly ContentView _container;
		protected readonly Grid _navigationContainer;
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

			_statusbarHeight = FluidConfig.DefaultStatusbarHeight;
			_navigationBarHeight = FluidConfig.DefaultNavigationBarHeight;

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
						(_container.Content as IView).GetViewModel().PresentationMode);
			});

			_container = new ContentView();

			_navigationContainer = new Grid
			{
				Padding = 0,
				RowSpacing = 0,
				ColumnSpacing = 0,
				Children = {
					
					new FluidShadowView{
						VerticalOptions = LayoutOptions.End,
						HasShadow = true,
						ShadowRadius = 3,
						ShadowOpacity = 0.4,
						ShadowOffset = new Size(0, 1.0),
						BackgroundColor = Color.Transparent,
						Content = _navigationBar,
					},
				},
			};

			_AddViewsToBottomOfStack(_layout);

			// Add contents container
			_layout.Children.Add(_container, () => GetContainerRectangle());

			// Add navigation container
			_layout.Children.Add(_navigationContainer, () => GetNavigationBarRectangle());
			_layout.Children.Add(new BoxView
			{
				BackgroundColor = Config.PrimaryColor,
				VerticalOptions = LayoutOptions.Start,
			}, () => new Rectangle(0, 0, _layout.Width, _statusbarHeight));

			_AddViewsToTopOfStack(_layout);

			// Bindings
			this.BindTo(TitleProperty, nameof(IViewModel.Title));

			// Recognizer
			//var recognizer = new GestureRecognizerBehavior();
			//recognizer.Touched += Recognizer_Touched;
			//Behaviors.Add(recognizer);
		}

		~FluidNavigationContainer()
		{
			System.Diagnostics.Debug.WriteLine(this.GetType().Name + " Finalized.");
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

		void Recognizer_Touched(object sender, GestureRecognizerEventArgs e)
		{
			// UpdateFromGestureRecognizer(e.FirstPoint.X, e.Velocity, e.TouchType);
		}

		/// <summary>
		/// Callback from gesture recognizer in platform code
		/// </summary>
		public void UpdateFromGestureRecognizer(double x, double velocity, TouchType state)
		{
			var fromView = NavigationContext.Elements.Last();
			var index = NavigationContext.Elements.ToList().IndexOf(fromView);
			var toView = index > 0 ? NavigationContext.Elements.ElementAt(index - 1) : null;

			if (toView == null)
				return;
			
			var animateNavigation = GetViewHasNavigationBar(toView.View) != GetViewHasNavigationBar(fromView.View);

			switch (state)
			{
				case TouchType.Start:					
					_xstart = x;
					break;

				case TouchType.Moving:

					toView.View.TranslationX = Math.Max(0, x - _xstart);

					if (fromView != null)
						fromView.View.TranslationX = Math.Max(-(Width / 4), -(Width / 4) +
							(x - _xstart)/4);

					if (animateNavigation)
					{
						var distanceToAnimate = (_navigationBarHeight);
						var distanceAnimated = x - _xstart;
						var factor = distanceAnimated / Width;

						if (GetViewHasNavigationBar(fromView.View))
						{
							_navigationContainer.TranslationY = -distanceToAnimate + (distanceToAnimate * factor);
						}
						else
						{
							_navigationContainer.TranslationY = -(distanceToAnimate * factor);
						}
					}
					
					break;

				case TouchType.Ended:

					CheckTranslationAndSnap(-1, fromView.View, toView.View);

					break;
				case TouchType.Cancelled:

					toView.View.TranslationX = 0;

					if (fromView != null)
						fromView.View.TranslationX = -(Width / 4);
					
					break;
			}
		}

		#endregion

		#region INavigationContainer

		/// <summary>
		/// Add a new child to the container
		/// </summary>
		public void SetContent(View content)
		{
			if (content == null)
			{
				_container.Content = null;
				BindingContext = null;
                UpdateToolbarItems(null);
				return;
			}

			if (!(content is IView))
				throw new ArgumentException("Content must implement IView");
			
			_container.Content = content;
			BindingContext = (content as IView).GetViewModel();
			UpdateToolbarItems(content);
			OnPropertyChanged(nameof(BackButtonVisible));
		}

		/// <summary>
		/// Gets the root view.
		/// </summary>
		/// <returns>The root view.</returns>
		public View GetBaseView() { return this; }

		/// <summary>
		/// Gets or sets the navigation context
		/// </summary>
		public NavigationContext NavigationContext { get; set; }

		/// <summary>
		/// Returns true if backbutton should be visible
		/// </summary>
		public bool BackButtonVisible 
		{ 
			get { return NavigationContext != null ?
				NavigationContext.Elements.Count > 1 : false; } 
		}

		#endregion

		#region Transitions (IXAnimatable)

		public View GetChromeView()
		{
			return _navigationContainer;
		}

		public View GetContentsView()
		{
			return _container;
		}

		public virtual View GetOverlayView()
		{
			return null;
		}

		/// <summary>
		/// Transition a new view in 
		/// </summary>
		public virtual IEnumerable<XAnimationPackage> TransitionIn(
			INavigationContainer fromContainer, PresentationMode presentationMode)
		{
			var animations = new List<XAnimationPackage>();

			if (presentationMode == PresentationMode.Default)
			{
				// Animate the new contents in
				animations.Add(new XAnimationPackage(this)
					.Translate(Width, 0)
					.Set()
	               	.Translate(0, 0));

				// Move previous a litle bit out
				animations.Add(new XAnimationPackage(fromContainer.GetBaseView())
               		.Translate(-(Width * 0.25), 0));
			}
			else if (presentationMode == PresentationMode.Modal)
			{
				// Animate the new contents in
				animations.Add(new XAnimationPackage(this)
					.Translate(0, Height)
					.Set()
					.Duration(350)
					.Easing(EasingFunction.EaseIn)
            		.Translate(0, 0));
			}
			else if (presentationMode == PresentationMode.Popup)
			{
				// Animate the new contents in
				animations.Add(new XAnimationPackage(this)
					.Translate(0, Height)
					.Set()
	               	.Translate(0, 0));
			}

			// Additional animations?
			if (_container.Content is IXViewAnimatable)
					return (_container.Content as IXViewAnimatable).TransitionIn(
					fromContainer, this, animations, presentationMode);

			return animations;

		}

		/// <summary>
		/// Transitions out.
		/// </summary>
		public virtual IEnumerable<XAnimationPackage> TransitionOut(INavigationContainer toContainer, 
        	PresentationMode presentationMode)
		{
			var animations = new List<XAnimationPackage>();

			if (presentationMode == PresentationMode.Default)
			{
				// Animate the new contents in
				animations.Add(new XAnimationPackage(this)
					.Translate(Width, 0)
					.Animate());

				// Move previous a litle bit out
				animations.Add(new XAnimationPackage(toContainer.GetBaseView()).Translate(0, 0));
			}
			else if (presentationMode == PresentationMode.Modal)
			{
				// Animate
				animations.Add(new XAnimationPackage(this)
				    .Easing(EasingFunction.EaseIn)
	                .Translate(0, Height));
			}
			else if (presentationMode == PresentationMode.Popup)
			{
				
			}

			// Additional animations?
			if (_container.Content is IXViewAnimatable)
				return (_container.Content as IXViewAnimatable).TransitionOut(toContainer, this,
					animations, presentationMode);

			return animations;
		}

		#endregion

		#region Private Members

		void CheckTranslationAndSnap(double velocity, View toView, View fromView)
		{			
			double toViewTranslationX = 0.0;
			double fromViewTranslationX = toView != null ? toView.TranslationX : 0;

			var offset = fromView.TranslationX % (-1 * Width);
			bool resetTransformations = true;

			if (offset > Width * 0.33)
			{
				toViewTranslationX = Width;
				fromViewTranslationX = 0;
				resetTransformations = true;
			}
			
			var distance = toViewTranslationX - fromView.TranslationX;
			var duration = Math.Min(0.2, Math.Max(0.2, velocity.Equals(-1) ? 0.2f : distance / velocity));

			new XAnimationPackage(toView)
				.Duration((long)(duration*1000))
				.Translate(toViewTranslationX, 0)
				.Animate()
				.Run(() => {
					if (fromViewTranslationX.Equals(0));
					// TODO:
					//MvvmApp.Current.Presenter.DismissViewModelAsync(
					//	GetViewModel().PresentationMode, false, false);
			});

			if(toView != null)
				new XAnimationPackage(fromView)
					.Duration((long)(duration * 1000))
					.Translate(fromViewTranslationX, 0)
					.Animate()
					.Run();			
		}

		void UpdateToolbarItems(View view)
		{
			_navigationBar.ToolbarItems.Clear();

			if (view == null)
				return;
			
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
				   		.Translate(0, -(_navigationBarHeight))
						.Animate()
						.Opacity(0.0)
						.Set()};
			

			_navigationContainer.TranslationY = -(_navigationBarHeight);
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

			_navigationContainer.TranslationY = -(_navigationBarHeight);
			return new XAnimationPackage[0];
		}

		Rectangle GetNavigationBarRectangle()
		{
			return new Rectangle(0, _statusbarHeight, _layout.Width, _navigationBarHeight);
		}

		Rectangle GetContainerRectangle()
		{
			var topView = _container.Content;
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

			return "FluidNavigationContainer: " + _container.Content.GetType().Name;
		}
	}
}
