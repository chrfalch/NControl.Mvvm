using System;
using System.Linq;
using CoreGraphics;
using NControl.Mvvm.Fluid;
using NControl.Mvvm.Fluid.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(FluidNavigationContainer), typeof(FluidNavigationContainerRenderer))]
namespace NControl.Mvvm.Fluid.iOS
{
	public class FluidNavigationContainerRenderer: ViewRenderer<FluidNavigationContainer, UIView> 
	{
		UIPanGestureRecognizer _reco;
		CGPoint _startLocation;

		public FluidNavigationContainerRenderer()
		{
			_reco = new UIPanGestureRecognizer(HandleSwipe);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<FluidNavigationContainer> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
				_reco.Enabled = Element.BackButtonVisible;
		}

		protected override void UpdateNativeWidget()
		{
			base.UpdateNativeWidget();

			if (GestureRecognizers == null || !GestureRecognizers.Contains(_reco))
				AddGestureRecognizer(_reco);
		}

		void HandleSwipe(UIPanGestureRecognizer reco)
		{
			if (reco.State == UIGestureRecognizerState.Began)
			{
				_startLocation = reco.LocationInView(this);

				if (Element != null)
					Element.UpdateFromGestureRecognizer(_startLocation.X, -1, PanState.Started);
			}
			else if (reco.State == UIGestureRecognizerState.Changed)
			{
				var stopLocation = reco.LocationInView(this);
				if (Element != null)
					Element.UpdateFromGestureRecognizer(stopLocation.X, -1, PanState.Moving);
			}
			else if (reco.State == UIGestureRecognizerState.Ended)
			{
				var stopLocation = reco.LocationInView(this);
				var velocity = reco.VelocityInView(this);

				if (Element != null)
					Element.UpdateFromGestureRecognizer(stopLocation.X, (double)velocity.X, PanState.Ended);
			}
			else if (reco.State == UIGestureRecognizerState.Cancelled)
			{
				if (Element != null)
					Element.UpdateFromGestureRecognizer(-1, -1, PanState.Cancelled);
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == nameof(FluidNavigationContainer.BackButtonVisible))
			{
				_reco.Enabled = Element.BackButtonVisible;
			}
		}
	}
}
