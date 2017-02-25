using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public class FluidNavigationContainer : ContentView, 
		INavigationContainer, IXAnimatable
	{
		#region Private Members

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

			var statusbarHeight = 22;
			var navigationBarHeight = 44;

			_navigationBar = new FluidNavigationBar { BindingContext = this, }
				.BindTo(FluidNavigationBar.TitleProperty, nameof(Title))
				.BindTo(FluidNavigationBar.BackButtonVisibleProperty, nameof(BackButtonVisible));

			// Back button command
			_navigationBar.BackButtonCommand = new AsyncCommand(async _=> 
				await MvvmApp.Current.Presenter.DismissViewModelAsync(GetViewModel().PresentationMode));
			
			_container = new Grid();

			_layout.Children.Add(_navigationBar, () => new Rectangle(
				0, statusbarHeight, _layout.Width, navigationBarHeight));

			_layout.Children.Add(_container, () => new Rectangle(
				0, statusbarHeight + navigationBarHeight, _layout.Width,
				_layout.Height - (statusbarHeight + navigationBarHeight)));

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

		#region INavigationContainer

		/// <summary>
		/// Add a new child to the container
		/// </summary>
		public void AddChild(View view)
		{			
			_container.Children.Add(view);
			BindingContext = GetViewModel();

			OnPropertyChanged(nameof(BackButtonVisible));
		}

		/// <summary>
		/// Remove a view from the container
		/// </summary>
		public void RemoveChild(View view)
		{
			OnPropertyChanged(nameof(BackButtonVisible));

			if (!_container.Children.Contains(view))
				throw new ArgumentException("View not part of child collection.");

			_container.Children.Remove(view);

			BindingContext = GetViewModel();
		}

		/// <summary>
		/// Returns number of children in container
		/// </summary>
		public int Count { get { return _container == null ? 0 : _container.Children.Count; } }

		/// <summary>
		/// Returns true if backbutton should be visible
		/// </summary>
		public bool BackButtonVisible { get { return Count > 1; } }

		#endregion

		#region Transitions

		/// <summary>
		/// Transition a new view in 
		/// </summary>
		public virtual IEnumerable<XAnimation.XAnimation> TransitionIn(
			View view, PresentationMode presentationMode)
		{
			if (presentationMode == PresentationMode.Default)
			{
				var index = _container.Children.IndexOf(view);
				var fromView = index > 0 ? _container.Children.ElementAt(index - 1) : null;
	
				// Animate the new contents in
				var animateContentsIn = new XAnimation.XAnimation(new[] { view });
				animateContentsIn
					.Translate(Width, 0)
					.Set()
					.Translate(0, 0);

				// Move previous a litle bit out
				var animatePreviousOut = new XAnimation.XAnimation(new[] { fromView });
				animatePreviousOut
					.Translate(-(Width / 4), 0);

				return new[] { animateContentsIn, animatePreviousOut };
			}
			else if (presentationMode == PresentationMode.Modal)
			{
				// Animate the new contents in
				var animateContentsIn = new XAnimation.XAnimation(new[] { view });
				animateContentsIn
					.Translate(0, Height)
					.Set()
					.Translate(0, 0);

				return new[] { animateContentsIn };
			}
			else if (presentationMode == PresentationMode.Popup)
			{
				// Animate the new contents in
				var animateContentsIn = new XAnimation.XAnimation(new[] { view });
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
		public virtual IEnumerable<XAnimation.XAnimation> TransitionOut(View view, PresentationMode presentationMode)
		{
			if (presentationMode == PresentationMode.Default)
			{
				var index = _container.Children.IndexOf(view);
				var toView = index > 0 ? _container.Children.ElementAt(index - 1) : null;

				// Animate
				return new[]
				{
					new XAnimation.XAnimation(new[] { view }).Translate(Width, 0),
					new XAnimation.XAnimation(new[] { toView }).Translate(0, 0)
				};
			}

			if (presentationMode == PresentationMode.Modal)
			{
				// Animate
				return new[]
				{
					new XAnimation.XAnimation(new[] { view }).Translate(0, Height),				
				};
			}

			return null;
		}

		# endregion

		IViewModel GetViewModel()
		{
			var current = _container.Children.LastOrDefault() as IView;
			if (current != null)
				return current.GetViewModel();

			return null;
		}
	}
}
