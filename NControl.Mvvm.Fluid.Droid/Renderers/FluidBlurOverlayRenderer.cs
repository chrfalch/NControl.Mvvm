using System;
using Xamarin.Forms;
using NControl.Mvvm;
using NControl.Mvvm.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Android.Renderscripts;
using Android.Graphics.Drawables;
using Android.Views;
using System.Threading.Tasks;
using Android.App;

[assembly: ExportRenderer(typeof(FluidBlurOverlay), typeof(FluidBlurOverlayRenderer))]
namespace NControl.Mvvm.Droid
{
	public class FluidBlurOverlayRenderer : ViewRenderer<FluidBlurOverlay, FormsViewGroup>
	{
		Rect _lastRect;

		protected override void OnElementChanged(ElementChangedEventArgs<FluidBlurOverlay> e)
		{
			base.OnElementChanged(e);
		}

		protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged(w, h, oldw, oldh);

			var newRect = new Rect(0, 0, w, h);
			if (_lastRect != newRect)
			{
				// Find our top view
				var view = (Context as Activity).Window.DecorView;

				// Statusbar height
				Rect rect = new Rect();
				view.GetWindowVisibleDisplayFrame(rect);

				// Take screenshot
				view.DrawingCacheEnabled = true;
				var bitmap = Bitmap.CreateBitmap(view.DrawingCache);
				view.DrawingCacheEnabled = false;

				//var resultBmp = Bitmap.CreateBitmap(rect.Width(), rect.Height() - rect.Top, bitmap.GetConfig());
				//new Canvas(resultBmp).DrawBitmap(bitmap, 0, -rect.Top, null);
				var resultBmp = Bitmap.CreateBitmap(bitmap, 0, rect.Top, rect.Width(), rect.Height() - rect.Top);

				Background = new BitmapDrawable(CreateBlurredImage(25, resultBmp));

				_lastRect = newRect;
			}
		}

		Bitmap CreateBlurredImage(int radius, Bitmap originalBitmap)
		{
			// Create another bitmap that will hold the results of the filter.
			Bitmap blurredBitmap;
			blurredBitmap = Bitmap.CreateBitmap(originalBitmap);

			// Create the Renderscript instance that will do the work.
			var rs = RenderScript.Create(Forms.Context);

			// Allocate memory for Renderscript to work with
			var input = Allocation.CreateFromBitmap(rs, originalBitmap, Allocation.MipmapControl.MipmapFull, AllocationUsage.Script);
			var output = Allocation.CreateTyped(rs, input.Type);

			// Load up an instance of the specific script that we want to use.
			var script = ScriptIntrinsicBlur.Create(rs, Android.Renderscripts.Element.U8_4(rs));
			script.SetInput(input);

			// Set the blur radius
			script.SetRadius(radius);

			// Start the ScriptIntrinisicBlur
			script.ForEach(output);

			// Copy the output to the blurred bitmap
			output.CopyTo(blurredBitmap);

			return blurredBitmap;
		}
	}
}
