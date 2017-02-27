using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using NControl.Mvvm.Droid;
using Android.Graphics.Drawables;
using NControl.Mvvm;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace NControl.Mvvm.Droid
{
	public class ExtendedButtonRenderer : ButtonRenderer
	{
		GradientDrawable _normal, _pressed;
		float _displayDensity = 1.0f;

		public ExtendedButtonRenderer()
		{
			using (var metrics = Forms.Context.Resources.DisplayMetrics)
				_displayDensity = metrics.Density;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				var button = e.NewElement;

				// Create a drawable for the button's normal state
				_normal = new Android.Graphics.Drawables.GradientDrawable();

				// Create a drawable for the button's pressed state
				_pressed = new Android.Graphics.Drawables.GradientDrawable();

				UpdateBorder();

			}
		}

		void UpdateBorder()
		{ 
			_normal.SetStroke((int)(Element.BorderWidth * _displayDensity), Element.BorderColor.ToAndroid());
			_normal.SetCornerRadius(Element.BorderRadius * _displayDensity);

			if (Element.BackgroundColor.R.Equals(-1.0) && Element.BackgroundColor.G.Equals(-1.0) && Element.BackgroundColor.B.Equals(-1.0))
				_normal.SetColor(Android.Graphics.Color.Transparent);
			else
				_normal.SetColor(Element.BackgroundColor.ToAndroid());

			var highlight = Context.ObtainStyledAttributes(
				new int[] { 
				Android.Resource.Attribute.ColorActivatedHighlight 
			}).GetColor(0, Android.Graphics.Color.Gray);

			_pressed.SetColor(highlight);
			_pressed.SetStroke((int)(Element.BorderWidth * _displayDensity), Element.BorderColor.ToAndroid());
			_pressed.SetCornerRadius(Element.BorderRadius * _displayDensity);

			// Add the drawables to a state list and assign the state list to the button
			var sld = new StateListDrawable();

			if(!Element.BorderWidth.Equals(0))
				sld.AddState(new int[] { Android.Resource.Attribute.StatePressed }, _pressed);
			
			sld.AddState(new int[] { }, _normal);

			Control.SetBackground(sld);
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (_normal != null && _pressed != null)
			{
				if (e.PropertyName == nameof(Element.BorderRadius) ||
				    e.PropertyName == nameof(Element.BorderWidth) || 
				    e.PropertyName == nameof(Element.BorderColor))
				{
					UpdateBorder();
				}
			}
		}
	}
}
