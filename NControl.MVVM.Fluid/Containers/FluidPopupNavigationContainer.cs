using System;
using System.Collections.Generic;
using System.Linq;
using NControl.XAnimation;
using NControl.Controls;
using Xamarin.Forms;

namespace NControl.Mvvm
{
	public class FluidPopupNavigationContainer : ContentView, INavigationContainer, IXAnimatable
	{
		readonly RelativeLayout _layout;
		readonly FluidBlurOverlay _overlay;
		readonly ContentView _container;
		readonly ContentView _containerBorders;
		readonly ContentView _containerContents;

		const int Duration = 150;

		public FluidPopupNavigationContainer()
		{
			Content = _layout = new RelativeLayout();

			// Create overlay
			_overlay = new FluidBlurOverlay();
			_layout.Children.Add(_overlay, () => _layout.Bounds);

			// Create container
			_container = new ContentView();
			_containerContents = new FluidRoundCornerView
			{
				BorderRadius = FluidConfig.DefaultPopupCornerRadius,
				BorderColor = Config.BorderColor,
				BorderWidth = Config.DefaultBorderSize,
				Content = _container,
			};

			_containerBorders = new FluidShadowView
			{
				HasShadow = true,
				BorderRadius = FluidConfig.DefaultPopupCornerRadius,
				Content = _containerContents
			};

			_layout.Children.Add(_containerBorders, () => GetContentSize());
		}

		#region Properties

		public static BindableProperty OverlayBackgroundColorProperty = BindableProperty.Create(
			nameof(OverlayBackgroundColor), typeof(Color),
			typeof(FluidPopupNavigationContainer), Config.ViewTransparentBackgroundColor,
			BindingMode.OneWay);

		/// <summary>
		/// Gets or sets the background color for the overlay
		/// </summary>
		public Color OverlayBackgroundColor
		{
			get { return (Color)GetValue(OverlayBackgroundColorProperty); }
			set { SetValue(OverlayBackgroundColorProperty, value); }
		}

		#endregion

		#region Transitions

		public IEnumerable<XAnimationPackage> TransitionIn(INavigationContainer fromContainer, PresentationMode presentationMode)
		{
			var retVal = new[] {
				new XAnimationPackage(_overlay)
					.Opacity(0.0)
					.Set()
					.Duration(150)
					.Easing(EasingFunction.EaseIn)
					.Opacity(1.0)
					.Then(),

				new XAnimationPackage(_containerBorders)
					.Scale(1.3)
					.Opacity(0.0)
					.Set()
					.Duration(Duration)
					.Easing(EasingFunction.EaseIn)
					.Opacity(1.0)
					.Scale(1.0)
					.Then()
			};

			if (GetContentsView() is IXViewAnimatable)
				return (GetContentsView() as IXViewAnimatable).TransitionIn(
					fromContainer, this, retVal, presentationMode);

			return retVal;
		}

		public IEnumerable<XAnimationPackage> TransitionOut(
			INavigationContainer toContainer, PresentationMode presentationMode)
		{
			var retVal = new[] {
				new XAnimationPackage(_overlay)
					.Opacity(0.0)
					.Duration(Duration)
					.Easing(EasingFunction.EaseOut)
					.Then(),

				new XAnimationPackage(_containerBorders)
					.Scale(1.3)
					.Opacity(0.0)
					.Duration(Duration)
					.Easing(EasingFunction.EaseOut)
					.Then()
			};

			if (GetContentsView() is IXViewAnimatable)
				return (GetContentsView() as IXViewAnimatable).TransitionOut(
					toContainer, this, retVal, presentationMode);

			return retVal;
		}

		#endregion

		#region Navigation Container

		public void SetContent(View content) 
		{ 
			if (content == null)
			{
				_container.Content = null;				       
				return;
			}

			if (!(content is IView))
				throw new ArgumentException("Content must implement IView");

			_container.Content = content;
		}

		public NavigationContext NavigationContext { get; set; }
		public View GetBaseView() { return this; }
		public View GetChromeView() { return null; }
		public View GetContentsView() { return _container; }
		public View GetOverlayView() { return _overlay; }

		/// <summary>
		/// Called when the container has been navigated to and is presented and ready on screen.
		/// </summary>
		public void OnNavigatedTo(NavigationElement fromElement)
		{
		}

		/// <summary>
		/// Called when the container has been navigated out/from and is dismissed and not longer visible on screen.
		/// </summary>
		public void OnNavigatedFrom(NavigationElement toContainer)
		{
		}

		#endregion

		#region Private Members

		Rectangle GetContentSize()
		{
			var s = (_container.Content as ContentView)
				.Measure(double.PositiveInfinity, double.PositiveInfinity);

			var contentSize = new Size(Width * 0.7, s.Request.Height);

			return new Rectangle(
				(_layout.Width / 2) - (contentSize.Width / 2),
				(_layout.Height / 2) - (contentSize.Height / 2),
				contentSize.Width, contentSize.Height);

		}

		#endregion
	}
}
