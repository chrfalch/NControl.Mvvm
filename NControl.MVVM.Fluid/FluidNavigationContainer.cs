using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public class FluidNavigationContainer : ContentView
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

		#region Public Members

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

		#endregion

		#region Properties

		#endregion

		IViewModel GetViewModel()
		{
			var current = _container.Children.LastOrDefault() as IView;
			if (current != null)
				return current.GetViewModel();

			return null;
		}
	}
}
