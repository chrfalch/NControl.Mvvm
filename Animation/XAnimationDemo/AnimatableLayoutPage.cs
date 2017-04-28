using System;
using NControl.XAnimation;
using Xamarin.Forms;

namespace XAnimationDemo
{
	public class AnimatableLayoutPage: ContentPage
	{
		public AnimatableLayoutPage()
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
			var easing = new EasingFunctionBezier(.36, .83, .49, 1.03);

			layout.AddAnimation((l) => 
                  new XAnimationPackage(box1)						
	                    .Frame(0, 0, 60, 40)
	                    .Set()      
	                    .Duration(500)
	                    .Frame(0, 0, l.Width - 10, 150)
	                    .Easing(easing)
						.Animate());

			layout.Children.Add(box1);

			var box2 = new BoxView { BackgroundColor = Color.Silver };

			layout.AddAnimation((l) => 
                  new XAnimationPackage(box2)
	                    .Frame(70, 0, l.Width-80, 10)
	                    .Set()
	                    .Duration(500)
	                    .Frame(0, 160, l.Width-10, 40)	
	                    .Easing(easing)
						.Animate());

			layout.Children.Add(box2);

			var box3 = new BoxView { BackgroundColor = Color.Silver };

			layout.AddAnimation((l) => 
                  new XAnimationPackage(box3)
						.Frame(70, 20, l.Width-80, 10)
	                    .Set()
	                    .Frame(0, 210, l.Width-10, 10)
	                    .Duration(500)
	                    .Easing(EasingFunction.EaseOut)
						.Animate());

			layout.Children.Add(box3);

			Content = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Children = {
					layout,
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.Center,
						Children = {
							new Button{
								Text = "Animate",
								Command = new Command((obj) => {
									layout.Animate();
								}),
															
							},

							new Button{
								Text = "Reverse",
								Command = new Command((obj) => {
									layout.Reverse();
								}),
							}
						}
					},
					slider,
				},
			};
		}
	}
}
