using System;
using System.Collections.Generic;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm.Fluid
{
	public class FluidModalContainer : ContentView, INavigationContainer, IXAnimatable
	{
		readonly RelativeLayout _layout;
		readonly BoxView _overlay;
		readonly Grid _container;

		public FluidModalContainer()
		{
			Content = _layout = new RelativeLayout();

			// Create overlay
			_overlay = new BoxView { 
				BindingContext = this,
			}.BindTo(BackgroundColorProperty, nameof(OverlayBackgroundColor));

			_layout.Children.Add(_overlay, () => _layout.Bounds);

			// Create container
			_container = new Grid();
			_layout.Children.Add(_container, () => new Rectangle(
				(_layout.Width / 2) - (ContentSize.Width / 2),
				(_layout.Height / 2) - (ContentSize.Height / 2),
				ContentSize.Width, ContentSize.Height));
		}

		#region Properties

		public static BindableProperty OverlayBackgroundColorProperty =
			BindableProperty.Create(nameof(OverlayBackgroundColor), typeof(Color),
									typeof(FluidModalContainer), Color.Gray.MultiplyAlpha(0.4),
				BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the background color for the overlay
		/// </summary>
		public Color OverlayBackgroundColor
		{
			get { return (Color)GetValue(OverlayBackgroundColorProperty); }
			set { SetValue(OverlayBackgroundColorProperty, value); }
		}

		public static BindableProperty ContentSizeProperty = BindableProperty.Create(
			nameof(ContentSize), typeof(Size), typeof(FluidModalContainer), Size.Zero,               
			BindingMode.OneWay, propertyChanged:(bindable, oldValue, newValue) =>
			{
				var ctrl = (FluidModalContainer)bindable;
				ctrl._layout.ForceLayout();
			});

		/// <summary>
		/// Contents size
		/// </summary>
		public Size ContentSize
		{
			get { return (Size)GetValue(ContentSizeProperty); }
			set { SetValue(ContentSizeProperty, value); }
		}

		#endregion

		#region Transitions

		public IEnumerable<XAnimationPackage> TransitionIn(
			View view, PresentationMode presentationMode)
		{
			return new[] {
				new XAnimationPackage(_overlay)
					  .Opacity(0.0)
					  .Set()
					  .Opacity(0.4)
					  .Animate(),

				new XAnimationPackage(_container)
					.Translate(0, Height)
					.Set()
					.Translate(0, 0)
					.Animate()
			};
		}

		public IEnumerable<XAnimation.XAnimationPackage> TransitionOut(
			View view, PresentationMode presentationMode)
		{
			return new[] {
				new XAnimationPackage(_overlay)
					  .Opacity(0.0)
					  .Animate(),

				new XAnimationPackage(_container)
					.Translate(0, Height)
					.Animate()
			};
		}

		#endregion

		#region Navigation Container

		public void AddChild(View view, PresentationMode presentationMode)
		{
			_container.Children.Add(view);

			if (view is ILeftBorderProvider)
				(view as ILeftBorderProvider).IsLeftBorderVisible = _container.Children.Count > 1;
		}

		public void RemoveChild(View view, PresentationMode presentationMode)
		{
			_container.Children.Remove(view);
		}

		public int Count
		{
			get { return _container.Children.Count; }
		}

		public View GetRootView() { return this; }

		#endregion

	}
}
