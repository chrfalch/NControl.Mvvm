using System;
using Xamarin.Forms;
using UIKit;
using System.Linq;
using Xamarin.Forms.Platform.iOS;

namespace NControl.Mvvm.iOS
{
	public class TouchGestureRecognizerProvider: IGestureRecognizerProvider
	{
		#region Events

		/// <summary>
		/// Occurs when touched.
		/// </summary>
		public event EventHandler<GestureRecognizerEventArgs> Touched
		{
			add { _reco.Touched += value; }
			remove { _reco.Touched -= value; }
		}

		#endregion

		#region Private Members

		readonly TouchGestureRecognizer _reco = new TouchGestureRecognizer();

		#endregion

		/// <summary>
		/// Attaches to an element
		/// </summary>
		/// <param name="element">Element.</param>
		public void AttachTo(VisualElement element)
		{
			element.PropertyChanged += (sender, e) => {
				if (e.PropertyName == "Renderer")
				{
					var r = Platform.GetRenderer(element);
					if (r != null)
					{
						r.NativeView.AddGestureRecognizer(_reco);
					}
				}
			};
		}

		/// <summary>
		/// Detaches from an element
		/// </summary>
		public void DetachFrom(VisualElement element)
		{
			var renderer = Platform.GetRenderer(element);
			if (renderer != null)
				renderer.NativeView.RemoveGestureRecognizer(_reco);
		}
	}		
}
