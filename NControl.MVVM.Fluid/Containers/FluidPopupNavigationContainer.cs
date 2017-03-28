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
		readonly Grid _container;
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
			_container = new Grid();
			_containerContents = new FluidRoundCornerView
			{
				BorderRadius = (int)MvvmApp.Current.Sizes.Get(FluidConfig.DefaultPopupCornerRadius),
				BorderColor = MvvmApp.Current.Colors.Get(Config.BorderColor),
				BorderWidth = MvvmApp.Current.Sizes.Get(Config.DefaultBorderSize),
				Content = _container,
			};

			_containerBorders = new FluidShadowView{
				HasShadow = true,
				BorderRadius = (int)MvvmApp.Current.Sizes.Get(FluidConfig.DefaultPopupCornerRadius),
				Content = _containerContents
			};

			_layout.Children.Add(_containerBorders, () => GetContentSize());
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

		#endregion

		#region Transitions

		public IEnumerable<XAnimationPackage> TransitionIn(
			View view, PresentationMode presentationMode)
		{
			var retVal = new[] {
				new XAnimationPackage(_overlay)
					.Opacity(0.0)
					.Set()
					.Duration(150)
					.Easing(EasingFunction.EaseIn)
					.Opacity(1.0)
					.Animate(),

				new XAnimationPackage(_containerBorders)
					.Scale(1.3)
					.Opacity(0.0)
					.Set()
					.Duration(Duration)
					.Easing(EasingFunction.EaseIn)
					.Opacity(1.0)
					.Scale(1.0)
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
					.Duration(Duration)
					.Easing(EasingFunction.EaseOut)
				    .Animate(),

				new XAnimationPackage(_containerBorders)
					.Scale(1.3)
					.Opacity(0.0)
					.Duration(Duration)
					.Easing(EasingFunction.EaseOut)
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
			var s = (_container.Children.LastOrDefault() as ContentView).Measure(double.PositiveInfinity, double.PositiveInfinity);
			var contentSize = new Size(Width * 0.7, s.Request.Height);

			//var lastView = _container.Children.LastOrDefault(); // as IContentSizeProvider;
			//var contentSize = ContentSize;
			//if (lastView != null)
			//{
			//	if (!lastView.ContentSize.Width.Equals(0.0))
			//		contentSize.Width = lastView.ContentSize.Width;

			//	if (!lastView.ContentSize.Height.Equals(0.0))
			//		contentSize.Height = lastView.ContentSize.Height;
			//}

			return new Rectangle(
				(_layout.Width / 2) - (contentSize.Width / 2),
				(_layout.Height / 2) - (contentSize.Height / 2),
				contentSize.Width, contentSize.Height);

		}
	}
}
