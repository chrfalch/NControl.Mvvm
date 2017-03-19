using System;
using NControl.XAnimation;
using Xamarin.Forms;
namespace XAnimationDemo
{
	public class XAnimationDemoPage3 : ContentPage
	{
		readonly MyRelativeLayout _layout;
		bool _isExpanded;

		Rectangle GetLabelRectangle()
		{
			if (_isExpanded)
				return new Rectangle(0, 0, _layout.Width, _layout.Height * 0.75);
			
			return new Rectangle(40, 40, _layout.Width-80, _layout.Height * 0.5);
		}

		Rectangle GetImageRectangle()
		{
			var retVal = GetLabelRectangle();
			retVal.X = 0;
			retVal.Y = 0;
			return retVal;
		}

		public XAnimationDemoPage3()
		{
			var image = new Image
			{
				Source = "https://upload.wikimedia.org/wikipedia/commons/2/21/EverestfromKalarPatarcrop.JPG",
				Aspect = Aspect.AspectFill,
				Opacity = 0.4,
			};

			var label = new Label
			{
				Text = "Hello World",
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center
			};

			var contentView = new Grid
			{
				BackgroundColor = Color.Aqua,
				Children = {
					// image,
					new ContentView{
						Content = label,
						BackgroundColor = Color.Lime,
						Margin = 10,
					}
				}
			};

			_layout = new MyRelativeLayout();
			_layout.BackgroundColor = Color.Yellow;
			_layout.HeightRequest = 140;
			_layout.Children.Add(contentView, ()=> GetLabelRectangle());

			var button = new Button
			{
				Text = "Animate!",
				Command = new Command(() => {

					_isExpanded = !_isExpanded;

					new XAnimationPackage(contentView)
						.Duration(1250)
						.Easing(.68, .19, .77, 1.32)
						.Frame(GetLabelRectangle())
						.Animate()
						.Run();

					//new XAnimationPackage(image)
					//	.Duration(1250)
					//	.Easing(.68, .19, .77, 1.32)
					//	.Frame(GetImageRectangle())
					//	.Animate()
					//	.Run();
				}),
			};

			// The root page of your application
			Title = "XAnimationDemo3";
			Content = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				Padding = 14,
				Spacing = 50,
				Children = {
					_layout,
					button,
				}
			};
		}
	}

	public class MyRelativeLayout : RelativeLayout
	{ 
		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			base.LayoutChildren(x, y, width, height);
		}

		protected override void InvalidateLayout()
		{
			base.InvalidateLayout();
		}

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			return base.OnMeasure(widthConstraint, heightConstraint);
		}
	}
}
