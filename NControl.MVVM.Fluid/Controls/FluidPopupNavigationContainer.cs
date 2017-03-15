using System;
using System.Collections.Generic;
using System.Linq;
using NControl.XAnimation;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidPopupNavigationContainer : ContentView, INavigationContainer, IXAnimatable
	{
		readonly RelativeLayout _layout;
		readonly FluidBlurOverlay _overlay;
		readonly Grid _container;

		public FluidPopupNavigationContainer()
		{
			Content = _layout = new RelativeLayout();

			// Create overlay
			_overlay = new FluidBlurOverlay();

			_layout.Children.Add(_overlay, () => _layout.Bounds);

			// Create container
			_container = new Grid { 
				BackgroundColor = MvvmApp.Current.Colors.Get(Config.ViewBackgroundColor),
				Children = {
					new Frame {  OutlineColor = Color.Red, HasShadow = false, Opacity = 1.0 },
				},
			};

			_layout.Children.Add(_container, () => GetContentSize());
		}

		#region Properties

		public static BindableProperty OverlayBackgroundColorProperty = BindableProperty.Create(
			nameof(OverlayBackgroundColor), typeof(Color),
			typeof(FluidPopupNavigationContainer), MvvmApp.Current.Colors.Get(Config.ViewTransparentBackgroundColor),
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
			nameof(ContentSize), typeof(Size), typeof(FluidPopupNavigationContainer), Size.Zero,
			BindingMode.OneWay, propertyChanged: (bindable, oldValue, newValue) =>
			 {
				 var ctrl = (FluidPopupNavigationContainer)bindable;
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
			var retVal = new[] {
				new XAnimationPackage(_overlay)
					  .Opacity(0.0)
					  .Set()
					  .Opacity(1.0)
					  .Animate(),

				new XAnimationPackage(_container)
					.Translate(0, Height)
					.Set()
					.Translate(0, 0)
					.Animate()
			};

			var child = GetChild(0);
			if (child is IXViewAnimatable)
				return (child as IXViewAnimatable).TransitionIn(child, this, retVal, presentationMode);

			return retVal;
		}

		public IEnumerable<XAnimationPackage> TransitionOut(
			View view, PresentationMode presentationMode)
		{
			var retVal = new[] {
				new XAnimationPackage(_overlay)
					  .Opacity(0.0)
					  .Animate(),

				new XAnimationPackage(_container)
					.Translate(0, Height)
					.Animate()
			};

			var child = GetChild(0);
			if (child is IXViewAnimatable)
				return (child as IXViewAnimatable).TransitionOut(child, this, retVal, presentationMode);

			return retVal;
		}

		#endregion

		#region Navigation Container

		public void AddChild(View view, PresentationMode presentationMode)
		{
			_container.Children.Add(view);
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

		public View GetNavigationBarView()
		{
			return null;
		}

		public View GetContainerView()
		{
			return _container;
		}

		public View GetChild(int index)
		{
			return _container.Children.ElementAt(index);
		}

		public View GetOverlayView()
		{
			return _overlay;
		}

		#endregion

		Rectangle GetContentSize()
		{
			var lastView = _container.Children.LastOrDefault() as IContentSizeProvider;
			if (lastView == null || lastView.ContentSize == Size.Zero)
				return new Rectangle(
						(_layout.Width / 2) - (ContentSize.Width / 2),
						(_layout.Height / 2) - (ContentSize.Height / 2),
					ContentSize.Width, ContentSize.Height);

			return new Rectangle(
				(_layout.Width / 2) - (lastView.ContentSize.Width / 2),
				(_layout.Height / 2) - (lastView.ContentSize.Height / 2),
				lastView.ContentSize.Width, lastView.ContentSize.Height);
		}
	}
}
