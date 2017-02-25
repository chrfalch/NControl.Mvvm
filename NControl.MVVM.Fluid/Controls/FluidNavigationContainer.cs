using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

			_navigationBar = new FluidNavigationBar()
				.BindTo(FluidNavigationBar.TitleProperty, nameof(IViewModel.Title));
			
			_container = new Grid();

			_layout.Children.Add(_navigationBar, () => new Rectangle(
				0, statusbarHeight, _layout.Width, navigationBarHeight));

			_layout.Children.Add(_container, () => new Rectangle(
				0, statusbarHeight + navigationBarHeight, _layout.Width,
				_layout.Height - (statusbarHeight + navigationBarHeight)));
		}

		#region INavigationContainer

		/// <summary>
		/// Add a new child to the container
		/// </summary>
		public void AddChild(View view)
		{			
			_container.Children.Add(view);
			BindingContext = GetViewModel();
		}

		/// <summary>
		/// Remove a view from the container
		/// </summary>
		public void RemoveChild(View view)
		{
			if (!_container.Children.Contains(view))
				throw new ArgumentException("View not part of child collection.");

			_container.Children.Remove(view);

			BindingContext = GetViewModel();
		}

		/// <summary>
		/// Returns number of children in container
		/// </summary>
		public int Count { get { return _container.Children.Count; } }

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

			//if (presentationMode == PresentationMode.Popup)
			//{
			//	// Animate
			//	return new[]
			//	{
			//		new XAnimation.XAnimation(new[] { this }).Translate(0, Height),
			//		new XAnimation.XAnimation(new[] { overlay }).Opacity(0.0)
			//	};
			//}

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
