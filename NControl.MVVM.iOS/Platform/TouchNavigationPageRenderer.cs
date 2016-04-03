using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using NControl.Mvvm.iOS;
using NControl.Mvvm;
using UIKit;

[assembly:ExportRenderer(typeof(ModalNavigationPage), typeof(TouchNavigationPageRenderer))]
namespace NControl.Mvvm.iOS
{
	/// <summary>
	/// Touch page renderer.
	/// </summary>
	public class TouchNavigationPageRenderer: NavigationRenderer
	{
		/// <summary>
		/// Wills the move to parent view controller.
		/// </summary>
		/// <param name="parent">Parent.</param>
		public override void WillMoveToParentViewController (UIViewController parent)
		{
			base.WillMoveToParentViewController (parent);
		
			// Check if we have a child controller 
			if (ChildViewControllers.Length > 0) 
			{				
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {

					// Check if firstviewmodel is set and has a presentation mode of modal
					if ((Element as ModalNavigationPage).FirstChildViewModel.PresentationMode == PresentationMode.Modal) {

						parent.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
					}
				}
			}
		}

		/// <summary>
		/// Views the will appear.
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {

				// Check if firstviewmodel is set and has a presentation mode of modal
				if ((Element as ModalNavigationPage).FirstChildViewModel.PresentationMode == PresentationMode.Modal) {

					var parent = ParentViewController;
					var sv = parent.View.Superview;
					sv.Layer.CornerRadius = 0;
				}
			}
		}
	}
}

