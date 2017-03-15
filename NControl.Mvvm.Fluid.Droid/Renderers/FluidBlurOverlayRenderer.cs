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
using Android.Util;

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
				var screenBmp = TakeScreenShot(Context as Activity);

				var resizedBmp = GetResizedBitmap(screenBmp, (int)(screenBmp.Width * 0.25), (int)(screenBmp.Height * 0.25));

				Background = new BitmapDrawable(CreateBlurredImage(25, resizedBmp));

				_lastRect = newRect;
			}
		}

		Bitmap GetResizedBitmap(Bitmap bm, int newWidth, int newHeight)
		{
			var width = bm.Width;
			var height = bm.Height;
			var scaleWidth = ((float)newWidth) / width;
			var scaleHeight = ((float)newHeight) / height;

			// CREATE A MATRIX FOR THE MANIPULATION
			Matrix matrix = new Matrix();

			// RESIZE THE BIT MAP
			matrix.PostScale(scaleWidth, scaleHeight);

			// "RECREATE" THE NEW BITMAP
			Bitmap resizedBitmap = Bitmap.CreateBitmap(
				bm, 0, 0, width, height, matrix, false);

			bm.Recycle();

			return resizedBitmap;
		}

		Bitmap TakeScreenShot(Activity activity)
		{
			var view = activity.Window.DecorView;

			view.DrawingCacheEnabled = true;
			view.BuildDrawingCache();

			var bitmap = view.DrawingCache;
			var rect = new Rect();
			activity.Window.DecorView.GetWindowVisibleDisplayFrame(rect);

			int statusBarHeight = rect.Top;

			Android.Graphics.Point displaySize = new Android.Graphics.Point();
			activity.WindowManager.DefaultDisplay.GetSize(displaySize);
			int width = displaySize.X;
			int height = displaySize.Y;

			var screenShotBitmap = Bitmap.CreateBitmap(bitmap, 0, statusBarHeight, width,
				 height - statusBarHeight);
			
			view.DestroyDrawingCache();
			return screenShotBitmap;
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
