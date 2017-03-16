using NControl.Mvvm;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using NControl.Mvvm.Droid;
using System;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(FluidRoundCornerView), typeof(FluidRoundedCornerViewRenderer))]
namespace NControl.Mvvm.Droid
{
	public class FluidRoundedCornerViewRenderer : ViewRenderer<FluidRoundCornerView, FormsViewGroup>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<FluidRoundCornerView> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null && e.OldElement == null)
				Background = new BorderDrawable(e.NewElement, this);
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
				e.PropertyName == FluidRoundCornerView.BorderColorProperty.PropertyName ||
				e.PropertyName == FluidRoundCornerView.BorderRadiusProperty.PropertyName ||
				e.PropertyName == FluidRoundCornerView.BorderWidthProperty.PropertyName)
				Background.InvalidateSelf();
		}

		public override void Draw(Canvas canvas)
		{
			using (var clipPath = new Path())
			{
				float rx = Forms.Context.ToPixels(Element.BorderRadius);
				float ry = Forms.Context.ToPixels(Element.BorderRadius);
				clipPath.AddRoundRect(new RectF(canvas.ClipBounds), rx, ry, Path.Direction.Cw);
				canvas.ClipPath(clipPath);

				base.Draw(canvas);

				if (Element.BorderColor != Xamarin.Forms.Color.Transparent)
				{
					using (var paint = new Paint { AntiAlias = true })
					using (var path = new Path())
					using (Path.Direction direction = Path.Direction.Cw)
					using (Paint.Style style = Paint.Style.Stroke)
					using (var rect = new RectF(0, 0, Width, Height))
					{
						path.AddRoundRect(rect, rx, ry, direction);

						var strokeWidth = (((float)Element.BorderWidth) * Resources.DisplayMetrics.Density);

						paint.StrokeWidth = strokeWidth;
						paint.SetStyle(style);
						paint.Color = Element.BorderColor.ToAndroid();

						canvas.DrawPath(path, paint);
					}
				}			
			}
		}
	}

	public class BorderDrawable: Drawable
	{
		public FluidRoundCornerView Element { get; private set; }
		public FluidRoundedCornerViewRenderer Renderer { get; private set; }
		public BorderDrawable(FluidRoundCornerView element, FluidRoundedCornerViewRenderer renderer)
		{
			Element = element;
			Renderer = renderer;
		}

		public override void Draw(Canvas canvas)
		{
			var element = Element;
			var cornerRadius = (float)element.BorderRadius * Renderer.Resources.DisplayMetrics.Density;

			// Paint rounded rect itself
			var strokeWidth = (((float)element.BorderWidth) * Renderer.Resources.DisplayMetrics.Density);

			if (element.BackgroundColor != Xamarin.Forms.Color.Transparent)
			{
				using (var paint = new Paint { AntiAlias = true })
				using (var path = new Path())
				using (Path.Direction direction = Path.Direction.Cw)
				using (Paint.Style style = Paint.Style.Fill)
				using (var rect = new RectF(0, 0, Renderer.Width, Renderer.Height))
				{
					float rx = Forms.Context.ToPixels(cornerRadius);
					float ry = Forms.Context.ToPixels(cornerRadius);
					path.AddRoundRect(rect, rx, ry, direction);

					global::Android.Graphics.Color color = Element.BackgroundColor.ToAndroid();

					paint.SetStyle(style);
					paint.Color = color;

					canvas.DrawPath(path, paint);
				}
			}

			if (element.BorderColor != Xamarin.Forms.Color.Transparent)
			{
				using (var paint = new Paint { AntiAlias = true })
				using (var path = new Path())
				using (Path.Direction direction = Path.Direction.Cw)
				using (Paint.Style style = Paint.Style.Stroke)
				using (var rect = new RectF(0, 0, Renderer.Width, Renderer.Height))
				{
					float rx = Forms.Context.ToPixels(cornerRadius);
					float ry = Forms.Context.ToPixels(cornerRadius);
					path.AddRoundRect(rect, rx, ry, direction);

					paint.StrokeWidth = strokeWidth;
					paint.SetStyle(style);
					paint.Color = Element.BorderColor.ToAndroid();

					canvas.DrawPath(path, paint);
				}
			}

		}

		public override void SetAlpha(int alpha)
		{
			throw new NotImplementedException();
		}

		public override void SetColorFilter(ColorFilter colorFilter)
		{
			throw new NotImplementedException();
		}

		public override int Opacity
		{
			get { return 0; }
		}
	}
}
