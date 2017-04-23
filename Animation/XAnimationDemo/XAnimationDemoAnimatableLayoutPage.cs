using System;
using NControl.XAnimation;
using Xamarin.Forms;

namespace XAnimationDemo
{
	public class XAnimationDemoAnimatableLayoutPage: ContentPage
	{
		public XAnimationDemoAnimatableLayoutPage()
		{
			var slider = new Slider
			{
				Minimum = 0.0,
				Maximum = 1.0,
				Value = 0.0,
				Margin = 20,
			};

			var layout = new XAnimatableLayout { 
				Margin = 24,	
			};

			slider.ValueChanged += (sender, e) => {
				layout.Interpolation = slider.Value;
			};

			var box1 = new BoxView { BackgroundColor = Color.Red };

			layout.AddAnimation((l) => 
                  new XAnimationPackage(box1)
						.Frame(0, 0, 60, 40)
	                    .Set()	                    
	                    .Frame(0, 0, l.Width - 10, 150)
						.Animate());

			layout.Children.Add(box1);

			var box2 = new BoxView { BackgroundColor = Color.Silver };

			layout.AddAnimation((l) => 
                  new XAnimationPackage(box2)
						.Frame(70, 0, l.Width-80, 10)
	                    .Set()
	                    .Frame(0, 160, l.Width-10, 20)
						.Animate());

			layout.Children.Add(box2);

			var box3 = new BoxView { BackgroundColor = Color.Silver };

			layout.AddAnimation((l) => 
                  new XAnimationPackage(box3)
						.Frame(70, 20, l.Width-80, 10)
	                    .Set()
	                    .Frame(0, 195, l.Width-10, 10)
						.Animate());

			layout.Children.Add(box3);

			Content = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Children = {
					layout,
					slider,
				},
			};
		}
	}
}
